using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatColorDemo : MonoBehaviour
{
    [SerializeField] Rat RatPrefab;
    [SerializeField] Transform[] points;
    [ContextMenu("Spawn")]
    void Spawn()
    {
		for (int i = 0; i < GameManager.ratColors.GetLength(1); i++)
		{
            RatData rd = new();
            rd.lightColor = GameManager.ratColors[0, i];
            rd.darkColor = GameManager.ratColors[1, i];
            Rat rat = Instantiate(RatPrefab, points[i]).GetComponent<Rat>();
            rat.AssignInfo(rd);
            rat.SetColor();
            rat.NavAgent.enabled = false;   
		}
    }
}
