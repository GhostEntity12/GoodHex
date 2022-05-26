using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Task : MonoBehaviour
{
	public enum State { Locked, Unlocked, Complete }
	[Header("Task Setup")]
	[SerializeField, Tooltip("Tasks that are required to be complete before this task triggers task")] Task[] requiredTasks;
	[SerializeField, Tooltip("The points at which rats stand to do the task")] TaskPoint[] taskPoints = new TaskPoint[0];
	[SerializeField, Tooltip("How long the task takes")] float taskDuration;
	[Tooltip("How far into completion the task is")] float progress;

	public State TaskState { get; private set; } = State.Locked;

	/// <summary>
	/// Returns a list of rats that are currently assigned to this task
	/// </summary>
	public List<Rat> AssignedRats => taskPoints.Select(tp => tp.AssignedRat).Where(r => r != null).ToList();
	/// <summary>
	/// Returns whether all the slots are filled
	/// </summary>
	float SlotsFilled => taskPoints.Where(p => p.AssignedRat != null).Count();
	/// <summary>
	/// Returns whether all the rats are in range of the task
	/// </summary>
	bool RatsInPlace => taskPoints.All(p => p.InRange);
	/// <summary>
	/// The Progress bar for the task
	/// </summary>
	ProgressBar progressBar;
	[SerializeField] float progressBarOffset = 2f;
	Renderer r;
	[SerializeField] Sprite normalSprite, highlightSprite;
	Color highlightColor = new(1, 0.9f, 0.5f, 0.8f);

	// Start is called before the first frame update
	void Start()
	{
		progressBar = Instantiate(GameManager.Instance.progressBarPrefab, GameManager.Instance.progressCanvas.transform).GetComponent<ProgressBar>();
		progressBar.Setup(this, taskPoints.Length, progressBarOffset);

		if (requiredTasks.Length == 0)
		{
			OnUnlock();
		}
		r = GetComponent<Renderer>();
	}

	// Update is called once per frame
	void Update()
	{
		switch (TaskState)
		{
			case State.Locked:
				if (requiredTasks.All(t => t.TaskState == State.Complete))
				{
					OnUnlock();
				}
				break;
			case State.Unlocked:
				foreach (TaskPoint point in taskPoints)
				{
					if (point.AssignedRat && point.InRange)
					{
						point.AssignedRat.InPlace();
					}
				}
				if (RatsInPlace && SlotsFilled == taskPoints.Length)
				{
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

	/// <summary>
	/// Activates the task
	/// </summary>
	public virtual void OnUnlock()
	{
		progressBar.SetActive(true);
		TaskState = State.Unlocked;
		progress = 0;
	}

	/// <summary>
	/// Runs on completion of the task
	/// </summary>
	public void OnComplete()
	{
		progressBar.SetActive(false);
		TaskState = State.Complete;

		// Unset Rats
		foreach (TaskPoint point in taskPoints)
		{
			point.Clear();
		}
	}

	public void Hover(bool hovering)
	{
		if (TaskState != State.Unlocked) return;
		if (hovering && RatManager.Instance.HasSelectedRats)
		{
			switch (r)
			{
				case MeshRenderer m:
					m.material.color = highlightColor;
					break;
				case SpriteRenderer s:
					s.sprite = highlightSprite;
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
					s.sprite = normalSprite;
					break;
				default:
					break;
			}
			GameManager.Instance.Reticle.SetTask(null);
		}
	}

	/// <summary>
	/// Takes a list of rats and assigns them to the task
	/// </summary>
	/// <param name="rats">The rats to assign to the task</param>
	/// <returns>Any rats that were not assigned</returns>
	public List<Rat> AssignRats(List<Rat> rats)
	{
		if (SlotsFilled == taskPoints.Length) return null;

		Queue<TaskPoint> availablePoints = new(taskPoints.Where(p => p.AssignedRat == null));
		// Might need some rework to make sure that rats cannot be assigned to the same task multiple times
		Queue<Rat> ratQueue = new(rats
			.Where(r => !taskPoints.Contains(r.Task)) // Eclude rats already on the task
			.OrderBy(r => Vector3.Distance(r.transform.position, transform.position)) // order by distance (ascending)
		);

		while (availablePoints.Count > 0 && ratQueue.Count > 0)
		{
			Rat r = ratQueue.Dequeue();
			TaskPoint p = availablePoints.Dequeue();
			r.SetTask(p);
			r.Deselect();
			p.SetRat(r);
		}
		return ratQueue.ToList();
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
			Gizmos.DrawSphere(point.taskPosition, 0.25f);
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(point.taskPosition, 0.25f);
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
	public Rat AssignedRat { get; private set; }
	public bool InRange => AssignedRat && Vector3.Distance(taskPosition, AssignedRat.transform.position) < 0.5f;

	public void SetRat(Rat r) { AssignedRat = r; }
	public void UnsetRat() => AssignedRat = null;

	public void Clear()
	{
		AssignedRat.UnsetTask();
		UnsetRat();
	}
}
