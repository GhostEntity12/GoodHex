﻿using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class Assignable : BaseTask
{
	[SerializeField, Tooltip("The points at which rats stand to do the task")]
	protected TaskPoint[] taskPoints = new TaskPoint[0];
	public TaskPoint[] TaskPoints => taskPoints;

	protected Renderer r;

	new protected void Start()
	{
		r = GetComponent<Renderer>();
		base.Start();
	}

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

	/// <summary>
	/// Highlights the task
	/// </summary>
	/// <param name="doHighlight"></param>
	public void Highlight(bool doHighlight)
	{
		GameManager.Instance.Reticle.SetAssignable(doHighlight ? this : null);

		if (TaskState == State.Unlocked && (GameManager.Instance.RatManager.HasSelectedRats || !doHighlight))
		{
			GameManager.Instance.Highlighter.Highlight(r, doHighlight);
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

