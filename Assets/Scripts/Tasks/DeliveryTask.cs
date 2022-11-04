using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DeliveryTask : Assignable
{
	public List<DeliveryOption> DeliveryOptions;

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
						ValidDelivery(taskPoint.rat.heldItem.ItemId)
					)
					{
						foreach (var deliveryOption in DeliveryOptions)
						{
							if (taskPoint.rat.heldItem.ItemId == deliveryOption.ID)
							{
								deliveryOption.Trigger();
								break;
							}
						}
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
		col.enabled = false;
		Highlight(false);
		TaskState = State.Complete;
		IsComplete = true;
		onCompleteEvents?.Invoke();
		requiredTasks = new BaseTask[0];

		GameManager.Instance.TaskManager.ClearRatsOnTask(this);
	}

	public bool ValidDelivery(string id)
	{
		foreach (DeliveryOption option in DeliveryOptions)
		{
			if (id == option.ID)
			{
				return true;
			}
		}
		return false;
	}
}
[System.Serializable]
public class DeliveryOption
{
	[field: SerializeField] public string ID { get; private set; }
	[SerializeField] DummyTask dummyTask;

	public void Trigger()
	{
		dummyTask.SetState(BaseTask.State.Complete);
	}
}