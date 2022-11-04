using UnityEngine;
using UnityEngine.AI;

public class CatRandomisedNav : MonoBehaviour
{
    [SerializeField] Transform[] positions;
    private NavMeshAgent agent;
    private int destPoint = 0;
    private int currentDest = 0;
    private bool exceptionThrown = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //agent.autoBraking = false;
        destPoint = Random.Range(0, positions.Length);
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
        if (positions.Length == 0)
        {
            if (!exceptionThrown)
            {
                Debug.LogError("No Positions Implemented");
                exceptionThrown = true;
                return;
            }
            else
            {
                return;
            }
        }
        if (destPoint != currentDest)
        {
            currentDest = destPoint;
            agent.destination = positions[destPoint].position;
            destPoint = Random.Range(0, positions.Length);
        }
        else
        {
            currentDest = destPoint;
            destPoint = Random.Range(0, positions.Length);  
        }
    }

}
