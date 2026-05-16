using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // handling new input system
    public InputActionReference moveAction;
    private Vector2 moveDirection;

    public InputActionReference toggleFlashlightAction;

    public InputActionReference sprintAction;
    private bool sprintHeld;
    public bool sprintToggled;
    private bool isSprinting;

    public InputActionReference aimAction;
    private Vector2 aimDirection;
    private bool usingMouseAim = true;

    private float stickDeadzone = 0.1f;

    public InputActionReference continueAction;

    // used for moving the character and handling physics
    private Rigidbody2D rigidBody;

    // set the player speed
    private float speed;
    [SerializeField] private float walkSpeed = 2.5f;
    [SerializeField] private float sprintSpeed = 5f;

    // keep track of enemy vision cone collision
    private bool inVisionCone;
    GameObject currentGuardObject;

    // handling detection
    public float maxDetection = 100f;
    public float currentDetection;
    public float detectionRate = 100f;
    public float forgetRate = 20f;
    [SerializeField] private AudioClip caughtWarningSound;
    private float caughtSoundCooldown = 3f;
    private float nextCaughtSoundTime = 0f;
    public Image progressBar;
    private bool isCaught;

    // handling sprint
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaUseRate = 50f;
    public float staminaRechargeRate = 30f;
    public Image staminaBar;

    // keep track of key collection
    private bool hasKey;
    [SerializeField] private AudioClip collectKey;

    // handle player text
    public GameObject playerTextObject;
    private TextMeshPro playerText;

    // flashlight controls
    public bool flashlightOn;
    [SerializeField] private Light2D renderingFlashLight;
    [SerializeField] private PolygonCollider2D flashLightCollider;
    [SerializeField] private AudioClip flashlightClickSound;
    private AudioSource audioSource;

    // level controls
    public string nextLevel;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        
        hasKey = false;

        playerText = playerTextObject.GetComponent<TextMeshPro>();

        flashlightOn = false;

        audioSource = GetComponent<AudioSource>();

        currentDetection = 0;

        currentStamina = maxStamina;

        isCaught = false;

        speed = walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {

        if (inVisionCone) {
            //GameObject guardCollidedWith = collision.transform.parent.gameObject;
            Vector2 guardDirection = new Vector2(currentGuardObject.transform.position.x - transform.position.x, currentGuardObject.transform.position.y - transform.position.y);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, guardDirection, guardDirection.magnitude, LayerMask.GetMask("Wall"));

            if (hit) {
                // wall in the way, dont reload scene
                Debug.Log("detected hit");
            }
            else {
                Debug.Log("no wall detected");
                currentDetection += detectionRate * Time.deltaTime;
                //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                if (Time.time >= nextCaughtSoundTime) {
                    audioSource.PlayOneShot(caughtWarningSound);
                    nextCaughtSoundTime = Time.time + caughtSoundCooldown;
                }
            }
        }

        // handle player inputs
        if (!isCaught) {

            // use new input system to move the player
            moveDirection = moveAction.action.ReadValue<Vector2>();

            // sprinting
            sprintHeld = sprintAction.action.IsPressed();
            isSprinting = sprintHeld || sprintToggled;

            if (isSprinting && currentStamina > 0 && moveDirection.sqrMagnitude > stickDeadzone) {
                if (currentStamina > 5) {
                    speed = sprintSpeed;
                }
                currentStamina -= staminaUseRate * Time.deltaTime;
            }
            else {
                currentStamina += staminaRechargeRate * Time.deltaTime;
                speed = walkSpeed;
                sprintToggled = false;
            }

            // where movement once sat, moved to FixedUpdate to prevent "sticky" movement

            // use new input system to aim
            if (usingMouseAim) {
                Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

                aimDirection = (mouseWorldPosition - transform.position);
            }
            else {
                aimDirection = aimAction.action.ReadValue<Vector2>();
            }

            if (aimDirection.sqrMagnitude > stickDeadzone) {
                transform.right = aimDirection.normalized;
            }
            
        }
        else {
            // player got caught
        }

        // handle detection
        //currentDetection += detectionRate * Time.deltaTime;
        currentDetection -= forgetRate * Time.deltaTime;

        if (currentDetection > maxDetection) {
            currentDetection = maxDetection;
        }
        else if (currentDetection < 0) {
            currentDetection = 0;
        }

        if (currentDetection == maxDetection) {
            isCaught = true;
            StartCoroutine(handleCaught());
        }

        progressBar.transform.localScale = new Vector3(1, currentDetection/maxDetection, 1);

        // handle stamina

        if (currentStamina > maxStamina) {
            currentStamina = maxStamina;
        }
        else if (currentStamina < 0) {
            currentStamina = 0;
        }

        staminaBar.transform.localScale = new Vector3(1, currentStamina/maxStamina, 1);

    }

    void FixedUpdate() {
        if (isCaught) return;
        
        if (moveDirection.sqrMagnitude > stickDeadzone) {
            rigidBody.velocity = new Vector2(moveDirection.normalized.x * speed, moveDirection.normalized.y * speed);
        }
        else {
            sprintToggled = false;
        }
    }

    // handling collision with guard vision cones
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "VisionCone") {
            
            inVisionCone = true;
            currentGuardObject = collision.transform.parent.gameObject;
            
        }
        else if (collision.tag == "Key") {
            hasKey = true;
            playerText.text = "Found a key!";
            audioSource.PlayOneShot(collectKey);
            Destroy(collision.gameObject);
            StartCoroutine(clearText());
        }
        else if (collision.tag == "Door") {
            // load next scene, currently just restart scene
            if (hasKey) {
                SceneManager.LoadScene(nextLevel);
            }
            else {
                playerText.text = "Need key...";
                StartCoroutine(clearText());
            }
        }
        else if (collision.tag == "Cake") {
            SceneManager.LoadScene(nextLevel);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "VisionCone") {
            inVisionCone = false;
        }
    }

    IEnumerator clearText() {
        yield return new WaitForSeconds(5f);
        playerText.text = "";
    }

    IEnumerator handleCaught() {
        playerText.text = "Caught.";
        playerText.color = Color.red;
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // enable and disable new input system actions
    private void OnEnable() {
        toggleFlashlightAction.action.performed += toggleFlashlight;
        aimAction.action.performed += OnAimPerformed;
        sprintAction.action.performed += OnSprintPerformed;
        continueAction.action.performed += HandleContinue;
    }

    private void OnDisable() {
        toggleFlashlightAction.action.performed -= toggleFlashlight;
        aimAction.action.performed -= OnAimPerformed;
        sprintAction.action.performed -= OnSprintPerformed;
        continueAction.action.performed -= HandleContinue;

    }

    // toggle flashlight using new input system
    private void toggleFlashlight(InputAction.CallbackContext context) {
        if (isCaught) {
            return;
        }
        
        if (flashlightOn) {
            flashlightOn = false;
            renderingFlashLight.intensity = 0f;
            flashLightCollider.enabled = false;
            audioSource.PlayOneShot(flashlightClickSound);
        }
        else {
            flashlightOn = true;
            renderingFlashLight.intensity = 1f;
            flashLightCollider.enabled = true;
            audioSource.PlayOneShot(flashlightClickSound);
        }
    }

    private void OnAimPerformed(InputAction.CallbackContext context) {
        usingMouseAim = context.control.device is Mouse;
    }

    private void OnSprintPerformed(InputAction.CallbackContext context) {
        if (context.control.path.Contains("leftStickPress")) {
            sprintToggled = !sprintToggled;
        }
    }

    private void HandleContinue(InputAction.CallbackContext context) {
        if (isCaught) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else {
            return;
        }
    }
}

// for help with player rotation:
// https://www.youtube.com/watch?v=9_i6S_rDZuA