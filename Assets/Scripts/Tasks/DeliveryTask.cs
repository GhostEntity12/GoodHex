using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DeliveryTask : Assignable
{
	[field: SerializeField] public string TriggerId { get; private set; }

	protected Collider col;

	public bool Locked => locked;
	protected bool locked;

	new SpriteRenderer renderer;

	[Space(20), SerializeField]
	protected UnityEvent onUnlockEvents;
	[Space(20), SerializeField]
	protected UnityEvent onActivateEvents;
	[Space(20), SerializeField]
	protected UnityEvent onDeactivateEvents;
	[Space(20), SerializeField]
	protected UnityEvent onCompleteEvents;

	protected new void Start()
	{
		col = GetComponent<Collider>();
		col.enabled = false;
		renderer = GetComponent<SpriteRenderer>();
		renderer.enabled = false;
		base.Start();
	}

	// Update is called once per frame
	protected void Update()
	{
		switch (TaskState)
		{
			case State.Locked:
				if (requiredTasks.Length > 0 && requiredTasks.All(t => t.IsComplete))
				{
					col.enabled = true;
					OnUnlock();
				}
				break;
			case State.Unlocked:
				foreach (TaskPoint taskPoint in TaskPoints)
				{
					if (
						taskPoint.rat &&
						taskPoint.rat.ArrivedAtTask() &&
						taskPoint.rat.IsHoldingItem &&
						taskPoint.rat.heldItem.ItemId == TriggerId
					)
					{
						Destroy(taskPoint.rat.heldItem.gameObject);
						OnComplete();
						break;
					}
				}
				break;
			case State.Active:
				break;
			case State.Complete:
				break;
			default:
				break;
		}
	}

	/// <summary>
	/// Runs on activation of the task
	/// </summary>
	protected override void OnActivate()
	{
		base.OnActivate();
		TaskState = State.Active;
		onActivateEvents?.Invoke();
	}

	///<summary>
	/// Activates the task
	/// </summary>
	protected override void OnUnlock()
	{
		base.OnUnlock();
		col.enabled = true;
		renderer.enabled = true;
		TaskState = State.Unlocked;
		onUnlockEvents?.Invoke();
	}

	/// <summary>
	/// Runs on completion of the task
	/// </summary>
	protected override void OnComplete()
	{
		base.OnComplete();
		renderer.enabled = false;
		Highlight(false);
		TaskState = State.Complete;
		IsComplete = true;
		onCompleteEvents?.Invoke();
		requiredTasks = new BaseTask[0];

		GameManager.Instance.TaskManager.ClearRatsOnTask(this);
	}
}
