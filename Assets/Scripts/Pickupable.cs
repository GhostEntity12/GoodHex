using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : MonoBehaviour
{
    public string itemId;

    public string ReturnItemId()
    {
        return itemId;
        
    }
}