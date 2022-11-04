using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
	public static Color ColorMoveTowards(Color current, Color target, float maxDistanceDelta)
	{
		if (current == target) return current;

		Color.RGBToHSV(current, out float currentH, out float currentS, out float currentV);
		Color.RGBToHSV(target, out float targetH, out float targetS, out float targetV);

		if (Mathf.Abs(currentH - targetH) > 0.5) // Looping H value
		{
			currentH += 1;
		}

		Vector3 v3Current = new (currentH, currentS, currentV);
		Vector3 v3Target = new (targetH, targetS, targetV);
		Vector3 v3Color = Vector3.MoveTowards(v3Current, v3Target, maxDistanceDelta);

		Color color = Color.HSVToRGB(targetS == 0 ? currentH : (v3Color.x % 1), v3Color.y, v3Color.z);
		//color.a = alpha;

		//float hDifference = Mathf.Abs(currentH - targetH);
		//float sDifference = Mathf.Abs(currentS - targetS);
		//float vDifference = Mathf.Abs(currentV - targetV);

		//float largestDifference = Mathf.Min(Mathf.Min(hDifference, sDifference), vDifference);

		//float h = Mathf.MoveTowards(currentH, targetH, maxDistanceDelta * hDifference / largestDifference);
		//float s = Mathf.MoveTowards(currentS, targetS, maxDistanceDelta * sDifference / largestDifference);
		//float v = Mathf.MoveTowards(currentV, targetV, maxDistanceDelta * vDifference / largestDifference);

		//Color color = Color.HSVToRGB(
		//	targetS == 0 ? currentH : h % 1, // handling to 
		//	s, 
		//	v);

		return color;
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = Random.Range(0, n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
}
