using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashlightUI : MonoBehaviour
{
    private Image buttonImage;

    public Sprite flashlightOnSprite;
    public Sprite flashlightOffSprite;

    public PlayerController playerController;

    // Start is called before the first frame update
    void Awake()
    {
        buttonImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        buttonImage.sprite = playerController.flashlightOn ? flashlightOnSprite : flashlightOffSprite;
    }
}
