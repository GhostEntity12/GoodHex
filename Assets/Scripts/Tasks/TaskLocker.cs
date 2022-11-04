using UnityEngine;

public class TaskLocker : MonoBehaviour
{
    [SerializeField] ProgressTask[] tasksToLock;

    // Update is called once per frame
    public void Lock(bool locked)
    {
		foreach (ProgressTask task in tasksToLock)
		{
            task.Lock(locked);
		}
    }
}
