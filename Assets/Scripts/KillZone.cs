using UnityEngine;
using UnityEngine.AI;

public class KillZone : MonoBehaviour
{
    private GameObject rat;
    NavMeshAgent agentCat;

    void Start()
    {
        agentCat = transform.parent.GetComponent<NavMeshAgent>();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Rat") // collider.TryGetComponent(out Rat rat))
        {
            rat = collider.gameObject;
            Debug.Log(agentCat);
            agentCat.isStopped = true;
            Invoke("Kill", 2f);
            //rat.Kill();
        }
    }

    void Kill()
    {
        Destroy(rat);
        agentCat.isStopped = false;
    }
}
