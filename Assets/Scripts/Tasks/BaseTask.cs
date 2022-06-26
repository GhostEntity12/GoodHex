using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTask : MonoBehaviour
{
    public enum State { Locked, Unlocked, Complete }
    [SerializeField, Tooltip("Tasks that are required to be complete before this task triggers task")] protected NormalTask[] requiredTasks;

    protected abstract void OnUnlock();
    protected abstract void OnComplete();

    // Start is called before the first frame update
    protected void Start()
    {
        if (requiredTasks.Length == 0)
        {
            OnUnlock();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
