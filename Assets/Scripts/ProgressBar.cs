using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
	[SerializeField] Transform ratContainer;
	[SerializeField] GameObject ratSilhouettePrefab;
	[SerializeField] Sprite mouseUnfilled;
	[SerializeField] Sprite mouseFilled;
	[SerializeField] Image fill;
	List<Image> ratSilhouettes = new();
	public bool Complete => fill.fillAmount == 1;
	[SerializeField] TextMeshProUGUI number;
	public void SetProgress(float amount) => fill.fillAmount = amount;
	public void AddProgress(float amount) => fill.fillAmount += amount;
	Vector3 anchoredPos;
	float offsetTime;
	public void Setup(Task t, int ratCount, float offsetHeight)
	{
		for (int i = 0; i < ratCount; i++)
		{
			ratSilhouettes.Add(Instantiate(ratSilhouettePrefab, ratContainer).GetComponent<Image>());
		}
		SetProgress(0);
		gameObject.SetActive(false);
		anchoredPos = GameManager.Instance.mainCamera.WorldToScreenPoint(t.transform.position + Vector3.up * offsetHeight);
	}
	public void SetActive(bool active) => gameObject.SetActive(active);
	public void SetRats(int rats)
	{
		for (int i = 0; i < ratSilhouettes.Count; i++)
		{
			ratSilhouettes[i].sprite = i < rats ? mouseFilled : mouseUnfilled;
		}
	}

	private void Start()
	{
		offsetTime = Random.Range(0f, 3f);
	}
	private void Update()
	{
		transform.position = anchoredPos + 2f * Mathf.Sin(Time.time + offsetTime) * Vector3.up;
	}
}
