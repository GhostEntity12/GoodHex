using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    //[SerializeField] Transform position;
    Vector3[] spawnPoints;

    //private bool captured;
    private Rat rat = null;

    //private int selectedSpawn;
    Bounds bounds;

    private bool hazardActivated;

    float timer;

    bool sleeping;

    MeshRenderer renderer;

    [SerializeField]
    float sleepTimer;

    private float setTimer;

    [SerializeField]
    Material activeMaterial;

    [SerializeField]
    Material inactiveMaterial;

    void Start()
    {
        bounds = GetComponent<Collider>().bounds;

        //captured = false;
        spawnPoints =
            GameObject
                .FindGameObjectsWithTag("SpawnPoints")
                .Select(t => t.transform.position)
                .ToArray();
        timer = 3f;
        sleeping = true;
        setTimer = sleepTimer;
        renderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (sleeping)
        {
            renderer.material = inactiveMaterial;
            sleepTimer -= Time.deltaTime;
            if (sleepTimer <= 0)
            {
                sleeping = false;
                renderer.material = activeMaterial;
                sleepTimer = setTimer;
            }
        }
        else
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
                deadRats.Enqueue (rat);
            }
        }
        while (deadRats.Count >= 0)
        {
            deadRats.Dequeue().Kill();
            if (deadRats.Count <= 0)
            {
                sleeping = true;
            }
        }
    }
}
