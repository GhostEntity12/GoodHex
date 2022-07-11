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
	[SerializeField] TextMeshProUGUI number;

	readonly List<Image> ratSilhouettes = new();
	Vector3 anchoredPos;
	float offsetTime;

	Vector3 taskPos;
	float offset;
	float canvasScale;

	public bool Complete => fill.fillAmount == 1;
	public void SetProgress(float amount) => fill.fillAmount = amount;
	public void AddProgress(float amount) => fill.fillAmount += amount;
	public void SetActive(bool active) => gameObject.SetActive(active);

	public void Setup(StandardTask t, int ratCount, float offsetHeight)
	{
		for (int i = 0; i < ratCount; i++)
		{
			ratSilhouettes.Add(Instantiate(ratSilhouettePrefab, ratContainer).GetComponent<Image>());
		}
		SetProgress(0);
		gameObject.SetActive(false);

		taskPos = t.transform.position;
		offset = offsetHeight;

		offsetTime = Random.Range(0f, 3f);
	}

	public void SetRats(int rats)
	{
		for (int i = 0; i < ratSilhouettes.Count; i++)
		{
			ratSilhouettes[i].sprite = i < rats ? mouseFilled : mouseUnfilled;
		}
	}

	private void Update()
	{
		transform.position = anchoredPos + 4f * canvasScale * Mathf.Sin(Time.time + offsetTime) * Vector3.up;
	}

	public void CanvasScaleUpdate(float scale)
	{
		canvasScale = scale;
		anchoredPos = GameManager.Instance.mainCamera.WorldToScreenPoint(taskPos + Vector3.up * offset);
	}
}
