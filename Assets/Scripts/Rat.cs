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
	public bool Occupied => TaskManager.Instance.ratTasks[this] != null;
	[field: SerializeField] public bool Wandering { get; private set; }

	public RatData Info { get; private set; }

	public bool AtTask;

	private void Start()
	{
		anim = GetComponentInChildren<Animator>();
		NavAgent = GetComponent<NavMeshAgent>();

		if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 0.1f, NavMesh.AllAreas))
		{
			transform.position = hit.position;
		}
		SetWander();
	}

	private void Update()
	{
		anim.SetFloat("movementSpeed", NavAgent.velocity.magnitude);
		anim.SetBool("wandering", Wandering);
		anim.SetBool("occupied", Occupied && TaskManager.Instance.RatInPlace(this));

		// Reducing flicker
		graphic.flipX = NavAgent.velocity.x switch
		{
			< 0.05f => true,
			> -0.05f => false,
			_ => graphic.flipX
		};

		if (!Occupied)
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
		NavAgent.speed = 2f * Info.SpeedModifier;
		Wandering = false;
	}

	public void AtTaskPoint()
	{
		AtTask = true;
		anim.SetBool("occupied", true);
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

	public void Kill()
	{
		RatManager.Instance.RemoveRat(this);
		// Leave corpse?
		Destroy(gameObject);
	}

	public void Select()
	{
		graphic.color = Color.green;
	}

	public void Deselect()
	{
		graphic.color = Color.white;
	}
}
