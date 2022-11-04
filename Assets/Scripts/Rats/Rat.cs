using UnityEngine;
using UnityEngine.AI;

public class Rat : MonoBehaviour
{
	[SerializeField] SpriteRenderer graphic;
	public RatData Info { get; private set; }
	Animator anim;
	RatEmotes ratEmotes;

	[Header("Navigation")]
	private const float StoppingDistance = 0.07f;
	[field: SerializeField] public NavMeshAgent NavAgent { get; private set; }
	[field: SerializeField] public bool Wandering { get; private set; }
	public bool AssignedToTask => GameManager.Instance.TaskManager.ratTasks.ContainsKey(this);

	[Header("Carrying")]
	public Transform holdSpot;
	public Pickupable heldItem;
	public bool IsHoldingItem => heldItem;

	[Header("Selector")]
	[SerializeField] SpriteRenderer selectionSprite;
	[SerializeField] float selectionBobSpeed = 2f;
	[SerializeField] float selectionBobHeight = 0.015f;
	Vector3 selectionCachePos;
	float selectionBobOffset;

	private float patience;
	private bool isDead = false;

	bool paused;

	[SerializeField] float deathTimer = 5f;

	private void Awake()
	{
		GameManager.Pause += SetPaused;
		ratEmotes = GetComponentInChildren<RatEmotes>();
		anim = GetComponentInChildren<Animator>();
		NavAgent = GetComponent<NavMeshAgent>();
		selectionCachePos = selectionSprite.transform.localPosition;
		selectionBobOffset = Random.value;
	}

	private void Start()
	{
		if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 0.1f, NavMesh.AllAreas))
		{
			transform.position = hit.position;
		}
		if (!NavAgent.hasPath)
		{
			SetWander();
		}
	}

	private void Update()
	{
		if (paused) return;

		if (isDead)
		{
			deathTimer -= Time.deltaTime;
			if (deathTimer <= 0)
			{
				Destroy(gameObject);
			}
			return;
		}

		anim.SetFloat("movementSpeed", NavAgent.velocity.magnitude);
		anim.SetBool("wandering", Wandering);
		anim.SetBool("carrying", IsHoldingItem);

		SetTaskAnimation(ArrivedAtTask());

		// Reducing flicker
		graphic.flipX = NavAgent.velocity.x switch
		{
			< 0.01f => true,
			> -0.01f => false,
			_ => graphic.flipX
		};

		if (!AssignedToTask)
		{
			if (Vector3.Distance(transform.position, NavAgent.destination) < StoppingDistance)
			{
				NavAgent.ResetPath();
				patience -= Time.deltaTime;
				if (patience <= 0)
				{
					SetWander();
				}
			}
		}
		selectionSprite.transform.localPosition = selectionCachePos + (Mathf.Sin((Time.time + selectionBobOffset) * selectionBobSpeed) * selectionBobHeight * Vector3.up);
	}
	public void AssignInfo(RatData ratInfo) => Info = ratInfo;

	/// <summary>
	/// Gives the rat a destination to move to
	/// </summary>
	/// <param name="position"></param>
	public void SetDestination(Vector3 position)
	{
		NavAgent.SetDestination(position);
		patience = Info.PatienceDuration;
		NavAgent.speed = (IsHoldingItem ? 1.5f : 2f) * Info.SpeedModifier;
		Wandering = false;
	}

	/// <summary>
	/// Sets the rat to wander
	/// </summary>
	public void SetWander()
	{
		Wandering = true;
		NavAgent.speed = 0.25f * Info.SpeedModifier;
		Vector2 rand = Random.insideUnitCircle * Info.WanderRadius;
		Vector3 offsetted = new(transform.position.x + rand.x, transform.position.y, transform.position.z + rand.y);
		if (NavMesh.SamplePosition(offsetted, out NavMeshHit nMHit, 200f, NavMesh.AllAreas))
		{
			NavAgent.SetDestination(nMHit.position);
		}
	}

	public void Pickup(Pickupable item)
	{
		heldItem = item;
		heldItem.transform.parent = holdSpot;
		heldItem.transform.localPosition = Vector3.zero;
		if (heldItem.TryGetComponent(out Rigidbody rb))
		{
			rb.isKinematic = false;
		}
		if (heldItem.destinationSprite)
		{
			ratEmotes.SetEmote(heldItem.destinationSprite);
		}
	}

	public void SetEmote(RatEmotes.Emotes emote) => ratEmotes.SetEmote(emote);

	void SetPaused(bool paused)
	{
		this.paused = paused;
		NavAgent.isStopped = paused;
		anim.speed = paused ? 0 : 1;
	}

	public bool ArrivedAtTask()
	{
		if (GameManager.Instance.TaskManager.GetDistanceToTask(this) == -1)
		{
			return false;
		}
		else return GameManager.Instance.TaskManager.GetDistanceToTask(this) < 0.1f;
	}

	public void Kill()
	{
		isDead = true;
		Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f);
		foreach (Collider collider in colliders)
		{
			if (collider.gameObject.TryGetComponent(out Rat r))
			{
				r.SetEmote(RatEmotes.Emotes.Sad);
			}
		}

		GameManager.Instance.RatManager.RemoveRat(this);
		GameManager.Instance.RatManager.QueueRatForRespawn(Info);
		anim.SetTrigger("Dead");

		if (GameManager.Instance.Scorer)
		{
			GameManager.Instance.Scorer.AddDeath();
		}
	}

	public void Select()
	{
		LeanTween.scaleY(selectionSprite.gameObject, 1, 0.1f).setEaseOutBack();
	}

	public void Deselect()
	{
		LeanTween.scaleY(selectionSprite.gameObject, 0, 0.1f).setEaseInBack();
	}

	void SetTaskAnimation(bool doAnimation)
	{
		TaskPoint.TaskAnimation taskAnim = TaskPoint.TaskAnimation.Null;
		if (doAnimation)
		{
			taskAnim = GameManager.Instance.TaskManager.GetTaskPoint(this).animationName;
		}

		anim.SetBool("fish", taskAnim == TaskPoint.TaskAnimation.Fish);
		anim.SetBool("pull", taskAnim == TaskPoint.TaskAnimation.Pull);
		anim.SetBool("push", taskAnim == TaskPoint.TaskAnimation.Push);
		anim.SetBool("spin", taskAnim == TaskPoint.TaskAnimation.Spin);
	}

	private void OnDestroy()
	{
		GameManager.Pause -= SetPaused;
	}

	public void SetColor()
	{
		graphic.material.SetColor("_LitColor", Info.lightColor);
		graphic.material.SetColor("_ShadowColor", Info.darkColor);
	}
}
