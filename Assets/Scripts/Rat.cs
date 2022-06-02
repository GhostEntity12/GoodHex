using UnityEngine;
using UnityEngine.AI;

public class Rat : MonoBehaviour
{
	[SerializeField] SpriteRenderer graphic;
	Animator anim;

	[Header("Navigation")]
	private const float StoppingDistance = 1.5f;
	[field: SerializeField] public NavMeshAgent NavAgent { get; private set; }

	private float patience;

	public bool Occupied => Task != null;
	public TaskPoint Task { get; private set; }
	[field: SerializeField] public bool Wandering { get; private set; }

	public RatData Info { get; private set; }

	private void Start()
	{
		anim = GetComponentInChildren<Animator>();
		NavAgent = GetComponent<NavMeshAgent>();
		SetWander();
	}

	private void Update()
	{
		anim.SetFloat("movementSpeed", NavAgent.velocity.magnitude);
		anim.SetBool("wandering", Wandering);

		// Reducing flicker
		graphic.flipX = NavAgent.velocity.x switch
		{
			> 0.1f => true,
			< -0.1f => false,
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

	public void InPlace()
	{
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

	public void SetTask(TaskPoint point)
	{
		Task = point;
		SetDestination(point.taskPosition);
	}

	public void UnsetTask()
	{
		Task?.UnsetRat();
		Task = null;
		anim.SetBool("occupied", false);
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
