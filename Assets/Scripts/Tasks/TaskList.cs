using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskList : MonoBehaviour
{
	[SerializeField] TaskListItem[] taskSlots;

	Dictionary<ProgressTask, TaskListItem> taskDict = new();
	Queue<ProgressTask> taskQueue = new();
	Queue<TaskListItem> tliQueue;

	[SerializeField] TweenedElement listObject;
	[SerializeField] TweenedElement revealObject;

	bool paused;

	private void Start()
	{
		listObject.SetCachesAndPosition(new(0, 1100));
		revealObject.SetCachesAndPosition(new(0, 300));
		revealObject.SlideElement(TweenedElement.ScreenState.Onscreen, tweenType: LeanTweenType.easeOutCubic);

		tliQueue = new(taskSlots);
		GameManager.Pause += Pause;
	}


	// Update is called once per frame
	void Update()
	{
		if (taskQueue.Count > 0 && tliQueue.Count > 0)
		{
			Register(taskQueue.Dequeue());
		}
	}

	public void Complete(ProgressTask task)
	{
		if (taskDict.ContainsKey(task))
		{
			taskDict[task].Depopulate();
		}
	}
	void Register(ProgressTask task)
	{
		if (task.TaskImage)
		{
			TaskListItem tli = tliQueue.Dequeue();
			tli.transform.SetAsLastSibling();
			taskDict.Add(task, tli);
			tli.gameObject.SetActive(true);
			tli.Populate(task);
		}
	}
	public void Deregister(TaskListItem tli)
	{
		taskDict.Remove(tli.Task);
		tliQueue.Enqueue(tli);
		tli.transform.SetAsFirstSibling();
		tli.gameObject.SetActive(false);
	}

	public void AddTaskToList(ProgressTask task)
	{
		if (tliQueue.Count == 0) taskQueue.Enqueue(task);
		else Register(task);
	}

	void Pause(bool paused) => this.paused = paused;


	[ContextMenu("In")]
	public void DropDown()
	{
		if (paused) return;
		revealObject.SlideElement(TweenedElement.ScreenState.Offscreen, () =>
			listObject.SlideElement(TweenedElement.ScreenState.Onscreen, null, LeanTweenType.easeOutBack, 0.4f),
			LeanTweenType.easeOutCubic, 0.2f);
	}

	[ContextMenu("Out")]
	public void RaiseUp()
	{
		if (paused) return;
		listObject.SlideElement(TweenedElement.ScreenState.Offscreen, () =>
			revealObject.SlideElement(TweenedElement.ScreenState.Onscreen, null, LeanTweenType.easeOutCubic, 0.2f),
		LeanTweenType.easeInBack, 0.4f);
	}
}
