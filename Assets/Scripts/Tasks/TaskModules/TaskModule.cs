using UnityEngine;

public abstract class TaskModule : MonoBehaviour
{
	protected bool active = false;
	private void Awake()
	{
		GameManager.Pause += SetPaused;
	}

	public void Trigger()
	{
		active = !active;
		if (active) 
		{
			OnActivate();
		}
		else
		{
			OnDeactivate();
		}
	}
	protected abstract void OnActivate();
	protected abstract void OnDeactivate();

	protected abstract void SetPaused(bool pause);
	private void OnDestroy()
	{
		GameManager.Pause -= SetPaused;
	}
}
