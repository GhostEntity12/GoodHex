using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Ghost
{
	public class AssetPrefabRestorer : MonoBehaviour
	{
		public GameObject[] m_SourcePrefabs;

		[ContextMenu("Fix Prefabs")]
		void FixPrefabs()
		{
			List<GameObject> failedObjects = new List<GameObject>();

			GameObject fixedAssets = new GameObject(gameObject.name + " - Fixed");
			fixedAssets.transform.position = transform.position;
			fixedAssets.transform.parent = transform.parent;
			fixedAssets.transform.SetSiblingIndex(transform.GetSiblingIndex());

			int successCount = 0;

			foreach (Transform child in transform)
			{
				GameObject[] matchingPrefab = m_SourcePrefabs.ToList().Where(prefab => prefab.name == child.name).ToArray();

				if (matchingPrefab.Length > 0)
				{
					successCount++;
					GameObject obj = new();
#if UNITY_EDITOR
					obj = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(matchingPrefab[0], fixedAssets.transform);
#endif
					obj.transform.position = child.position;
					obj.transform.rotation = child.rotation;
					obj.transform.localScale = child.localScale;
					obj.name = child.name;
				}
				else
				{
					Debug.LogWarning($"Couldn't find a matching prefab for {child}", child);
					failedObjects.Add(child.gameObject);
				}
			}

			while (failedObjects.Count > 0)
			{
				failedObjects[0].transform.parent = fixedAssets.transform;
				failedObjects.RemoveAt(0);
			}

			Debug.Log($"Succeeded at replacing {successCount} prefabs!");

			DestroyImmediate(gameObject);
		}
	}
}