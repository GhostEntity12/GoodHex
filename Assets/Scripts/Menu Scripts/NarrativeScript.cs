using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NarrativeScript : MonoBehaviour
{
    [SerializeField] CanvasGroup narrative;
    [SerializeField] float fadeSpeed;
    [SerializeField] float delayAmount;
    [SerializeField] GameObject button;
    float delayTime;
    bool showing = false;
    bool completed = false;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        button.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!completed) {
            timer += Time.deltaTime;

            if (!showing) {
                narrative.alpha = Mathf.Lerp(0f, 1f, timer / fadeSpeed);
                if (narrative.alpha == 1) {
                    delayTime += Time.deltaTime;
                    if (delayTime > delayAmount || Input.GetMouseButtonDown(0)) {
                        showing = true;
                        timer = 0;
                        delayTime = 0;
                    }
                }
            } else {
                narrative.alpha = Mathf.Lerp(1f, 0f, timer / fadeSpeed);
                if (narrative.alpha == 0) {
                    //showing = false;
                    timer = 0;
                    completed = true;
                    button.SetActive(true);
                }
            }
        }
    }
}
