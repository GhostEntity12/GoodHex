using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TaskModuleScript : TaskModule
{
	public UnityEvent OnActivateEvent;
	public UnityEvent OnDeactivateEvent;

	protected override void OnActivate()
	{
		OnActivateEvent?.Invoke();
	}

	protected override void OnDeactivate()
	{
		OnDeactivateEvent?.Invoke();
	}

	protected override void SetPaused(bool pause) { }
}
