using UnityEngine;
using UnityEngine.AI;

public class KillZone : MonoBehaviour
{
    //private GameObject rat;
    NavMeshAgent agentCat;

    void Start()
    {
        agentCat = transform.parent.GetComponent<NavMeshAgent>();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out Rat rat))
        {
            //rat = collider.gameObject;
            Debug.Log(agentCat);
            agentCat.isStopped = true;
            rat.Kill();
            Invoke("Resume", 0.5f);
        }
    }

    void Resume()
    {
        agentCat.isStopped = false;
    }
}
