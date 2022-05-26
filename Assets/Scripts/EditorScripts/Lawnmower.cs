using System.Collections.Generic;
using UnityEngine;

namespace Ghost
{

	public class Lawnmower : MonoBehaviour
	{
		public LayerMask m_MowArea;
		[Range(0, 1)]
		public float m_MowSensitivity = 0.05f;

		[ContextMenu("Mow The Lawn")]
		void Mow()
		{
			List<GameObject> markedForDestroy = new List<GameObject>();
			foreach (Transform item in transform)
			{
				if (Physics.OverlapSphere(item.position, m_MowSensitivity, m_MowArea).Length > 0)
				{
					markedForDestroy.Add(item.gameObject);
				}
			}

			Debug.Log($"Mowing {markedForDestroy.Count} items");

			foreach (GameObject markedItem in markedForDestroy)
			{
				DestroyImmediate(markedItem);
			}

		}

		[ContextMenu("ChildCount")]
		void GetChildren()
		{
			print(transform.childCount);
		}
	}
}
