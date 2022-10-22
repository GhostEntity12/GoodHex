using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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
	[SerializeField] bool showProgressBar = true;
	[SerializeField, Tooltip("How high above the task the progress bar should appear")]
	protected float progressBarOffset = 0.1f;
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
	[Space(20), SerializeField]
	protected UnityEvent onLockedUnlockEvents;

	protected int RatsInPlace => taskPoints.Where(p => p.rat != null && p.rat.ArrivedAtTask()).Count();
	[SerializeField] public GameObject taskSprite;

	protected Collider col;

	public bool Locked => locked;
	protected bool locked;

	protected new void Start()
	{
		col = GetComponent<Collider>();
		col.enabled = false;

		progressBar = GameManager.Instance.CreateProgressBar();
		progressBar.Setup(this, taskPoints.Length, progressBarOffset, showProgressBar);
		base.Start();
	}
	public void UpdateBar()
	{
		progressBar.UpdateTaskPos(this);
		progressBar.CanvasScaleUpdate(GameManager.Instance.ProgressBarManager.CanvasScaleCache);
	}

	public void Lock(bool locked)
	{
		this.locked = locked;
		GameManager.Instance.TaskManager.ClearRatsOnTask(this);
		progressBar.SetActive(!locked && (TaskState == State.Unlocked));
		progress = 0;

		if (!locked)
		{
			onLockedUnlockEvents?.Invoke();
		}
	}
}
