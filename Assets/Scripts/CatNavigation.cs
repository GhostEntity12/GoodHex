using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CatNavigation : MonoBehaviour
{
    [SerializeField] Transform[] positions;
    private NavMeshAgent agent;
    private int destPoint = 0;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //agent.autoBraking = false;
        GotoNextPoint();
    }

    private void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GotoNextPoint();
        }
    }

    void GotoNextPoint()
    {
        if (positions.Length == 0) { return; }
        agent.destination = positions[destPoint].position;
        destPoint = (destPoint + 1) % positions.Length;
    }


}
