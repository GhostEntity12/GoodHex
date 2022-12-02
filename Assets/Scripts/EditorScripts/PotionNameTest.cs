using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionNameTest : MonoBehaviour
{
	string[] words1 =
	{
		"Potion",
		"Elixir",
		"Balm",
		"Oil",
		"Tonic",
		"Soda",
		"Water",
		"Slime",
		"Brew",
		"Concoction",
		"Soup",
		"Tincture",
		"Tears",
	};

	string[] words2 =
	{
		"Wet",
		"Green",
		"Gastric",
		"Foul",
		"Frightening",
		"Unholy",
		"Cheesy",
		"Vague",
		"Unhealthy",
		"Fast",
		"Slowed",
		"Apathetic",
		"Clean",
		"Moderate",
		"Good",
		"Mild",
		"Edible",
		"Disappointing",
		"Declining",
		"Veiled",
		"Existential",
		"Magical",
		"Disproven",
		"Prancing",
		"Decaying",
		"Creeping",
		"Absolute",
		"Damp",
		"Moist",
		"Philosophical"
	};

	string[] words3 =
	{
		"Threats",
		"Dampness",
		"Jumps",
		"Drenching",
		"Food",
		"Joy",
		"Decay",
		"Growth",
		"Literacy",
		"Health",
		"Intelligence",
		"Stamina",
		"Strength",
		"Dread",
		"Cheese",
		"Steve",
		"Unicorns",
		"Grades",
		"Nothing",
		"Rememberance",
		"Nostalgia",
	};

	[ContextMenu("Print")]
	void Print()
	{
		for (int i = 0; i < 50; i++)
		{
			Debug.Log($"{words1[Random.Range(0, words1.Length)]} of {words2[Random.Range(0, words2.Length)]} {words3[Random.Range(0, words3.Length)]}");
		}
	}
}
