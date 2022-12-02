using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cauldron : MonoBehaviour
{
	Ingredient[] ingredients;
	Ingredient[] chosenIngredients = new Ingredient[4];

	Recipe activeRecipe;

	void Start()
	{
		ChooseIngredients();
	}

	void Update()
	{

	}

	void ChooseIngredients()
	{
		List<int> numList = new();

		for (int i = 0; i < ingredients.Length; i++)
		{
			numList.Add(i);
		}
		numList.Shuffle();
		Queue<int> numQueue = new(numList);

		for (int i = 0; i < chosenIngredients.Length; i++)
		{
			chosenIngredients[i] = ingredients[numQueue.Dequeue()];
		}
	}
}

[System.Serializable]
public class Ingredient
{
	public string name;
	public Sprite spriteRaw;
	public Sprite spriteGround;
	public Sprite spriteSliced;
	public Sprite spriteBoiled;
}

[System.Serializable]
public class PreparedIngredient
{
	public enum CookMethod { Raw, Ground, Sliced, Boiled, Length };
	readonly Ingredient ingredient;
	readonly CookMethod method;
	public Ingredient Ingredient => ingredient;
	public CookMethod Method => method;

	public PreparedIngredient(Ingredient i, CookMethod m)
	{
		ingredient = i;
		method = m;
	}
}

[System.Serializable]
public class Recipe
{
	enum CorrectElements { None, Ingredient, Process, Both };
	PreparedIngredient[] recipeIngredients = new PreparedIngredient[3];


	public Recipe(Ingredient[] potentialIngredients)
	{
		for (int i = 0; i < recipeIngredients.Length; i++)
		{
			recipeIngredients[i] =
				new(potentialIngredients[Random.Range(0, recipeIngredients.Length)], (PreparedIngredient.CookMethod)Random.Range(1, (int)PreparedIngredient.CookMethod.Length));
		}
	}

	public void CheckForIngredients(PreparedIngredient p0, PreparedIngredient p1, PreparedIngredient p2)
	{
		Queue<PreparedIngredient> q0 = new(new[] { p0, p1, p2 });
		List<CorrectElements> results = new();
		ProcessMatch(IngredientMatch(ExactMatch(q0, ref results), ref results), ref results);
	}

	Queue<PreparedIngredient> ExactMatch(Queue<PreparedIngredient> q, ref List<CorrectElements> results)
	{
		if (q.Count == 0) return new();
		// Exact Matches
		Queue<PreparedIngredient> rq = new();
		while (q.Count > 0)
		{
			PreparedIngredient p = q.Dequeue();
			bool matched = false;
			for (int i = 0; i < recipeIngredients.Length; i++)
			{
				if (results[i] != CorrectElements.None) continue;
				if (recipeIngredients[i].Ingredient == p.Ingredient && recipeIngredients[i].Method == p.Method)
				{
					results[i] = CorrectElements.Both;
					matched = true;
					break;
				}
			}
			if (!matched)
			{
				rq.Enqueue(p);
			}
		}
		return rq;
	}
	
	Queue<PreparedIngredient> IngredientMatch(Queue<PreparedIngredient> q, ref List<CorrectElements> results)
	{
		if (q.Count == 0) return new();
		// Exact Matches
		Queue<PreparedIngredient> rq = new();
		while (q.Count > 0)
		{
			PreparedIngredient p = q.Dequeue();
			bool matched = false;
			for (int i = 0; i < recipeIngredients.Length; i++)
			{
				if (results[i] != CorrectElements.None) continue;
				if (recipeIngredients[i].Ingredient == p.Ingredient)
				{
					results[i] = CorrectElements.Ingredient;
					matched = true;
					break;
				}
			}
			if (!matched)
			{
				rq.Enqueue(p);
			}
		}
		return rq;
	}

	Queue<PreparedIngredient> ProcessMatch(Queue<PreparedIngredient> q, ref List<CorrectElements> results)
	{
		if (q.Count == 0) return new();
		// Exact Matches
		Queue<PreparedIngredient> rq = new();
		while (q.Count > 0)
		{
			PreparedIngredient p = q.Dequeue();
			bool matched = false;
			for (int i = 0; i < recipeIngredients.Length; i++)
			{
				if (results[i] != CorrectElements.None) continue;
				if (recipeIngredients[i].Method == p.Method)
				{
					results[i] = CorrectElements.Process;
					matched = true;
					break;
				}
			}
			if (!matched)
			{
				rq.Enqueue(p);
			}
		}
		return rq;
	}
}