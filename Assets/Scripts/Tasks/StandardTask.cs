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
				if (requiredTasks.Length == 0 || requiredTasks.All(t => t.IsComplete))
				{
					col.enabled = true;
					if (RequiresItem)
					{
						foreach (TaskPoint taskPoint in TaskPoints)
						{
							if (taskPoint.rat &&
								taskPoint.rat.ArrivedAtTask() &&
								taskPoint.rat.IsHoldingItem &&
								taskPoint.rat.heldItem.ItemId == TriggerId)
							{
								RequiresItem = false;
								Destroy(taskPoint.rat.heldItem.gameObject);
								GameManager.Instance.TaskManager.ClearRatsOnTask(this);
								break;
							}
						}
					}
					else
					{
						OnUnlock();
					}
					break;
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
					progress = 0;
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
		onActivateEvents?.Invoke();
	}

	///<summary>
	/// Activates the task
	/// </summary>
	protected override void OnUnlock()
	{
		GetComponent<Collider>().enabled = true;
		progressBar.SetActive(true);
		TaskState = State.Unlocked;
		progress = 0;
		onUnlockEvents?.Invoke();
	}

	/// <summary>
	/// Runs on completion of the task
	/// </summary>
	protected override void OnComplete()
	{
		Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);
		foreach (Collider collider in colliders)
		{
			Rat r = collider.gameObject.GetComponent<Rat>();
			if (r)
			{
				if (Random.value < 0.9f)
				{
					r.SetEmote(RatEmotes.Emotes.Celebrate);
				}
				else
				{
					r.SetEmote(RatEmotes.Emotes.Sleepy);
				}

			}
		}
		
		progressBar.SetActive(false);
		Highlight(false);
		TaskState = State.Complete;
		IsComplete = true;
		onCompleteEvents?.Invoke();

		GameManager.Instance.TaskManager.ClearRatsOnTask(this);
	}
}
