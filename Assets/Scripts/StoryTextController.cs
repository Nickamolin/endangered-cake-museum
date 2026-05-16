using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StoryText : MonoBehaviour
{
    [Header("Dialogue Variants")]
    [TextArea] public string keyboardMouseDialogue;
    [TextArea] public string touchDialogue;
    [TextArea] public string gamepadDialogue;

    [Header("Continue Prompt Variants")]
    public string keyboardContinuePrompt = "Click To Continue";
    public string touchContinuePrompt = "Tap To Continue";
    public string gamepadContinuePrompt = "Press [A] To Continue";

    [Header("Advance")]
    public string inputStage;
    public bool tutorialText = false;

    [Header("Optional Continue Button")]
    [SerializeField] private GameObject continueButton;

    [Header("Input")]
    [SerializeField] private InputActionReference continueAction;

    [Header("Typewriter")]
    [SerializeField] private float characterDelay = 0.05f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip characterSound;

    private TextMeshProUGUI dialogueText;
    private TextMeshProUGUI continueButtonText;

    private string currentDialogue;
    private string currentContinuePrompt;

    private Coroutine typeCoroutine;

    private bool isTyping;
    private bool dialogueComplete;

    private void Awake()
    {
        dialogueText = GetComponent<TextMeshProUGUI>();

        if (continueButton != null)
        {
            continueButtonText = continueButton.GetComponentInChildren<TextMeshProUGUI>();
        }

        DetermineInputVariant();
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

    private void Start()
    {
        StartDialogue(currentDialogue);
    }

    private void DetermineInputVariant()
    {
        // Gamepad takes priority
        if (Gamepad.current != null)
        {
            currentDialogue = gamepadDialogue;
            currentContinuePrompt = gamepadContinuePrompt;
            return;
        }

        // Mobile touch
        if (Application.isMobilePlatform)
        {
            currentDialogue = touchDialogue;
            currentContinuePrompt = touchContinuePrompt;
            return;
        }

        // Default desktop
        currentDialogue = keyboardMouseDialogue;
        currentContinuePrompt = keyboardContinuePrompt;
    }

    private void OnContinue(InputAction.CallbackContext context)
    {
        Continue();
    }

    private void StartDialogue(string dialogue)
    {
        dialogueText.text = "";

        if (continueButton != null)
        {
            continueButton.SetActive(false);
        }

        dialogueComplete = false;
        typeCoroutine = StartCoroutine(TypeText(dialogue));
    }

    private IEnumerator TypeText(string dialogue)
    {
        isTyping = true;

        foreach (char c in dialogue)
        {
            dialogueText.text += c;

            if (audioSource != null &&
                characterSound != null &&
                !char.IsWhiteSpace(c))
            {
                audioSource.PlayOneShot(characterSound);
            }

            yield return new WaitForSeconds(characterDelay);
        }

        isTyping = false;
        dialogueComplete = true;

        ShowContinueButton();
    }

    public void Continue()
    {
        if (isTyping)
        {
            FinishTypingImmediately();
            return;
        }

        if (dialogueComplete && !tutorialText)
        {
            Advance();
        }
    }

    private void FinishTypingImmediately()
    {
        if (typeCoroutine != null)
        {
            StopCoroutine(typeCoroutine);
        }

        dialogueText.text = currentDialogue;
        isTyping = false;
        dialogueComplete = true;

        ShowContinueButton();
    }

    private void ShowContinueButton()
    {
        if (continueButton == null) return;

        continueButton.SetActive(true);

        if (continueButtonText != null)
        {
            continueButtonText.text = currentContinuePrompt;
        }
    }

    public void Advance()
    {
        SceneManager.LoadScene(inputStage);
    }
}