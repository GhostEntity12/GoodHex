using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskList : MonoBehaviour
{
	TaskListItem[] tasks;

	private void Start()
	{
		tasks = GetComponentsInChildren<TaskListItem>();
	}


	// Update is called once per frame
	void Update()
	{
		foreach (TaskListItem task in tasks)
		{

			bool target = task.T.TaskState switch
			{
				Task.State.Locked => false,
				Task.State.Unlocked => true,
				Task.State.Complete => false,
				_ => throw new System.Exception("Invalid Task State")
			};

			if (task.gameObject.activeInHierarchy != target)
			{
				task.gameObject.SetActive(target);
			}
		}
	}
}
