using System.Linq;
using UnityEngine;

public class RecurringTask : ProgressTask
{
	[SerializeField] float timeToReunlock = 5f;
	float reunlockTimer;
	bool prerequisitesMet = false;

	protected new void Start()
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
				// Prerequisites check is to trigger unlock for the first time
				if (!prerequisitesMet)
				{
					// If all required tasks are complete, unlock
					if ((requiredTasks.Length == 0 || requiredTasks.All(t => t.IsComplete)) && !RequiresItem) 
					{
						prerequisitesMet = true;
						OnUnlock();
					}
				}
				// Unlock if the timer reaches 0
				else
				{
					reunlockTimer -= Time.deltaTime;
					if (reunlockTimer <= 0)
					{
						OnUnlock();
						reunlockTimer = timeToReunlock;
					}
				}
				break;

			case State.Unlocked:
				// Set the number of displayed rats
				progressBar.SetRats(RatsInPlace);

				// Set state to active if all slots are filled
				if (taskPoints.All(p => p.rat != null && p.rat.ArrivedAtTask()))
				{
					OnActivate();
				}
				break;

			case State.Active:
				// If rats are missing, revert to unlocked state
				// Not using OnUnlock() because that is for the initial unlock
				if (!taskPoints.All(p => p.rat != null && p.rat.ArrivedAtTask()))
				{
					TaskState = State.Unlocked;
					onDeactivateEvents?.Invoke();
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
	/// Allows quick unlocking of the task if locked
	/// </summary>
	public void Unlock()
	{
		if (TaskState != State.Locked) return;
		reunlockTimer = 0;
	}

	/// <summary>
	/// Runs on activation of the task
	/// </summary>
	protected override void OnActivate()
	{
		TaskState = State.Active;
		onActivateEvents?.Invoke();
	}

	/// <summary>
	/// Runs on unlocking of the task
	/// </summary>
	protected override void OnUnlock()
	{
		GetComponent<Collider>().enabled = true;
		progressBar.SetActive(true);
		TaskState = State.Unlocked;
		progress = 0;
		IsComplete = false;
		onUnlockEvents?.Invoke();
	}

	/// <summary>
	/// Runs on completion of the task
	/// </summary>
	protected override void OnComplete()
	{
		progressBar.SetActive(false);
		Highlight(false);
		TaskState = State.Locked;
		IsComplete = true;
		onCompleteEvents?.Invoke();

		GameManager.Instance.TaskManager.ClearRatsOnTask(this);
	}
}
