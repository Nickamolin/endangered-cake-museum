using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoryTextController : MonoBehaviour
{

    private TextMeshProUGUI storyTextDisplay;
    private TextMeshProUGUI continueButtonText;
    public Button continueButton;
    public string nextLevelName;
    
    public float textSpeed;

    private string storyText;
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
        continueButtonText = continueButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        continueButtonText.text = "";
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator updateText() {
        yield return new WaitForSeconds(textSpeed);
        
        if (storyTextDisplay.text.Length < storyText.Length) {
            storyTextDisplay.text = storyTextDisplay.text + storyText[textIndex];
            textIndex++;

            StartCoroutine(updateText());
        }
        else {
            continueButton.enabled = true;
            continueButtonText.text = "Continue";
        }
    }

    void TaskOnClick(){
		SceneManager.LoadScene(nextLevelName);
	}
}
