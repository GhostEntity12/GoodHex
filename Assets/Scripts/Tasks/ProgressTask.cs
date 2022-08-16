using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ProgressTask : BaseTask
{
	[field: SerializeField]
	public Sprite TaskImage { get; private set; }

	[Header("Task Setup")]
	[SerializeField, Tooltip("How long the task takes")]
	protected float taskDuration;
	[Tooltip("How far into completion the task is")]
	protected float progress;
	[SerializeField, Tooltip("The points at which rats stand to do the task")]
	protected TaskPoint[] taskPoints = new TaskPoint[0];

	[Header("Progress Bar")]
	[SerializeField, Tooltip("How high above the task the progress bar should appear")]
	protected float progressBarOffset = 2f;
	/// <summary>
	/// The Progress bar for the task
	/// </summary>
	protected ProgressBar progressBar;

	[Header("Highlight")]
	[SerializeField]
	protected Sprite normalSprite;
	[SerializeField]
	protected Sprite highlightSprite;
	protected Renderer r;
	protected Color highlightColor = new(0.5f, 1f, 0.3f, 0.8f);
	protected Color highlightColorUnavailable = new(0.5f, 0.5f, 0.5f, 0.8f);

	[Space(20), SerializeField]
	protected List<TaskModule> onUnlockEvents;
	[Space(20), SerializeField]
	protected List<TaskModule> onActivateEvents;
	[Space(20), SerializeField]
	protected List<TaskModule> onDeactivateEvents;
	[Space(20), SerializeField]
	protected List<TaskModule> onCompleteEvents;

	protected int RatsInPlace => taskPoints.Where(p => p.rat != null && p.rat.ArrivedAtTask).Count();
	public TaskPoint[] TaskPoints => taskPoints;
	[field: SerializeField] public bool RequiresItem { get; private set; }

	[SerializeField] string triggerId;

	new void Start()
	{

		r = GetComponent<Renderer>();

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
		Color c =
			TaskState == State.Unlocked && GameManager.Instance.RatManager.HasSelectedRats
				? highlightColor
				: highlightColorUnavailable;

		if (doHighlight)
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

	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Rat")
		{
			if (GameObject.FindWithTag("Item") != null)
			{
				if (GameObject.FindWithTag("Item").GetComponent<Pickupable>().ReturnItemId() == triggerId)
				{
					RequiresItem = false;
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
