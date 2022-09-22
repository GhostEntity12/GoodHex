using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
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

	protected Renderer r;

	[Space(20), SerializeField]
	protected UnityEvent onUnlockEvents;
	[Space(20), SerializeField]
	protected UnityEvent onActivateEvents;
	[Space(20), SerializeField]
	protected UnityEvent onDeactivateEvents;
	[Space(20), SerializeField]
	protected UnityEvent onCompleteEvents;

	protected int RatsInPlace => taskPoints.Where(p => p.rat != null && p.rat.ArrivedAtTask()).Count();
	[field: SerializeField] public bool RequiresItem { get; private set; }

	[field: SerializeField] public string TriggerId { get; private set; }

	protected Collider col;

	protected new void Start()
	{
		r = GetComponent<Renderer>();
		col = GetComponent<Collider>();
		col.enabled = false;

		progressBar = GameManager.Instance.CreateProgressBar();
		progressBar.Setup(this, taskPoints.Length, progressBarOffset);
		base.Start();
	}

	/// <summary>
	/// Highlights the task
	/// </summary>
	/// <param name="doHighlight"></param>
	public void Highlight(bool doHighlight)
	{
		GameManager.Instance.Reticle.SetTask(doHighlight ? this : null);

		if (TaskState == State.Unlocked && GameManager.Instance.RatManager.HasSelectedRats)
		{
			GameManager.Instance.Highlighter.Highlight(r, doHighlight);
		}
	}

	void OnTriggerEnter(Collider rat)
	{
		if (rat.GetComponent<PickUp>())
		{
			if (rat.GetComponentInChildren<Pickupable>())
			{
				if (rat.GetComponentInChildren<Pickupable>().ReturnItemId() == TriggerId)
				{
					RequiresItem = false;
					Destroy(GameObject.FindWithTag("Item"));
				}
			}
		}
	}
}
