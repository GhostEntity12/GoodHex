using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image fill;
    public bool Complete => fill.fillAmount == 1;
    public TextMeshProUGUI number;

    public void SetProgress(float amount) => fill.fillAmount = amount;
    public void AddProgress(float amount) => fill.fillAmount += amount;
}
