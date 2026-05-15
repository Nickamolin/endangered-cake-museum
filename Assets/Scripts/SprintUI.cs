using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SprintUI : MonoBehaviour
{
    private Image buttonImage;

    public Sprite sprintOnSprite;
    public Sprite sprintOffSprite;

    public PlayerController playerController;

    // Start is called before the first frame update
    void Awake()
    {
        buttonImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        buttonImage.sprite = playerController.sprintToggled ? sprintOnSprite : sprintOffSprite;
    }
}
