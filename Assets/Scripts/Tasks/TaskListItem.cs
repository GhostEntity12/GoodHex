using UnityEngine;
using UnityEngine.UI;

public class TaskListItem : MonoBehaviour
{
	public ProgressTask Task { get; private set; }

	[SerializeField] TaskList list;
	Image image;
	
	private void Start()
	{
		image = GetComponent<Image>();
		(transform as RectTransform).sizeDelta = Vector3.one * -25;
		gameObject.SetActive(false);
	}

	public void Populate(ProgressTask task)
	{
		Task = task;
		image.sprite = task.TaskImage;
		LeanTween.size(transform as RectTransform, Vector3.one * 120, 0.2f).setEaseOutBack();
	}

	public void Depopulate()
	{
		LeanTween.size(transform as RectTransform, Vector3.one * -25, 0.2f).setEaseInBack().setOnComplete(() => {
			image.sprite = null;
			list.Deregister(this);
			});
	}
}
