using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskCompletion : BaseTask
{
	bool gameComplete = false;
	protected override void OnUnlock()
	{
		if (!gameComplete)
		{
			GameManager.Instance.AllTasksComplete();
			gameComplete = true;
		}
	}
	protected override void OnComplete() { }
}
