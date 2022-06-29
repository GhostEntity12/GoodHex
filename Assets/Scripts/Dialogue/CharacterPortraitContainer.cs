using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterPortraitContainer : ScriptableObject
{
    public Sprite neutral, angry, happy, tired;
	public Sprite bodyBox;
	public Sprite nameBox;

	[ContextMenu("Populate Neutral")]
	void PopulateNeutralToAll() => angry = happy = tired = neutral;
}
