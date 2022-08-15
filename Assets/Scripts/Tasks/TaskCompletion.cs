using System.Linq;

public class TaskCompletion : BaseTask
{
	bool gameComplete = false;


	protected override void OnUnlock()
	{
		if (!gameComplete)
		{
			GameManager.Instance.AllTasksComplete();
			gameComplete = true;
		}
	}
	protected override void OnComplete() { }

	private void Update()
	{
		if (requiredTasks.All(t => t.TaskState == State.Complete))
		{
			OnUnlock();
		}
	}
}
