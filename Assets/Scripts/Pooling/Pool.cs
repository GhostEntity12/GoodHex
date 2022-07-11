using System.Collections.Generic;
using UnityEngine;

public abstract class Pool : MonoBehaviour
{
	public Poolable sourceObject;
	readonly Queue<Poolable> itemPool = new();

	public virtual Poolable GetPooledObject()
	{
		Poolable newObject;
		if (itemPool.Count == 0)
		{
			newObject = CreateNewPooledObject();
		}
		else
		{
			newObject = itemPool.Dequeue();
		}

		newObject.gameObject.SetActive(true);
		return newObject;
	}

	public virtual void ReturnPooledObject(Poolable returningObject)
	{
		itemPool.Enqueue(returningObject);
		returningObject.gameObject.SetActive(false);
	}

	public virtual Poolable CreateNewPooledObject()
	{
		Debug.LogWarning($"Ran out of items in the {gameObject.name} pool, instantiating a new instance");
		Poolable newObject = Instantiate(sourceObject.gameObject).GetComponent<Poolable>();
		newObject.sourcePool = this;
		newObject.gameObject.name = sourceObject.name + " (Pooled)";
		return newObject;
	}
}
