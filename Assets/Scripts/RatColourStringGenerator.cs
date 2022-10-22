using System.Text;
using UnityEngine;

public class RatColourStringGenerator : MonoBehaviour
{
	string s = @"30,31,35
33,40,52
31,80,154
86,81,155
107,69,106
41,78,152
33,40,52
74,110,176
87,46,58
87,46,58
109,146,201
67,22,34
30,28,29
43,61,99 ";
	[ContextMenu("Generate")]
	void Generate()
	{
		StringBuilder sb = new();
		string[] sArr = s.Split('\n');
		for (int i = 0; i < sArr.Length; i++)
		{
			sb.Append($"new Color32({sArr[i].Trim()},255),");
		}
		Debug.Log(sb.ToString());
	}
	[ContextMenu("Test")]
	void Test()
	{
		int[] numbers = new int[GameManager.ratColors.GetLength(1)];
		System.Random rand = new();
		for (int i = 0; i < 10000; i++)
		{
			float val = (float)rand.NextDouble();
			numbers[Mathf.FloorToInt(val / (0.99f / (GameManager.ratColors.GetLength(1) - 1)))]++;
		}
		string s = "";
		for (int i = 0; i < 15; i++)
		{
			s += numbers[i] + ", ";
		}
		Debug.Log(s);
	}
}
