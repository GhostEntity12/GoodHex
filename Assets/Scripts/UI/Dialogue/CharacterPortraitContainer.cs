using UnityEngine;

[CreateAssetMenu]
public class CharacterPortraitContainer : ScriptableObject
{
    public Sprite neutral, angry, happy, surprised, thinking;
	public Sprite bodyBox;
	public Sprite nameBox;

	[ContextMenu("Populate Neutral")]
	void PopulateNeutralToAll() => angry = happy = surprised = neutral = thinking;
}
