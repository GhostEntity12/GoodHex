using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyTask : BaseTask
{
	public void SetState(State state) => TaskState = state;
	
	protected override void OnComplete() { }

	protected override void OnUnlock() { }
}
