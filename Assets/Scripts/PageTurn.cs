using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageTurn : MonoBehaviour
{
    [SerializeField] Animator[] anims;
    [SerializeField] GameObject[] objects;
    private int page;
    // Start is called before the first frame update
    void Start()
    {
        page = 0;
        objects[page].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void TurnPage()
    {
        anims[page].enabled = true;
        
    }
}
