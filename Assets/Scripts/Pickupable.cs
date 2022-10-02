using UnityEngine;

public class Pickupable : Assignable
{
    [field: SerializeField]
    public string ItemId { get; private set; }

    public Sprite destinationSprite;

    [SerializeField]
    public GameObject indicatorSprite;

    protected new void Start()
    {
        base.Start();
        TaskState = State.Unlocked;
        ResetTaskPositions();
    }

    void Update()
    {
        if (taskPoints[0].rat && taskPoints[0].rat.ArrivedAtTask())
        {
            taskPoints[0].rat.Pickup(this);
            TaskState = State.Locked;
            GameManager.Instance.Highlighter.Highlight(r, false);
            if (indicatorSprite != null)
            {
                indicatorSprite.SetActive(true);
            }
        }
    }
}
