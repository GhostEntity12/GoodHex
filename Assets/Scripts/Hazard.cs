using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Hazard : MonoBehaviour
{
    [SerializeField] Transform position;
    Vector3[] spawnPoints;
    private bool captured;
    private Rat rat = null;
    private int selectedSpawn;
    void Start()
    {
        captured = false;
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoints").Select(t => t.transform.position).ToArray();
    }
    void Update()
    {
        if (rat)
        {
            if (rat.NavAgent.remainingDistance < 0.1f)
            {
                rat.Kill(1f);
                captured = true;
                rat = null;
            }
        }
        if (captured)
        {
            Invoke("RespawnRats", 2f);
            captured = false;
        }
    }
    void OnTriggerEnter(Collider collider)
    {
        if (!rat && collider.TryGetComponent(out rat))
        {
            rat.SetDestination(position.position);
        }
    }

    void RespawnRats()
    {
        selectedSpawn = Random.Range(0, spawnPoints.Length);
        GameManager.Instance.RatManager.SpawnRats(spawnPoints[selectedSpawn]);
    }
}
