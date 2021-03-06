using System.Collections.Generic;
using UnityEngine;

public class ProgressBarManager : MonoBehaviour
{
    Canvas progressCanvas;

    [SerializeField] ProgressBar progressBarPrefab;
    float canvasScaleCache;
    readonly List<ProgressBar> progressBars = new();

    public ProgressBar CreateProgressBar()
    {
        ProgressBar pb = Instantiate(progressBarPrefab, progressCanvas.transform);
        pb.CanvasScaleUpdate(canvasScaleCache);
        progressBars.Add(pb);
        return pb;
    }

    void Awake()
    {
        progressCanvas = GetComponent<Canvas>();
    }

    void Update()
    {
        // Update positions if the canvas scale has changed since the last frame
		if (canvasScaleCache != progressCanvas.transform.localScale.x)
		{
            canvasScaleCache = progressCanvas.transform.localScale.x;
			foreach (ProgressBar pb in progressBars)
			{
                pb.CanvasScaleUpdate(canvasScaleCache);
			}
		}
    }
}
