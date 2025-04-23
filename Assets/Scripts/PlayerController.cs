using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
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
    public float detectionRate = 1f;
    public float forgetRate = 0.5f;

    public Image progressBar;

    private bool isCaught;

    // keep track of key collection
    private bool hasKey;

    // handle player text
    public GameObject playerTextObject;
    private TextMeshPro playerText;

    // flashlight controls
    private bool flashlightOn;
    [SerializeField] private Light2D renderingFlashLight;
    [SerializeField] private PolygonCollider2D flashLightCollider;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        
        hasKey = false;

        playerText = playerTextObject.GetComponent<TextMeshPro>();

        flashlightOn = true;

        currentDetection = 0;

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
            }
        }

        // handle player inputs
        if (!isCaught) {
            if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)) {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, speed);
            }
            if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, -speed);
            }
            if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) {
                rigidBody.velocity = new Vector2(-speed, rigidBody.velocity.y);
            }
            if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) {
                rigidBody.velocity = new Vector2(speed, rigidBody.velocity.y);
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                toggleFlashlight();
            }

            if (Input.GetKeyDown(KeyCode.LeftShift)) {
                speed = sprintSpeed;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift)) {
                speed = walkSpeed;
            }

            // handle sprite facing direction
            Vector3 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Vector2 mouseDirection = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);

            transform.right = mouseDirection;
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
            Destroy(collision.gameObject);
            StartCoroutine(clearText());
        }
        else if (collision.tag == "Door") {
            // load next scene, currently just restart scene
            if (hasKey) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else {
                playerText.text = "Need key...";
                StartCoroutine(clearText());
            }
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

    private void toggleFlashlight() {
        if (flashlightOn) {
            flashlightOn = false;
            renderingFlashLight.intensity = 0f;
            flashLightCollider.enabled = false;
        }
        else {
            flashlightOn = true;
            renderingFlashLight.intensity = 1f;
            flashLightCollider.enabled = true;
        }
    }
}

// for help with player rotation:
// https://www.youtube.com/watch?v=9_i6S_rDZuA