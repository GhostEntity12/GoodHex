using UnityEngine;
using UnityEngine.UI;

public class RatEmotes : MonoBehaviour
{
	public enum Emotes { Happy, Sad, Angry, Celebrate, Food, Idea, Love, Sleepy }

	float demoTimer;
	float timer;
	[SerializeField] float emoteDuration = 1.5f;
	[SerializeField] Image bubbleImage;
	[SerializeField] Image emoteImage;

	[SerializeField] Sprite[] emotes;

	bool emoteActive;

	Camera cam;
	Canvas c;

	bool paused;

	private void Start()
	{
		cam = Camera.main;
		c = GetComponent<Canvas>();
		c.transform.rotation = Quaternion.Euler(-cam.transform.rotation.eulerAngles.x, 0, 0);
		demoTimer = Random.Range(2f, 15f);
	}

	// Update is called once per frame
	void Update()
	{
		if (paused) return;

		// Not setting position at the moment, as the canvas is parented to the rat
		if (emoteActive)
		{
			bool flip = cam.WorldToScreenPoint(transform.position).x < cam.pixelWidth / 2f;
			c.transform.localScale = flip ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
			emoteImage.transform.localScale = flip ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);

			timer += Time.deltaTime;
			if (timer > emoteDuration)
			{
				emoteActive = false;
				emoteImage.enabled = false;
				bubbleImage.enabled = false;
			}
		}
		// Demo stuff -random auto emotes
		else
		{
			demoTimer -= Time.deltaTime;
			if (demoTimer <= 0)
			{
				SetEmote((Emotes)Random.Range(0, 8));
				demoTimer = Random.Range(2f, 15f);
			}
		}
	}

	public void SetEmote(Emotes emote) => SetEmote(emotes[(int)emote]);

	public void SetEmote(Sprite emote)
	{
		Debug.Log("Setting emote");
		bubbleImage.enabled = true;
		emoteImage.enabled = true;
		emoteActive = true;

		emoteImage.sprite = emote;
		timer = 0;
	}

	public void SetPaused(bool paused) => this.paused = paused;
}
