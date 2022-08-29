using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hazard : MonoBehaviour
{
    [SerializeField] Transform position;
    private bool captured;
    private Rat rat;
    void Start()
    {
        captured = false;
    }
    void Update()
    {
        if (rat)
        {
            if (rat.NavAgent.remainingDistance < 0.1f)
            {
                rat.Kill(1f);
                rat = null;
            }
        } 
    }
    void OnTriggerEnter(Collider collider)
    {
        if (!rat && collider.TryGetComponent(out rat))
        {
            rat.NavAgent.SetDestination(position.position);
        }
    }
}
