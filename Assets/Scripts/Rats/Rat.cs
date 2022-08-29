using UnityEngine;
using UnityEngine.AI;

public class Rat : MonoBehaviour
{
	[SerializeField] SpriteRenderer graphic;
	Animator anim;

	[Header("Navigation")]
	private const float StoppingDistance = 0.15f;
	[field: SerializeField] public NavMeshAgent NavAgent { get; private set; }

	private float patience;
	public bool AssignedToTask => GameManager.Instance.TaskManager.ratTasks.ContainsKey(this);
	public bool ArrivedAtTask
	{
		get
		{
			TaskPoint tp = GameManager.Instance.TaskManager.GetTaskPoint(this);
			if (tp == null) return false;

			return Vector3.Distance(GameManager.Instance.TaskManager.GetTaskPoint(this).taskPosition, transform.position) < 0.1f;
		}
	}
	[field: SerializeField] public bool Wandering { get; private set; }

	public RatData Info { get; private set; }

	RatEmotes ratEmotes;
	PickUp pickUp;

	bool paused;

	private bool isDead = false;

	private void Start()
	{
		GameManager.Pause += SetPaused;

		ratEmotes = GetComponentInChildren<RatEmotes>();
		anim = GetComponentInChildren<Animator>();
		NavAgent = GetComponent<NavMeshAgent>();
		pickUp = GetComponent<PickUp>();

		if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 0.1f, NavMesh.AllAreas))
		{
			transform.position = hit.position;
		}
		SetWander();
	}

	private void Update()
	{
		if (paused) return;

		anim.SetFloat("movementSpeed", NavAgent.velocity.magnitude);
		anim.SetBool("wandering", Wandering);
		anim.SetBool("occupied", ArrivedAtTask);
		anim.SetBool("carrying", pickUp.IsHoldingItem);

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
		NavAgent.speed = (pickUp.IsHoldingItem ? 1f : 2f) * Info.SpeedModifier;
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

	public void SetEmote(RatEmotes.Emotes emote) => ratEmotes.SetEmote(emote);

	void SetPaused(bool paused)
	{
		this.paused = paused;
		NavAgent.isStopped = paused;
		anim.speed = paused ? 0 : 1;
	}

	public void Kill(float pauseTime)
	{
		isDead = true;
		NavAgent.isStopped = true;
		GameManager.Instance.RatManager.RemoveRat(this);
		// Leave corpse?
		Invoke("Remove", pauseTime);
	}

	public void Select()
	{
		graphic.color = Color.green;
	}

	public void Deselect()
	{
		graphic.color = Color.white;
	}
	private void OnDestroy()
	{
		GameManager.Pause -= SetPaused;
	}

	private void Remove() 
	{
		Destroy(gameObject);
	}
}
