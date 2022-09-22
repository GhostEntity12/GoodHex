﻿using UnityEngine;

public class Assignable : BaseTask
{
	[SerializeField, Tooltip("The points at which rats stand to do the task")]
	protected TaskPoint[] taskPoints = new TaskPoint[0];
	public TaskPoint[] TaskPoints => taskPoints;


	[ContextMenu("Reset Task Positions")]
	void ResetTaskPositions()
	{
		foreach (TaskPoint point in taskPoints)
		{
			point.taskPosition = transform.position;
		}
	}

	/// <summary>  
	/// Draws TaskPoint positions
	/// </summary>
	private void OnDrawGizmos()
	{
		if (taskPoints.Length == 0) return;

		foreach (TaskPoint point in taskPoints)
		{
			Gizmos.color = new Color(0.5f, 0.3f, 0f, 0.75f);
			Gizmos.DrawSphere(point.taskPosition, 0.05f);
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(point.taskPosition, 0.05f);
		}
	}

	protected override void OnActivate() { }
	protected override void OnComplete() { }
	protected override void OnUnlock() { }
}

[System.Serializable]
public class TaskPoint
{
	public Vector3 taskPosition;
	public Rat rat;
}
