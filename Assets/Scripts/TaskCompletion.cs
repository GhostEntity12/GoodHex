using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskCompletion : Task
{
	bool complete = false;
	public override void OnActivate()
	{
		if (!complete)
		{
			GameManager.Instance.AllTasksComplete();
			complete = true;
		}
	}
}
