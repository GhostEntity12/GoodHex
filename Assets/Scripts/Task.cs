using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Task : MonoBehaviour
{
	[SerializeField] TaskPoint[] taskPoints;
	bool SlotsAvailable => taskPoints.Any(p => !(p.rat == null));
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public List<Rat> AssignRats(ref List<Rat> rats)
	{
		if (!SlotsAvailable) return null;

		Queue<TaskPoint> availablePoints = new(taskPoints.Where(p => p.rat == null));
		Queue<Rat> ratQueue = new(rats);

		while (availablePoints.Count > 0 && ratQueue.Count > 0)
		{
			Rat r = ratQueue.Dequeue();
			TaskPoint p = availablePoints.Dequeue();
			r.SetTask(transform.position + p.relativePosition);
			Debug.Log(transform.position + p.relativePosition);
			p.rat = r;
		}

		return ratQueue.ToList();
	}

	private void OnDrawGizmos()
	{
		foreach (TaskPoint point in taskPoints)
		{
			Gizmos.color = new Color(0.5f, 0.3f, 0f, 0.75f);
			Gizmos.DrawCube(transform.position + point.relativePosition, Vector3.one * 0.5f);
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(transform.position + point.relativePosition, Vector3.one * 0.5f);
		}
	}
}

[System.Serializable]
public class TaskPoint
{
	public Vector3 relativePosition;
	public Rat rat;
}
