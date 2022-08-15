using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public Transform holdSpot;
    public LayerMask pickUpMask;

    private GameObject itemHolding;

    void Update()
    {
        CheckForPickup();
    }

    void CheckForPickup()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (itemHolding)
            {
                itemHolding.transform.position = transform.position;
                itemHolding.transform.parent = null;
                if (itemHolding.GetComponent<Rigidbody>())
                {
                    itemHolding.GetComponent<Rigidbody>().isKinematic = true;
                }
                itemHolding = null;
            }
            else
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
    }
}
