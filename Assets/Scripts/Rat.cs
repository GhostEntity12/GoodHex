using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Rat : MonoBehaviour
{
	[Header("Navigation")]
	private readonly float stoppingDistance = 1f;
	[field: SerializeField]	public NavMeshAgent NavAgent { get; private set; }
	

	private float patience;
	bool occupied;
	[field: SerializeField]	public bool Wandering { get; private set; }

	public RatData Info => info;

	RatData info;

	private void Start()
	{
		NavAgent = GetComponent<NavMeshAgent>();
		SetWander();
	}

	public void Setup(RatData ratInfo)
	{
		info = ratInfo;
	}

	private void Update()
	{
		if (!occupied)
		{
			if (Vector3.Distance(transform.position, NavAgent.destination) < stoppingDistance)
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

	public void SetDestination(Vector3 position)
	{
		NavAgent.SetDestination(position);
		patience = info.PatienceDuration;
		NavAgent.speed = 10f * Info.SpeedModifier;
		Wandering = false;
	}

	/// <summary>
	/// Sets the rat to wander
	/// </summary>
	public void SetWander()
	{
		Wandering = true;
		NavAgent.speed = 0.75f * Info.SpeedModifier;
		Vector2 rand = Random.insideUnitCircle * info.WanderRadius;
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
}
