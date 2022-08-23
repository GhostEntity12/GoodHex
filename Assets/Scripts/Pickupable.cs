using UnityEngine;

public class Pickupable : MonoBehaviour
{
    public string itemId;

    private Rat savedRat;

    public string ReturnItemId()
    {
        return itemId;
    }

    public void AssignRat(Rat rat)
    {
        savedRat = rat;
    }

    void Update()
    {
        if (savedRat && Vector3.Distance(savedRat.transform.position, transform.position) < 0.5f)
        {
            savedRat.GetComponent<PickUp>().CheckForPickup();
        }
    }
}