using UnityEngine;

public class Highlighter : MonoBehaviour
{
	[SerializeField] ParticleSystem meshHighlighter, spriteHighlighter;
	public void Highlight(Renderer renderer)
	{
		ParticleSystem.ShapeModule shape;
		ParticleSystem system = null;

		switch (renderer)
		{
			case SpriteRenderer s:
				system = spriteHighlighter;
				shape = system.shape;
				shape.spriteRenderer = s;
				break;
			case MeshRenderer m:
				system = meshHighlighter;
				shape = system.shape;
				shape.meshRenderer = m;
				break;
			default:
				Debug.Log(system);
				break;
		}
		system.Play();
	}

	public void StopHighlight()
	{
		meshHighlighter.Stop();
		meshHighlighter.Clear();
		spriteHighlighter.Stop();
		spriteHighlighter.Clear();
	}
}
