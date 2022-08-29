using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatWarp : ProgressTask
{
	[SerializeField] Transform warpPosition;
	protected new void Start()
	{
		base.Start();
	}

	void Update()
	{
		if (paused) return;

		foreach (Rat rat in GameManager.Instance.TaskManager.RatsOnTask(this))
		{
			if (rat.ArrivedAtTask)
			{
				WarpRatToObjectPosition(taskPoints[0].rat);
			}
		}

	}

	public void WarpRatToObjectPosition(Rat r)
	{
		r.NavAgent.Warp(warpPosition.position);
		GameManager.Instance.TaskManager.UnassignRats(r);
	}

	protected override void OnActivate() { }
	protected override void OnComplete() { }
	protected override void OnUnlock()
	{
		TaskState = State.Unlocked;
	}
}
