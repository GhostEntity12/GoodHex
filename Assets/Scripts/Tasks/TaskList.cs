using UnityEngine;

public class TaskList : MonoBehaviour
{
	[SerializeField] TweenedElement listObject;
	[SerializeField] TweenedElement revealObject;
	TaskListItem[] tasks;

	private void Start()
	{
		tasks = GetComponentsInChildren<TaskListItem>();
		listObject.SetCachesAndPosition(new(0, 1100));
		revealObject.SetCachesAndPosition(new(0, 300));
		revealObject.SlideElement(TweenedElement.ScreenState.Onscreen, tweenType: LeanTweenType.easeOutCubic);
	}


	// Update is called once per frame
	void Update()
	{
		foreach (TaskListItem task in tasks)
		{
			bool target = task.T.TaskState switch
			{
				BaseTask.State.Locked => false,
				BaseTask.State.Unlocked => true,
				BaseTask.State.Active => true,
				BaseTask.State.Complete => false,
				_ => throw new System.Exception("Invalid Task State")
			};

			if (task.gameObject.activeInHierarchy != target)
			{
				task.gameObject.SetActive(target);
			}
		}
	}
	[ContextMenu("In")]
	public void DropDown()
	{
		revealObject.SlideElement(TweenedElement.ScreenState.Offscreen, () =>
			listObject.SlideElement(TweenedElement.ScreenState.Onscreen, null, LeanTweenType.easeOutBack, 0.4f),
			LeanTweenType.easeOutCubic, 0.2f);
	}

	[ContextMenu("Out")]
	public void RaiseUp()
	{
		listObject.SlideElement(TweenedElement.ScreenState.Offscreen, () =>
			revealObject.SlideElement(TweenedElement.ScreenState.Onscreen, null, LeanTweenType.easeOutCubic, 0.2f),
		LeanTweenType.easeInBack, 0.4f);
	}
}
