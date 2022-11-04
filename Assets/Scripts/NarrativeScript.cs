using UnityEngine;

public class NarrativeScript : MonoBehaviour
{
    [SerializeField] CanvasGroup narrative;
    [SerializeField] float fadeSpeed;
    [SerializeField] CanvasGroup fade;
    bool showing = false;
    bool completed = false;
    float timer;
    readonly float fadeTime = 1.5f;

    // Update is called once per frame
    void Update()
    {
        if (!completed) {
            timer += Time.deltaTime;

            if (!showing) {
                narrative.alpha = Mathf.Lerp(0f, 1f, timer / fadeSpeed);
                if (narrative.alpha == 1) {
                    if (Input.GetMouseButtonDown(0)) {
                        showing = true;
                        timer = 0;
                    }
                }
            } else {
                narrative.alpha = Mathf.Lerp(1f, 0f, timer / fadeSpeed);
                if (narrative.alpha == 0) {
                    //showing = false;
                    timer = 0;
                    completed = true; 
                }
            }
        } else {
            timer += Time.deltaTime;
            fade.alpha = Mathf.Lerp(0f, 1f, timer / fadeTime);
            if (fade.alpha == 1) {
                LoadSceneManager.LoadScene("Controls");
            }
        }
    }
}
