using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShowOnMobile : MonoBehaviour
{
    private bool usingGamepad = false;

    // // Start is called before the first frame update
    // void Start() {
    //     gameObject.SetActive(Application.isMobilePlatform); // Works for WebGL too! :)
    // }

    private void Update() {

        if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame) {
            usingGamepad = true;
        }

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) {
            usingGamepad = false;
        }

        gameObject.SetActive(Application.isMobilePlatform && !usingGamepad);

    }
}
