using System.Collections.Generic;
using UnityEngine;

public class TaskLooper : MonoBehaviour
{
	[SerializeField] List<BaseTask> tasksToLoop;
	[SerializeField] bool relockFirstTask = false;
	// Update is called once per frame
	void Update()
	{
		if (tasksToLoop[^1].IsComplete)
		{
			foreach (BaseTask task in tasksToLoop)
			{
				task.SetState(BaseTask.State.Locked);
			}
			if (!relockFirstTask)
			{
				tasksToLoop[0].SetState(BaseTask.State.Unlocked);
			}
		}
	}
}
