using UnityEngine;
using UnityEngine.UI;

public class TaskListItem : MonoBehaviour
{
    [field: SerializeField] public StandardTask T { get; private set; }

	private void Start()
	{
		GetComponent<Image>().sprite = T.TaskImage;
	}
}
