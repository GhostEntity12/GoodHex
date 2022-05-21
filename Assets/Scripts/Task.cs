using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Task : MonoBehaviour
{
	[Header("Task Setup")]
	[SerializeField, Tooltip("Tasks that are required to be complete before this task triggers task")] Task[] requiredTasks;
	[SerializeField, Tooltip("The points at which rats stand to do the task")] TaskPoint[] taskPoints = new TaskPoint[0];
	[SerializeField, Tooltip("How long the task takes")] float taskDuration;
	/// <summary>
	/// Whether the task is currently available for completion
	/// </summary>
	public bool Available { get; private set; } = false;
	[Tooltip("How far into completion the task is")] float progress;
	[Tooltip("Whether the task is completed")] bool complete = false;

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

	// Start is called before the first frame update
	void Start()
	{
		progressBar = Instantiate(GameManager.Instance.progressBarPrefab, GameManager.Instance.progressCanvas.transform).GetComponent<ProgressBar>();
		progressBar.Setup(this, taskPoints.Length);

		if (requiredTasks.Length == 0)
		{
			OnActivate();
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (Available && !complete)
		{
			foreach (TaskPoint point in taskPoints)
			{
				if (point.AssignedRat && point.InRange)
				{
					point.AssignedRat.InPlace();
				}
			}
			if (RatsInPlace && SlotsFilled == taskPoints.Length) // Rat conditions
			{
				progress += Time.deltaTime / taskDuration;
				progressBar.SetProgress(progress);
				if (progress >= 1)
				{
					OnComplete();
				}
			}
		}
		else if (!Available && !complete)
		{
			if (requiredTasks.All(t => t.complete))
			{
				OnActivate();
			}
		}
	}

	/// <summary>
	/// Activates the task
	/// </summary>
	public virtual void OnActivate()
	{
		progressBar.SetActive(true);
		Available = true;
		progress = 0;
	}

	/// <summary>
	/// Runs on completion of the task
	/// </summary>
	public void OnComplete()
	{
		progressBar.SetActive(false);
		Debug.Log(progressBar.gameObject, progressBar.gameObject);
		Available = false;
		complete = true;

		// Unset Rats
		foreach (TaskPoint point in taskPoints)
		{
			point.AssignedRat.UnsetTask();
			point.UnsetRat();
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
		SphereCollider c = GetComponent<SphereCollider>();
		if (c)
		{
			Gizmos.color = new Color(0.1f, 0.8f, 0.3f, 0.75f);
			Gizmos.DrawSphere(transform.position, c.radius);
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(transform.position, c.radius);
		}

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
}
