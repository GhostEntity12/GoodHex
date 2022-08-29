using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskLooper : BaseTask
{
    [SerializeField] List<ProgressTask> tasksToLoop;

	protected override void OnActivate() { }

	protected override void OnComplete() { }

	protected override void OnUnlock()
	{
		foreach (ProgressTask task in tasksToLoop)
		{
			task.SetState(State.Locked);
		}
		tasksToLoop[0].SetState(State.Unlocked);
	}
}
