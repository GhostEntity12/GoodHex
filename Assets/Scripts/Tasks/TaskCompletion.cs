using System.Linq;
using UnityEngine;

public class TaskCompletion : BaseTask
{
	bool gameComplete = false;
	[SerializeField] TextAsset dialogue;

	protected override void OnUnlock()
	{
		if (!gameComplete)
		{
			if (dialogue)
			{
				GameManager.Instance.DialogueManager.QueueDialogue(dialogue, onEndAction: () =>
				{
					GameManager.Instance.AllTasksComplete();
					gameComplete = true;
				});
			}
			else
			{
				GameManager.Instance.AllTasksComplete();
				gameComplete = true;
			}
		}
	}

	protected override void OnActivate() { }
	protected override void OnComplete() { }

	private void Update()
	{
		if (requiredTasks.All(t => t.TaskState == State.Complete))
		{
			OnUnlock();
		}
	}
}
