using UnityEngine;

public abstract class ComponentPauser : MonoBehaviour
{
	protected bool paused;
	protected virtual void Awake()
	{
		GameManager.Pause += Pause;
	}
	protected virtual void Pause(bool paused) => this.paused = paused;

	private void OnDestroy()
	{
		GameManager.Pause -= Pause;
	}
}
