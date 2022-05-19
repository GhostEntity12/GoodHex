using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClockSimplified : MonoBehaviour
{
    [SerializeField] float dayLength = 100;
    [SerializeField] TextMeshProUGUI timerText;
    // Update is called once per frame
    void Update()
    {
        if (dayLength > 0)
        {
            dayLength -= Time.deltaTime;
            timerText.text = $"{(int)dayLength}";
        }
        else
        {
            SceneManager.LoadScene("GameOver");
        }
    }
}
