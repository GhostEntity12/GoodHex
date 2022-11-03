using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class TimedHazard : MonoBehaviour
{
    float timer;
    bool KillZoneActive;
    List<Rat> DangerRats = new List<Rat>();

    // Start is called before the first frame update
    void Start()
    {
        KillZoneActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        //Debug.Log(timer);

        if (timer > 10)
        {
            KillZoneActive = true;
            Debug.Log("Killzone Active");
        }

        if (timer > 15)
        {
            KillZoneActive = false;
            timer = 0;
        }

        if (KillZoneActive)
        {
            
            foreach (Rat rat in DangerRats)
            {
                rat.Kill(0.5f);
            }

            DangerRats.Clear();
        }

    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out Rat rat))
        {
            DangerRats.Add(rat);
            Debug.Log("Rat Added to DangerRats List");
        }

    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.TryGetComponent(out Rat rat))
        {
            DangerRats.Remove(rat);
            Debug.Log("Rat Removed from DangerRats List");
        }
    }
}






