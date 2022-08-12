using System.Linq;
using UnityEngine;

public class RecurringTask : ProgressTask
{
	[SerializeField] float timeToReunlock = 5f;
	float reunlockTimer;
	bool prerequisitesMet = false;

	new void Start()
	{
		reunlockTimer = timeToReunlock;
		base.Start();
	}

	void Update()
	{
		if (paused) return;
		switch (TaskState)
		{
			case State.Locked:
				if (prerequisitesMet)
				{
					reunlockTimer -= Time.deltaTime;
					if (reunlockTimer <= 0)
					{
						OnUnlock();
						reunlockTimer = timeToReunlock;
						onUnlockEvents.ForEach(tm => tm.Trigger());
					}
				}
				else
				{
					if (requiredTasks.All(t => t.TaskState == State.Complete)) // if all required tasks are complete
					{
						prerequisitesMet = true;
						OnUnlock();
						onUnlockEvents.ForEach(tm => tm.Trigger());
					}
				}
				break;
			case State.Unlocked:
				progressBar.SetRats(RatsInPlace);
				if (taskPoints.All(p => p.rat != null && p.rat.ArrivedAtTask))
				{
					TaskState = State.Active;
					onActivateEvents.ForEach(tm => tm.Trigger());
				}
				break;
			case State.Active:
				if (!taskPoints.All(p => p.rat != null && p.rat.ArrivedAtTask))
				{
					TaskState = State.Unlocked;
					onDeactivateEvents.ForEach(tm => tm.Trigger());
				}
				progress += Time.deltaTime / taskDuration;
				progressBar.SetProgress(progress);
				if (progress >= 1)
				{
					OnComplete();
					onCompleteEvents.ForEach(tm => tm.Trigger());
				}
				break;
			case State.Complete:
				break;
		}
	}

	///<summary>
	/// Activates the task
	/// </summary>
	protected override void OnUnlock()
	{
		progressBar.SetActive(true);
		TaskState = State.Unlocked;
		progress = 0;
	}

	/// <summary>
	/// Runs on completion of the task
	/// </summary>
	protected override void OnComplete()
	{
		progressBar.SetActive(false);
		Hover(false);
		TaskState = State.Locked;

		GameManager.Instance.TaskManager.ClearRatsOnTask(this);
	}
}
