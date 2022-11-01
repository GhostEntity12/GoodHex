using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class KillZone : MonoBehaviour
{
    Vector3[] spawnPoints;

    void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoints").Select(t => t.transform.position).ToArray();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out Rat rat))
        {
            rat.Kill(0.5f);
        }
    }
}
