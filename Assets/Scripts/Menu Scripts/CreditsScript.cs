using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsScript : MonoBehaviour
{
    [SerializeField] GameObject button;
    [SerializeField] int time;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DelayButton());
    }

    IEnumerator DelayButton() {
        yield return new WaitForSeconds(time);
        button.SetActive(true);
    }
}
