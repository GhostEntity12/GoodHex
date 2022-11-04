using System.Collections.Generic;
using UnityEngine;

public abstract class Hazard : MonoBehaviour
{
	[SerializeField] Bounds bounds;

	protected bool Activate()
	{
		Queue<Rat> deadRats = new();
		foreach (Rat rat in GameManager.Instance.RatManager.allRats)
		{
			if (bounds.Contains(rat.transform.position))
			{
				deadRats.Enqueue(rat);
			}
		}
		bool killingRats = deadRats.Count > 0;
		while (deadRats.Count > 0)
		{
			deadRats.Dequeue().Kill();
		}
		return killingRats;
	}

	[ContextMenu("Center")]
	void CenterHazard()
	{
		bounds.center = transform.position + Vector3.up * bounds.extents.y;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new(0.65f, 0.15f, 0.15f, 0.4f);
		Gizmos.DrawCube(bounds.center, bounds.size);
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(bounds.center, bounds.size);
	}
}
