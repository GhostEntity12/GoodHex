using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class ProgressTask : Assignable
{
	[field: SerializeField]
	public Sprite TaskImage { get; private set; }

	[Header("Task Setup")]
	[SerializeField, Tooltip("How long the task takes")]
	protected float taskDuration;
	[Tooltip("How far into completion the task is")]
	protected float progress;

	[Header("Progress Bar")]
	[SerializeField, Tooltip("How high above the task the progress bar should appear")]
	protected float progressBarOffset = 2f;
	/// <summary>
	/// The Progress bar for the task
	/// </summary>
	protected ProgressBar progressBar;

	[Space(20), SerializeField]
	protected UnityEvent onUnlockEvents;
	[Space(20), SerializeField]
	protected UnityEvent onActivateEvents;
	[Space(20), SerializeField]
	protected UnityEvent onDeactivateEvents;
	[Space(20), SerializeField]
	protected UnityEvent onCompleteEvents;

	protected int RatsInPlace => taskPoints.Where(p => p.rat != null && p.rat.ArrivedAtTask()).Count();
	[field: SerializeField] public bool RequiresItem { get; protected set; }
	[field: SerializeField] public string TriggerId { get; private set; }

	protected Collider col;

	protected new void Start()
	{
		col = GetComponent<Collider>();
		col.enabled = false;

		progressBar = GameManager.Instance.CreateProgressBar();
		progressBar.Setup(this, taskPoints.Length, progressBarOffset);
		base.Start();
	}

	public void AssignRatWithItem()
	{

	}

	//void OnTriggerEnter(Collider other)
	//{
	//	if (other.TryGetComponent(out Rat rat) && rat.IsHoldingItem && rat.heldItem.ItemId == TriggerId)
	//	{
	//		RequiresItem = false;
	//		Destroy(GameObject.FindWithTag("Item"));
	//	}
	//}
}
