using UnityEngine;

public abstract class BaseTask : MonoBehaviour
{
	public enum State { Locked, Unlocked, Active, Complete }
	[SerializeField, Tooltip("Tasks that are required to be complete before this task triggers task")] protected BaseTask[] requiredTasks;

	public State TaskState { get; protected set; } = State.Locked;
	public bool IsComplete { get; protected set; }

	protected bool paused = false;

	protected abstract void OnUnlock();
	protected abstract void OnActivate();
	protected abstract void OnComplete();

	// Start is called before the first frame update
	protected void Start()
	{
		GameManager.Pause += SetPaused;
		if (requiredTasks.Length == 0)
		{
			OnUnlock();
		}
	}

	void SetPaused(bool paused) => this.paused = paused;
	private void OnDestroy()
	{
		GameManager.Pause -= SetPaused;
	}

	public void SetState(State state) => TaskState = state;
}
