using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StandardTask : BaseTask
{
	[field: SerializeField] public Sprite TaskImage { get; private set; }

	[Header("Task Setup")]
	[SerializeField, Tooltip("How long the task takes")] float taskDuration;
	[Tooltip("How far into completion the task is")] float progress;
	[SerializeField, Tooltip("The points at which rats stand to do the task")] TaskPoint[] taskPoints = new TaskPoint[0];
	public TaskPoint[] TaskPoints => taskPoints;

	[Header("Progress Bar")]
	[SerializeField, Tooltip("How high above the task the progress bar should appear")] float progressBarOffset = 2f;
	/// <summary>
	/// The Progress bar for the task
	/// </summary>
	ProgressBar progressBar;

	[Header("Highlight")]
	[SerializeField] Sprite normalSprite;
	[SerializeField] Sprite highlightSprite;
	Renderer r;
	Color highlightColor = new(0.5f, 1f, 0.3f, 0.8f);
	Color highlightColorUnavailable = new(0.5f, 0.5f, 0.5f, 0.8f);

	[Space(20), SerializeField] List<TaskModule> onUnlockEvents;
	[Space(20), SerializeField] List<TaskModule> onActivateEvents;
	[Space(20), SerializeField] List<TaskModule> onDeactivateEvents;
	[Space(20), SerializeField] List<TaskModule> onCompleteEvents;

	int RatsInPlace => taskPoints.Where(p => p.rat != null && p.rat.ArrivedAtTask).Count();

	// Start is called before the first frame update
	new void Start()
	{
		r = GetComponent<Renderer>();

		progressBar = GameManager.Instance.CreateProgressBar();
		progressBar.Setup(this, taskPoints.Length, progressBarOffset);

		base.Start();
	}

	// Update is called once per frame
	void Update()
	{
		if (paused) return;
		switch (TaskState)
		{
			case State.Locked:
				if (requiredTasks.All(t => t.TaskState == State.Complete)) // if all required tasks are complete
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

	public void Hover(bool hovering)
	{
		Color c =
			TaskState == State.Unlocked && GameManager.Instance.RatManager.HasSelectedRats
				? highlightColor
				: highlightColorUnavailable;
		if (hovering)
		{
			switch (r)
			{
				case MeshRenderer m:
					m.material.color = c;
					break;
				case SpriteRenderer s:
					s.color = c;
					break;
				default:
					break;
			}
			GameManager.Instance.Reticle.SetTask(this);
		}
		else
		{
			switch (r)
			{
				case MeshRenderer m:
					m.material.color = new(0, 0, 0, 0);
					break;
				case SpriteRenderer s:
					s.color = new(0, 0, 0, 0);
					break;
				default:
					break;
			}
			GameManager.Instance.Reticle.SetTask(null);
		}
	}

	/// <summary>
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
}

[System.Serializable]
public class TaskPoint
{
	public Vector3 taskPosition;
	public Rat rat;
}
