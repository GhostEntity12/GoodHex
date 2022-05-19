using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
	[SerializeField] Image fill;
	public bool Complete => fill.fillAmount == 1;
	[SerializeField] TextMeshProUGUI number;
	public void SetProgress(float amount) => fill.fillAmount = amount;
	public void AddProgress(float amount) => fill.fillAmount += amount;

	public void Setup(Task t, int ratCount)
	{
		number.text = ratCount.ToString();
		SetProgress(0);
		gameObject.SetActive(false);
		transform.position = GameManager.Instance.mainCamera.WorldToScreenPoint(t.transform.position + Vector3.up * 2);
	}
	public void SetActive(bool active)
	{
		Debug.Log($"Change active state: {active}");
		gameObject.SetActive(active);
	}
}
