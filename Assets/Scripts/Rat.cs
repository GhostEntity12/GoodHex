using UnityEngine;
using UnityEngine.AI;

public class Rat : MonoBehaviour
{
	[SerializeField] SpriteRenderer graphic;

	[Header("Navigation")]
	private const float StoppingDistance = 1f;
	[field: SerializeField] public NavMeshAgent NavAgent { get; private set; }

	private float patience;

	/// <summary>
	/// Whether the rat is dong a task
	/// </summary>
	private bool occupied;

	[field: SerializeField] public bool Wandering { get; private set; }

	public RatData Info { get; private set; }

	private void Start()
	{
		NavAgent = GetComponent<NavMeshAgent>();
		SetWander();
	}

	private void Update()
	{
		if (!occupied)
		{
			if (Vector3.Distance(transform.position, NavAgent.destination) < StoppingDistance)
			{
				//NavAgent.ResetPath();
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

	public void SetTask(Vector3 point)
	{
		occupied = true;
		SetDestination(point);
	}

	public void UnsetTask() => occupied = false;

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
