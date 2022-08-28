using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hazard : MonoBehaviour
{
    //private NavMeshAgent ratAgent;
    void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out Rat rat))
        {
            //ratAgent = rat.GetComponent<NavMeshAgent>();
            //ratAgent.destination = gameObject.transform.position;
            //if (ratAgent.transform.position == gameObject.transform.position)
            rat.Kill(1f);
            //play animation
            //deactivate for few seconds
        }
    }
}
