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

	[Space(20), SerializeField] List<TaskModule> taskModules;

	int RatsInPlace => ratSlots.Where(p => p.Value != null && Vector3.Distance(p.Key.taskPosition, p.Value.transform.position) < 0.1f).Count();
	public Dictionary<TaskPoint, Rat> ratSlots = new();

	// Start is called before the first frame update
	new void Start()
	{
		r = GetComponent<Renderer>();

		progressBar = GameManager.Instance.CreateProgressBar();
		progressBar.Setup(this, taskPoints.Length, progressBarOffset);

		foreach (TaskPoint taskPoint in taskPoints)
		{
			ratSlots.Add(taskPoint, null);
		}
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
				}
				break;
			case State.Unlocked:
				foreach (TaskPoint point in taskPoints)
				{
					if (ratSlots[point] != null && // if no assigned rat
						GameManager.Instance.TaskManager.RatInPlace(ratSlots[point]))
					{
						ratSlots[point].ReachedTaskPoint();
					}
				}
				progressBar.SetRats(RatsInPlace);
				if (ratSlots.All(s => s.Value && s.Value.atTask) && RatsInPlace == taskPoints.Length)
				{
					taskModules.ForEach(tm => tm.OnActivate());
					progress += Time.deltaTime / taskDuration;
					progressBar.SetProgress(progress);
					if (progress >= 1)
					{
						OnComplete();
					}
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
		taskModules.ForEach(tm => tm.OnDeactivate());

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
}
