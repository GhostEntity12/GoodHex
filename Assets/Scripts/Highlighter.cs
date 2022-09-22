using UnityEngine;

public class Highlighter : MonoBehaviour
{
	[SerializeField] ParticleSystem meshHighlighter, spriteHighlighter;
	public void Highlight(Renderer renderer, bool doHighlight)
	{
		ParticleSystem.ShapeModule shape;
		ParticleSystem system = null;

		switch (renderer)
		{
			case SpriteRenderer s:
				system = spriteHighlighter;
				shape = system.shape;
				shape.spriteRenderer = doHighlight ? s : null;
				break;
			case MeshRenderer m:
				system = meshHighlighter;
				shape = system.shape;
				shape.meshRenderer = doHighlight ? m : null;
				break;
		}

		if (doHighlight)
		{
			system.Play();
		}
		else
		{
			system.Stop();
		}
	}
}
