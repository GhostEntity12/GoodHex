using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimplifiedPB : MonoBehaviour
{
    public Image fill;
    
    public void SetProgress(float amount) => fill.fillAmount = amount;
    public void AddProgress(float amount) => fill.fillAmount += amount;
}
