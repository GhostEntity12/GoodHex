using System.Linq;
using UnityEngine;

public class StandardTask : ProgressTask
{
	// Update is called once per frame
	void Update()
	{
		if (paused) return;
		switch (TaskState)
		{
			case State.Locked:
				if ((requiredTasks.Length == 0 || requiredTasks.All(t => t.IsComplete)) && !RequiresItem) // if all required tasks are complete
				{
					OnUnlock();
				}
				break;

			case State.Unlocked:

				// Set the number of displayed rats
				progressBar.SetRats(RatsInPlace);

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

	///<summary>
	/// Activates the task
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
		progressBar.SetActive(false);
		Highlight(false);
		TaskState = State.Complete;
		IsComplete = true;
		onCompleteEvents.ForEach(tm => tm.Trigger());

		GameManager.Instance.TaskManager.ClearRatsOnTask(this);
	}
}
