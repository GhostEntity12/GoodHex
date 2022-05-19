using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskCompletion : Task
{
	bool gameComplete = false;
	public override void OnActivate()
	{
		if (!gameComplete)
		{
			GameManager.Instance.AllTasksComplete();
			gameComplete = true;
		}
	}
}
