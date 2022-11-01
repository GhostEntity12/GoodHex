using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class TimedHazard : MonoBehaviour
{
    float timer;
    bool MurderTime;
    //List<Rat> DangerRats;
    //List<Rat> DangerRats;

    // Start is called before the first frame update
    void Start()
    {
        MurderTime = false;
        //List<Rat> DangerRats = new List<Rat>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        Debug.Log(timer);

        if (timer > 10)
        {
            MurderTime = true;
            Debug.Log("MURDER TIME");
        }

        if (timer > 15)
        {
            MurderTime = false;
            timer = 0;
        }

        
    }

    void OnTriggerEnter(Collider collider)
    {
        //if (collider.TryGetComponent(out Rat rat))
        //{
        //DangerRats.Add(rat);
        //Debug.Log("Rat Added OVER");
        //}

        if (collider.TryGetComponent(out Rat rat))
        {
            if (MurderTime == true)
            {
                //foreach (Rat rat in DangerRats)
                //{
                rat.Kill(0.5f);
                //}

            }
        }
    }

    //void OnTriggerExit(Collider collider)
    //{
        //if (collider.TryGetComponent(out Rat rat))
        //{
            //DangerRats.Remove(rat);
            //Debug.Log("Rat Removed OVER");
        //}
    //}
}






