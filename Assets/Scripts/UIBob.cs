using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBob : MonoBehaviour
{
	[SerializeField] float bobHeight = 0.01f;
	[SerializeField] float bobTime = 1f;

	Vector3 cache;

	private void Start()
	{
		cache = transform.localPosition;
	}

	// Update is called once per frame
	void Update()
	{
		transform.localPosition = cache + (bobHeight * Vector3.up * Mathf.Sin(Time.time * bobTime));
	}
}
