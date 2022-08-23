using UnityEngine;

public class PickUp : MonoBehaviour
{
    public Transform holdSpot;
    public LayerMask pickUpMask;

    public GameObject itemHolding;
    public bool IsHoldingItem => itemHolding;

    public void CheckForPickup()
    {
        Collider[] pickUpItems = Physics.OverlapSphere(transform.position, .2f, pickUpMask);
        foreach (var pickUpItem in pickUpItems)
        {
            itemHolding = pickUpItem.gameObject;
            itemHolding.transform.position = holdSpot.position;
            itemHolding.transform.parent = transform;
            if (itemHolding.GetComponent<Rigidbody>())
            {
                itemHolding.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }
}
