using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskUpdater : MonoBehaviour
{

	ProgressTask[] tasks;
	private void Start()
	{
		tasks = FindObjectsOfType<ProgressTask>();
	}

	public void UpdateBars()
	{
		foreach (ProgressTask task in tasks)
		{
			task.UpdateBar();
		}
	}
}
