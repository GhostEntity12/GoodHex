using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskCompletion : Task
{
	public override void OnActivate() => GameManager.Instance.AllTasksComplete();
}
