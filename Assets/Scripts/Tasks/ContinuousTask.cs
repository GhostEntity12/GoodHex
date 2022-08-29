using System.Linq;
using UnityEngine;

public class ContinuousTask : ProgressTask
{
	void Update()
	{
		if (paused) return;
		switch (TaskState)
		{
			case State.Locked:
				// If all required tasks are complete, unlock
				if ((requiredTasks.Length == 0 || requiredTasks.All(t => t.IsComplete)) && !RequiresItem) 
				{
					OnUnlock();
				}
				break;
			
			case State.Unlocked:
				// Set the number of displayed rats
				progressBar.SetRats(RatsInPlace); 

				// Decrease progress if not enough rats
				progress = Mathf.Clamp01(progress - (Time.deltaTime / taskDuration));
				progressBar.SetProgress(progress);

				// Set state to active if all slots are filled
				if (taskPoints.All(p => p.rat != null && p.rat.ArrivedAtTask))
				{
					OnActivate();
				}
				break;
			
			case State.Active:
				// If rats are missing, revert to unlocked state
				// Not using OnUnlock() because that is for the initial unlock
				if (!taskPoints.All(p => p.rat != null && p.rat.ArrivedAtTask))
				{
					TaskState = State.Unlocked;
					onDeactivateEvents.ForEach(tm => tm.Trigger());
				}

				// Increase progress
				progress = Mathf.Clamp01(progress + (Time.deltaTime / taskDuration));
				progressBar.SetProgress(progress);

				// If progress reaches 1, complete
				if (progress == 1)
				{
					OnComplete();
				}
				break;

			case State.Complete:
				break;
		}
	}

	/// <summary>
	/// Runs on activation of the task
	/// </summary>
	protected override void OnActivate()
	{
		TaskState = State.Active;
		onActivateEvents.ForEach(tm => tm.Trigger());
	}

	/// <summary>
	/// Runs on unlocking of the task
	/// </summary>
	protected override void OnUnlock()
	{
		progressBar.SetActive(true);
		TaskState = State.Unlocked;
		progress = 0;
		onUnlockEvents.ForEach(tm => tm.Trigger());
	}

	/// <summary>
	/// Runs on completion of the task
	/// </summary>
	protected override void OnComplete()
	{
		Highlight(false);
		TaskState = State.Unlocked;
		onCompleteEvents.ForEach(tm => tm.Trigger());

		GameManager.Instance.TaskManager.ClearRatsOnTask(this);
	}
}
