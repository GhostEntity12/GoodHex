using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TaskModule : MonoBehaviour
{
	public abstract void OnActivate();
	public abstract void OnDeactivate();
}
