using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoryTextController : MonoBehaviour
{

    private TextMeshProUGUI storyTextDisplay;
    private TextMeshProUGUI continueButtonTextDisplay;
    public Button continueButton;
    public string nextLevelName;
    
    public float textSpeed;

    private string storyText;
    private string continueButtonText;
    private int textIndex;

    // Start is called before the first frame update
    void Start()
    {
        storyTextDisplay = GetComponent<TextMeshProUGUI>();

        textIndex = 0;

        storyText = storyTextDisplay.text;
        storyTextDisplay.text = "";

        StartCoroutine(updateText());

        continueButton.onClick.AddListener(TaskOnClick);

        continueButton.enabled = false;
        continueButtonTextDisplay = continueButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        
        continueButtonText = continueButtonTextDisplay.text;
        continueButtonTextDisplay.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
            storyTextDisplay.text = storyText;

            showContinueButton();
        }
    }

    IEnumerator updateText() {
        yield return new WaitForSeconds(textSpeed);
        
        if (storyTextDisplay.text.Length < storyText.Length) {
            storyTextDisplay.text = storyTextDisplay.text + storyText[textIndex];
            textIndex++;

            StartCoroutine(updateText());
        }
        else {
            showContinueButton();
        }
    }

    void TaskOnClick(){
		SceneManager.LoadScene(nextLevelName);
	}

    void showContinueButton() {
        continueButton.enabled = true;
        continueButtonTextDisplay.text = continueButtonText;
    }
}
