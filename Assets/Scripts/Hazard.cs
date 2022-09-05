using System.Collections.Generic;
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
    Bounds bounds;
    private bool hazardActivated;
    float timer;
    void Start()
    {
        bounds = GetComponent<Collider>().bounds;
        captured = false;
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoints").Select(t => t.transform.position).ToArray();
        timer = 3f;
    }
    void Update()
    {
        if (hazardActivated)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                HazardActivate();
                
            }
            // if (rat.NavAgent.remainingDistance < 0.1f)
            // {
            //     rat.Kill();
            //     captured = true;
            //     rat = null;
            // }
        }
    }
    void OnTriggerEnter(Collider collider)
    {
        if (!rat && collider.TryGetComponent(out rat))
        {
            //rat.SetDestination(position.position);
            hazardActivated = true;
        }
    }

    // void RespawnRats()
    // {
    //     selectedSpawn = Random.Range(0, spawnPoints.Length);
    //     GameManager.Instance.RatManager.SpawnRats(spawnPoints[selectedSpawn]);
    // }

    void HazardActivate()
    {
        Queue<Rat> deadRats = new Queue<Rat>();
        foreach (Rat rat in GameManager.Instance.RatManager.allRats)
        {
            if (bounds.Contains(rat.transform.position))
            {
                deadRats.Enqueue(rat);
            }
        }
        while (deadRats.Count > 0)
        {
            deadRats.Dequeue().Kill();
        }
    }
}
