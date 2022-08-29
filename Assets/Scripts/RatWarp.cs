using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatWarp : ProgressTask
{
	[SerializeField] Transform warpPosition;
	protected new void Start()
	{
		base.Start();
		TaskState = State.Unlocked;
	}

	void Update()
	{
		if (paused) return;

		if (GameManager.Instance.TaskManager.RatsOnTask(this).Count > 0)
		{
			List<Rat> ratsToDeselect = new();
			foreach (Rat rat in GameManager.Instance.TaskManager.RatsOnTask(this))
			{
				if (rat.ArrivedAtTask)
				{
					ratsToDeselect.Add(rat);
					WarpRatToObjectPosition(taskPoints[0].rat);
				}
			}
			GameManager.Instance.TaskManager.UnassignRats(ratsToDeselect.ToArray());
		}
	}

	public void WarpRatToObjectPosition(Rat r)
	{
		r.NavAgent.Warp(warpPosition.position);
	}

	protected override void OnActivate() { }
	protected override void OnComplete() { }
	protected override void OnUnlock()
	{
		TaskState = State.Unlocked;
	}
}
