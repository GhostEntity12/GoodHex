using System.Collections.Generic;
using UnityEngine;

public class TaskLooper : MonoBehaviour
{
	[SerializeField] List<ProgressTask> tasksToLoop;

	// Update is called once per frame
	void Update()
	{
		if (tasksToLoop[^1].IsComplete)
		{
			foreach (ProgressTask task in tasksToLoop)
			{
				task.SetState(BaseTask.State.Locked);
			}
			tasksToLoop[0].SetState(BaseTask.State.Unlocked);
		}
	}
}
