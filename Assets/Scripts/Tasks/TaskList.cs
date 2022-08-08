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
				BaseTask.State.Locked => false,
				BaseTask.State.Unlocked => true,
				BaseTask.State.Active => true,
				BaseTask.State.Complete => false,
				_ => throw new System.Exception("Invalid Task State")
			};

			if (task.gameObject.activeInHierarchy != target)
			{
				task.gameObject.SetActive(target);
			}
		}
	}
}
