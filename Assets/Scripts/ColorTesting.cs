using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ColorTesting : MonoBehaviour
{
    [SerializeField] Color target;
    [SerializeField] float speed = 0.1f;

    Renderer r;

    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        r.material.color = ExtensionMethods.ColorMoveTowards(r.material.color, target, speed);
    }
}
