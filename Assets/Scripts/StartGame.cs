using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{

    private TextMeshProUGUI titleTextDisplay;
    private TextMeshProUGUI startButtonTextDisplay;
    public Button startButton;
    public string startingLevelName;
    
    public float textSpeed;

    private string titleText;
    private string startButtonText;
    private int textIndex;
    public AudioSource audioSource;
    public AudioClip characterSound;

    // Start is called before the first frame update
    void Start()
    {
        titleTextDisplay = GetComponent<TextMeshProUGUI>();

        textIndex = 0;

        titleText = titleTextDisplay.text;
        titleTextDisplay.text = "";

        StartCoroutine(updateText());

        startButton.onClick.AddListener(TaskOnClick);

        startButton.enabled = false;
        startButtonTextDisplay = startButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        
        startButtonText = startButtonTextDisplay.text;
        startButtonTextDisplay.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
            titleTextDisplay.text = titleText;

            showStartButton();
        }
    }

    IEnumerator updateText() {
        yield return new WaitForSeconds(textSpeed);
        
        if (titleTextDisplay.text.Length < titleText.Length) {
            titleTextDisplay.text = titleTextDisplay.text + titleText[textIndex];
            audioSource.PlayOneShot(characterSound);
            textIndex++;

            StartCoroutine(updateText());
        }
        else {
            showStartButton();
        }
    }

    void TaskOnClick(){
		SceneManager.LoadScene(startingLevelName);
	}

    void showStartButton() {
        startButton.enabled = true;
        startButtonTextDisplay.text = startButtonText;
    }
}
