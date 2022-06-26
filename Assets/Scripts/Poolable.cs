using UnityEngine;

public class Poolable : MonoBehaviour
{
	protected bool active;
	[System.NonSerialized]
	public Pool sourcePool;

	protected void ReturnToPool() => sourcePool.ReturnPooledObject(this);
}
