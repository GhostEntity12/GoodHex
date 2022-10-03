using UnityEngine;

public class Pickupable : Assignable
{
    [field: SerializeField]
    public string ItemId { get; private set; }

    public Sprite destinationSprite;

    [field: SerializeField]
    public string SpriteName { get; private set; }

    private GameObject indicatorSprite;

    protected new void Start()
    {
        base.Start();
        TaskState = State.Unlocked;
        ResetTaskPositions();
        if (SpriteName != null)
        {
            indicatorSprite = GameObject.Find(SpriteName);
        }
    }

    void Update()
    {
        if (taskPoints[0].rat && taskPoints[0].rat.ArrivedAtTask())
        {
            taskPoints[0].rat.Pickup(this);
            TaskState = State.Locked;
            if (indicatorSprite != null)
            {
                indicatorSprite.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }
}
