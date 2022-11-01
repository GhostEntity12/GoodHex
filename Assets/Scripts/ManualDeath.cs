using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualDeath : MonoBehaviour
{
    List<Rat> DangerRats = new List<Rat>();

    public void KillRats() {
        foreach (Rat rat in DangerRats)
        {
            rat.Kill(0.5f);
        }

        DangerRats.Clear();
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
