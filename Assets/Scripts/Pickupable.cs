using UnityEngine;
using UnityEngine.Events;

public class Pickupable : Assignable
{
    [field: SerializeField]
    public string ItemId { get; private set; }

    public Sprite destinationSprite;

    [SerializeField] GameObject shadow;
    [Space(20), SerializeField]
    protected UnityEvent onUnlockEvents;

    protected new void Start()
    {
        base.Start();
        TaskState = State.Unlocked;
        OnUnlock();
        onUnlockEvents?.Invoke();
        ResetTaskPositions();
    }

    void Update()
    {
        if (taskPoints[0].rat && taskPoints[0].rat.ArrivedAtTask())
        {
            taskPoints[0].rat.Pickup(this);
            TaskState = State.Complete;
            IsComplete = true;
            OnComplete();
            shadow.SetActive(false);
        }
    }
}
