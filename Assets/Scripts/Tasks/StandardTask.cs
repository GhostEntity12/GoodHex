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
				if (requiredTasks.All(t => t.TaskState == State.Complete) && requiresItem == false) // if all required tasks are complete
				{
					OnUnlock();
					onUnlockEvents.ForEach(tm => tm.Trigger());
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
		TaskState = State.Complete;

		SpawnItem();

		GameManager.Instance.TaskManager.ClearRatsOnTask(this);
	}

	/// <summary>
	/// Draws TaskPoint positions
	/// </summary>
	private void OnDrawGizmos()
	{
		if (taskPoints.Length == 0) return;

		foreach (TaskPoint point in taskPoints)
		{
			Gizmos.color = new Color(0.5f, 0.3f, 0f, 0.75f);
			Gizmos.DrawSphere(point.taskPosition, 0.05f);
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(point.taskPosition, 0.05f);
		}
	}

	[ContextMenu("Reset Task Positions")]
	void ResetTaskPositions()
	{
		foreach (TaskPoint point in taskPoints)
		{
			point.taskPosition = transform.position;
		}
	}

	void SpawnItem()
	{
		Instantiate(itemToSpawn, spawner.transform.position, Quaternion.identity);
	}

	void OnTriggerEnter(Collider collider)
    {
		if(collider.gameObject.tag == "Rat")
        {
			if (GameObject.FindWithTag("Item") != null)
			{
				if (GameObject.FindWithTag("Item").GetComponent<Pickupable>().ReturnItemId() == triggerId)
				{
					requiresItem = false;
					Destroy(GameObject.FindWithTag("Item"));
				}
			}
		}
    }
}

[System.Serializable]
public class TaskPoint
{
	public Vector3 taskPosition;
	public Rat rat;
}
