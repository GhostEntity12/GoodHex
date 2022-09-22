using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBob : MonoBehaviour
{
    [SerializeField] float bobHeight = 0.01f;
    [SerializeField] float bobTime = 1f;
    // Update is called once per frame
    void Update()
    {
        transform.localPosition = bobHeight * Vector3.up * Mathf.Sin(Time.time * bobTime);
    }
}
