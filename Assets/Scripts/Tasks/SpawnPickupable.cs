using UnityEngine;

public class SpawnPickupable : MonoBehaviour
{

    [SerializeField] GameObject itemToSpawn;
    [SerializeField] GameObject spawner;

	public void Spawn()
	{
		Instantiate(itemToSpawn, spawner.transform.position, Quaternion.identity);
	}
}
