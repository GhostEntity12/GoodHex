using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DialogueManager : Singleton<DialogueManager>
{
	private enum Side { Left, Right }

	public static DialogueManager instance;

	public bool DialogueActive { get; private set; }
	bool isDisplayingText;
	IEnumerator displayDialogueCoroutine;

	[SerializeField, Tooltip("The post processing volume that blurs the background")] Volume blur;

	[Header("UI")]
	[SerializeField, Tooltip("How quickly the UI fades in")] float uiFadeInSpeed = 0.4f;

	[Space(15)]
	[SerializeField, Tooltip("The textbox which holds characters' names")] TextMeshProUGUI nameTextbox;
	[SerializeField, Tooltip("The object which holds characters' names")] Image nameBox;
	[SerializeField, Tooltip("The container for the object which holds a character's name")] RectTransform nameHolder;
	[SerializeField, Tooltip("The textbox which holds characters' dialogue")] TextMeshProUGUI dialogueTextbox;
	[SerializeField, Tooltip("The object which holds characters' dialogue")] Image dialogueBox;
	Vector2 namePos;

	[SerializeField, Tooltip("The left-side object which holds characters' image")] Image bustL;
	[SerializeField, Tooltip("The right-side object which holds characters' image")] Image bustR;
	[SerializeField, Tooltip("A black canvas to dim the background")] CanvasGroup darkenedBackground;
	CanvasGroup dialogueCanvas;

	[SerializeField, Tooltip("The prompt to skip the dialogue")] TweenedElement skipDialogueDisplay;
	bool skipDialogueDisplayActive = false;

	[Header("Text Display Options")]
	[SerializeField] KeyCode[] ProgressionKeys = new KeyCode[1] { KeyCode.Return };
	[SerializeField, Tooltip("The length of time to wait between displaying characters")] float delay = 0.05f;
	[SerializeField] float m_TweenSpeed = 0.2f;

	[Header("File")]
	[SerializeField, Tooltip("The scene to load")] TextAsset sceneName;
	[SerializeField, Tooltip("Whether to clear the scene after it has run")] bool clearAfterScene;
	readonly Queue<TextAsset> sceneQueue = new();
	readonly Queue<Action> onFinishDialogueActions = new();
	string[] parsedText;
	string[] fileLines;
	int currentLine;
	string characterName, characterDialogue, characterExpression;

	[Header("Characters")]
	[SerializeField, Tooltip("The sprite to display if the character cannot be found")] Sprite defaultCharacterSprite;
	[SerializeField] TweenedElement leftSpeaker;
	[SerializeField] TweenedElement rightSpeaker;
	[SerializeField] TweenedElement dialogueUI;
	readonly Dictionary<string, CharacterPortraitContainer> characterDictionary = new();
	CharacterPortraitContainer leftCharacter;
	CharacterPortraitContainer rightCharacter;
	CharacterPortraitContainer currentCharacter;
	Vector2 defaultPortraitSize;

	private void Start()
	{
		foreach (CharacterPortraitContainer characterPortraits in Resources.LoadAll<CharacterPortraitContainer>("CharacterDialogueSprites")) // Creates the dictionary
		{
			characterDictionary.Add(characterPortraits.name, characterPortraits);
		}
		ClearDialogueBox(); // Clears the dialogue box, just in case
		namePos = nameHolder.anchoredPosition;
		defaultPortraitSize = bustL.rectTransform.sizeDelta;
		dialogueCanvas = GetComponent<CanvasGroup>();

		leftSpeaker.SetCachesAndPosition(new Vector2(-1200, 0));
		rightSpeaker.SetCachesAndPosition(new Vector2(1200, 0));
		dialogueUI.SetCachesAndPosition(new Vector2(0, -400));
		skipDialogueDisplay.SetCachesAndPosition(new Vector2(0, 800));
	}

	/// <summary>
	/// Clears the dialogue box's name, dialogue and image
	/// </summary>
	void ClearDialogueBox()
	{
		bustL.sprite = null;
		bustR.sprite = null;
		nameTextbox.text = string.Empty;
		dialogueTextbox.text = string.Empty;
		currentCharacter = null;
	}

	/// <summary>
	/// Loads the next line from the fileLines array
	/// </summary>
	void LoadNewLine()
	{
		// Split the line into its components and store them
		parsedText = fileLines[currentLine].Split('|');
		currentLine++;

		bool sameChar = false;
		// Check if it's the same character
		if (currentCharacter)
		{
			sameChar = currentCharacter.name == parsedText[0];
		}

		// Set the variables
		currentCharacter = characterDictionary[parsedText[0]];
		characterName = currentCharacter.name;
		characterExpression = parsedText[1].ToLower();
		characterDialogue = parsedText[2];

		Debug.Log(currentCharacter);
		if (sameChar)
		{
			StartDisplaying();
		}
		else
		{
			SwapDialogue(currentCharacter);
		}

	}
	public void StartDisplaying()
	{
		isDisplayingText = true; // Marks the system as typing out letters. Used to determine what happens when pressing enter
								 // Set the portrait
		try
		{
			switch (parsedText[3].ToLower()[0])
			{
				case 'l':
					ManageDialoguePortrait(Side.Left);
					break;
				case 'r':
					ManageDialoguePortrait(Side.Right);
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		catch (IndexOutOfRangeException)
		{
			if (leftCharacter == currentCharacter || leftCharacter == null)
			{
				ManageDialoguePortrait(Side.Left);
			}
			else
			{
				ManageDialoguePortrait(Side.Right);
			}
		}

		if (leftCharacter == rightCharacter)
		{
			Debug.LogError($"{characterName} is taking up both sides!");
		}

		// Clears the dialogue box
		dialogueTextbox.text = string.Empty;

		// Sets the name box
		nameTextbox.text = characterName;

		// Declare and then start the coroutine/IEnumerator so it can be stopped later
		if (displayDialogueCoroutine != null) StopCoroutine(displayDialogueCoroutine);
		displayDialogueCoroutine = DisplayDialogue(characterDialogue);
		StartCoroutine(displayDialogueCoroutine);
	}

	private void ManageDialoguePortrait(Side side)
	{
		// Set references
		CharacterPortraitContainer characterA;
		Image bustA;
		TweenedElement speakerA;
		CharacterPortraitContainer characterB;
		Image bustB;
		TweenedElement speakerB;

		if (side == Side.Left)
		{
			characterA = leftCharacter;
			bustA = bustL;
			speakerA = leftSpeaker;
			leftCharacter = currentCharacter;

			characterB = rightCharacter;
			bustB = bustR;
			speakerB = rightSpeaker;
			nameHolder.anchoredPosition = namePos;
		}
		else
		{
			characterA = rightCharacter;
			bustA = bustR;
			speakerA = rightSpeaker;
			rightCharacter = currentCharacter;

			characterB = leftCharacter;
			bustB = bustL;
			speakerB = leftSpeaker;
			nameHolder.anchoredPosition = new Vector2(-namePos.x, namePos.y);
		}

		// Grey out the other character
		LeanTween.color(bustB.rectTransform, Color.gray, 0.1f);
		LeanTween.color(bustA.rectTransform, Color.white, 0.1f);

		// Swap portraits
		if (characterA == currentCharacter)
		{
			bustA.sprite = GetCharacterPortrait(currentCharacter, characterExpression);
			// Grow the active speaker
			LeanTween.size(bustB.rectTransform, defaultPortraitSize, 0.1f);
			LeanTween.size(bustA.rectTransform, defaultPortraitSize * 1.1f, 0.1f);
		}
		else
		{
			LeanTween.color(bustB.rectTransform, Color.gray, 0.1f);
			LeanTween.color(bustA.rectTransform, Color.white, 0.1f);
			characterA = currentCharacter;
			speakerA.SlideElement(TweenedElement.ScreenState.Offscreen, () =>
			{
				bustA.sprite = GetCharacterPortrait(currentCharacter, characterExpression);
				speakerA.SlideElement(TweenedElement.ScreenState.Onscreen);
				// Grow the active speaker
				LeanTween.size(bustB.rectTransform, defaultPortraitSize, 0.1f);
				LeanTween.size(bustA.rectTransform, defaultPortraitSize * 1.1f, 0.1f);
			});
		}
	}

	/// <summary>
	/// Converts the expression string into the associated Sprite variable in the given character. 
	/// Returns the unknown character sprite if no associated character is found
	/// </summary>
	/// <returns></returns>
	Sprite GetCharacterPortrait(CharacterPortraitContainer character, string expression)
	{
		Debug.Log(character);
		Debug.Log(expression);
		try { return (Sprite)typeof(CharacterPortraitContainer).GetField(expression).GetValue(character); }
		catch (Exception exception)
		{
			if (exception is KeyNotFoundException) // Character not found - return default character sprite.
			{
				return defaultCharacterSprite;
			}
			else if (exception is NullReferenceException) // Expression not found - return neutral.
			{
				return (Sprite)typeof(CharacterPortraitContainer).GetField("neutral").GetValue(character);
			}
			else throw exception;

		}
	}

	/// <summary>
	/// Displays a given string letter by letter
	/// </summary>
	/// <param name="text">The text to display</param>
	/// <returns></returns>
	IEnumerator DisplayDialogue(string text)
	{
		for (int i = 0; i < text.Length; i++) // Adds a letter to the textbox then waits the delay time
		{
			if (text[i] == '<') // If the letter is an opening tag character, autofill the rest of the tag
			{
				int indexOfClose = text[i..].IndexOf('>');
				if (indexOfClose == -1)
				{
					dialogueTextbox.text += text[i];
					yield return new WaitForSeconds(delay);
					continue;
				}
				dialogueTextbox.text += text.Substring(i, indexOfClose);
				i += indexOfClose - 1;
				continue;
			}

			dialogueTextbox.text += text[i];
			yield return new WaitForSeconds(delay);
		}

		isDisplayingText = false; // Marks the system as no longer typing out
	}

	private void Update()
	{
		dialogueCanvas.interactable = dialogueCanvas.blocksRaycasts = DialogueActive;
		if (Input.GetKeyDown(KeyCode.Escape) && DialogueActive)
		{
			skipDialogueDisplay.SlideElement(skipDialogueDisplayActive ? TweenedElement.ScreenState.Offscreen : TweenedElement.ScreenState.Onscreen, tweenSpeed: 0.05f);
			skipDialogueDisplayActive = !skipDialogueDisplayActive;
			return;
		}
		if (skipDialogueDisplayActive) return;

		if (GetAnyKeyDown(ProgressionKeys) && DialogueActive) // If enter is pressed and the textboxes are visible
		{
			if (isDisplayingText) // If the system is currently typing out, finish and return
			{
				StopCoroutine(displayDialogueCoroutine); // Stops the typing out
				displayDialogueCoroutine = null;
				dialogueTextbox.text = characterDialogue; // Fills the textbox with the entirety of the character's line
				isDisplayingText = false; // Marks the system as no longer typing out
				return;
			}
			else if (currentLine >= fileLines.Length) // If there are no more lines
			{
				EndScene();
			}
			else
			{
				LoadNewLine(); // Loads the next line
			}
		}
	}

	public void EndScene()
	{
		Debug.Log($"<color=#5cd3e0>[Dialogue]</color> Finished dialogue {sceneName.name}");
		LeanTween.alphaCanvas(darkenedBackground, 0.0f, 0.2f);
		if (blur)
		{
			LeanTween.value(blur.gameObject, Blur, 1, 0, 0.2f);
		}
		if (displayDialogueCoroutine != null) StopCoroutine(displayDialogueCoroutine); // Stops the typing out
		displayDialogueCoroutine = null;
		dialogueTextbox.text = characterDialogue; // Fills the textbox with the entirety of the character's line
		isDisplayingText = false; // Marks the system as no longer typing out

		currentLine = fileLines.Length;

		if (clearAfterScene) // Clears the scene if told to
		{
			sceneName = null;
		}
		SwapFromDialogue();
		leftCharacter = null;
		rightCharacter = null;
		DialogueActive = false;
		LeanTween.size(bustL.rectTransform, defaultPortraitSize, 0.1f);
		LeanTween.size(bustR.rectTransform, defaultPortraitSize, 0.1f);
		leftSpeaker.SlideElement(TweenedElement.ScreenState.Offscreen, FinishDialogue);
		rightSpeaker.SlideElement(TweenedElement.ScreenState.Offscreen);

		onFinishDialogueActions.Dequeue()?.Invoke();
	}

	/// <summary>
	/// Queues a dialogue file to be read
	/// </summary>
	/// <param name="_sceneName"></param>
	/// <param name="darkenAmount"></param>
	/// <param name="onEndAction"></param>
	public void QueueDialogue(TextAsset _sceneName, float darkenAmount = 0.9f, Action onEndAction = null)
	{
		Debug.Log($"<color=#5cd3e0>[Dialogue]</color> Queuing dialogue {_sceneName.name}");
		sceneQueue.Enqueue(_sceneName);
		onFinishDialogueActions.Enqueue(onEndAction);
		if (!DialogueActive)
		{
			TriggerDialogue(sceneQueue.Dequeue(), darkenAmount);
		}
	}

	private void TriggerDialogue(TextAsset _sceneName, float darkenAmount = 0.9f)
	{
		GameManager.Instance.SetPaused(true);
		LeanTween.alphaCanvas(darkenedBackground, darkenAmount, 0.4f);
		if (blur)
		{
			LeanTween.value(blur.gameObject, Blur, 0, 1, 0.4f);
		}
		DialogueActive = true;
		ClearDialogueBox();
		sceneName = _sceneName;
		Debug.Log($"<color=#5cd3e0>[Dialogue]</color> Starting dialogue {sceneName.name}");

		// Loads the file into memory
		TextAsset file = sceneName;

		// Throws error no matching file exists
		if (file == null)
		{
			Debug.LogError($"Dialogue file not found!");
			return;
		}
		if (!file.name.StartsWith("Dia_"))
		{
			Debug.LogError($"\"{file.name}\" isn't a dialogue file!");
			sceneName = null;
			return;
		}

		// Splits the input on its new lines
		fileLines = file.text.TrimEnd(Environment.NewLine.ToCharArray()).Split(
			new[] { "\r\n", "\r", "\n", Environment.NewLine },
			StringSplitOptions.None
			);
		currentLine = 0;
		LoadNewLine();
	}

	void FinishDialogue()
	{
		ClearDialogueBox();
		if (sceneQueue.Count > 0)
		{
			TriggerDialogue(sceneQueue.Dequeue());
		}
		GameManager.Instance.SetPaused(false);
	}

	private bool GetAnyKeyDown(params KeyCode[] aKeys)
	{
		foreach (var key in aKeys)
			if (Input.GetKeyDown(key))
				return true;
		return false;
	}

	private void Blur(float f) => blur.weight = f;

	public void SwapToDialogue(TextAsset sourceFile, float darkenAmount = 0.9f, Action onDialogueEndAction = null)
	{
		if (sourceFile)
		{
			QueueDialogue(sourceFile, darkenAmount, onDialogueEndAction);
		}
		else
		{
			Debug.LogError("No dialogue file!");
		}
	}

	public void SwapFromDialogue()
	{
		dialogueUI.SlideElement(TweenedElement.ScreenState.Offscreen);
	}

	/// <summary>
	/// Swaps the style of the dialogue
	/// </summary>
	/// <param name="uiData"></param>
	public void SwapDialogue(CharacterPortraitContainer character)
	{
		Debug.Log(character);
		dialogueUI.SlideElement(TweenedElement.ScreenState.Offscreen,
			() => LoadDialogueSkin(character.bodyBox, character.nameBox,
				() => dialogueUI.SlideElement(TweenedElement.ScreenState.Onscreen)));
	}

	/// <summary>
	/// Loads a skin for the dialogue UI
	/// </summary>
	/// <param name="uiStyle"></param>
	/// <param name="actionOnFinish"></param>
	private void LoadDialogueSkin(Sprite body, Sprite name, Action onComplete = null)
	{
		dialogueBox.sprite = body;
		nameBox.sprite = name;
		StartDisplaying();
		onComplete?.Invoke();
	}

	public void ToggleSkip()
	{
		skipDialogueDisplay.SlideElement(skipDialogueDisplayActive ? TweenedElement.ScreenState.Offscreen : TweenedElement.ScreenState.Onscreen, tweenSpeed: 0.05f);
		skipDialogueDisplayActive = !skipDialogueDisplayActive;
	}
}

[Serializable]
public class TweenedElement
{
	public enum ScreenState { Onscreen, Offscreen }

	[SerializeField] RectTransform rectTransform;
	internal Vector2[] cache = new Vector2[2];

	/// <summary>
	/// Caches the positions of the TweenedElement object for tweening
	/// </summary>
	/// <param name="offset">The offset for when the element is offscreen</param>
	public void SetCachesAndPosition(Vector2 offset)
	{
		cache[0] = rectTransform.anchoredPosition;
		cache[1] = cache[0] + offset;
		rectTransform.anchoredPosition = cache[1];
	}

	/// <summary>
	/// Abstracted function which allows sliding UI elements on or offscreen if they are defined as TweenedElements
	/// </summary>
	/// <param name="element">The element to be tweened</param>
	/// <param name="screenState">Whether the object should be on or off screen at the end of the tween</param>
	/// <param name="onComplete">Function on callback</param>
	/// <param name="tweenType">Overides the twwn type</param>
	public void SlideElement(ScreenState screenState, Action onComplete = null, LeanTweenType tweenType = LeanTweenType.easeInOutCubic, float tweenSpeed = 0.2f) =>
		LeanTween.move(rectTransform, cache[(int)screenState], tweenSpeed).setEase(tweenType).setOnComplete(onComplete);

	public void ScaleElement(ScreenState screenState, Action onComplete = null, LeanTweenType tweenType = LeanTweenType.easeInCubic, float tweenSpeed = 0.2f) =>
		LeanTween.scale(rectTransform, screenState == ScreenState.Onscreen ? Vector3.one : Vector3.zero, tweenSpeed).setEase(tweenType).setOnComplete(onComplete);
}