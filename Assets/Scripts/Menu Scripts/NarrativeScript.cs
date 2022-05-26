using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NarrativeScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI narrative1;
    [SerializeField] float duration = 0.5f;
    [SerializeField] int delay = 5;
    private Color color;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        FadeIn(narrative1);
        StartCoroutine(Delay(delay));
        FadeOut(narrative1);
    }

    public void FadeOut(TextMeshProUGUI text) { 
        float currentTime = 0.0f;
        color = text.color;
        while (currentTime < duration) {
            color.a = Mathf.Lerp(1f, 0f, currentTime/duration);
            text.color = color;
            currentTime += Time.deltaTime;
        }
    }

    public void FadeIn(TextMeshProUGUI text) {
        float currentTime = 0.0f;
        while (currentTime < duration) {
            color.a = Mathf.Lerp(0f, 1f, currentTime/duration);
            text.color = color;
            currentTime += Time.deltaTime;
        }
    }

    private IEnumerator Delay(int delayAmount) {
        yield return new WaitForSeconds(delayAmount);
    }
}
