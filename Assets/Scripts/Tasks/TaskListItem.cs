using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskListItem : MonoBehaviour
{
    [field: SerializeField] public Task T { get; private set; }

	private void Start()
	{
		GetComponent<Image>().sprite = T.TaskImage;
	}
}
