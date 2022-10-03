using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ComponentPauser : MonoBehaviour
{
	protected bool paused;
	protected virtual void Awake()
	{
		GameManager.Pause += Pause;
	}
	protected virtual void Pause(bool paused) => this.paused = paused;
}