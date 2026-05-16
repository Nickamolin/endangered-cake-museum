using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class StartGame : MonoBehaviour
{
    private TextMeshProUGUI titleTextDisplay;
    private TextMeshProUGUI startButtonTextDisplay;

    [Header("Scene")]
    public Button startButton;
    public string startingLevelName;

    [Header("Typewriter")]
    public float textSpeed = 0.05f;
    public AudioSource audioSource;
    public AudioClip characterSound;

    [Header("Input")]
    [SerializeField] private InputActionReference continueAction;

    [Header("Start Prompt Variants")]
    [TextArea] public string keyboardPrompt = "Press Space to Start";
    [TextArea] public string touchPrompt = "Tap to Start";

    // Kept for possible future controller-specific prompts.
    [TextArea] public string gamepadPrompt = "Press [A] to Start";

    private string titleText;
    private string startButtonText;
    private int textIndex;

    private Coroutine textCoroutine;
    private bool isTyping = false;
    private bool startButtonShown = false;

    private void Start()
    {
        titleTextDisplay = GetComponent<TextMeshProUGUI>();

        textIndex = 0;

        titleText = titleTextDisplay.text;
        titleTextDisplay.text = "";

        startButton.onClick.AddListener(TaskOnClick);
        startButton.enabled = false;

        startButtonTextDisplay = startButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        startButtonText = GetPromptText();
        startButtonTextDisplay.text = "";

        textCoroutine = StartCoroutine(UpdateText());
    }

    private void OnEnable()
    {
        if (continueAction != null)
        {
            continueAction.action.Enable();
            continueAction.action.performed += OnContinue;
        }
    }

    private void OnDisable()
    {
        if (continueAction != null)
        {
            continueAction.action.performed -= OnContinue;
            continueAction.action.Disable();
        }
    }

    private void OnContinue(InputAction.CallbackContext context)
    {
        Continue();
    }

    public void Continue()
    {
        if (isTyping)
        {
            FinishTypingImmediately();
            return;
        }

        if (startButtonShown)
        {
            TaskOnClick();
        }
    }

    private IEnumerator UpdateText()
    {
        isTyping = true;

        while (titleTextDisplay.text.Length < titleText.Length)
        {
            yield return new WaitForSeconds(textSpeed);

            titleTextDisplay.text += titleText[textIndex];

            if (audioSource != null && characterSound != null && !char.IsWhiteSpace(titleText[textIndex]))
            {
                audioSource.PlayOneShot(characterSound);
            }

            textIndex++;
        }

        isTyping = false;
        ShowStartButton();
    }

    private void FinishTypingImmediately()
    {
        if (textCoroutine != null)
        {
            StopCoroutine(textCoroutine);
        }

        titleTextDisplay.text = titleText;
        isTyping = false;

        ShowStartButton();
    }

    private string GetPromptText()
    {
        // For now, only distinguish mobile touch vs desktop keyboard/mouse.
        // Gamepad prompt is kept for possible future use.

        if (Application.isMobilePlatform)
        {
            return touchPrompt;
        }

        return keyboardPrompt;
    }

    private void ShowStartButton()
    {
        if (startButtonShown) return;

        startButtonShown = true;
        startButton.enabled = true;

        startButtonText = GetPromptText();
        startButtonTextDisplay.text = startButtonText;
    }

    private void TaskOnClick()
    {
        SceneManager.LoadScene(startingLevelName);
    }
}