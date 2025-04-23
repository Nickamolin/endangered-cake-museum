using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // used for moving the character and handling physics
    private Rigidbody2D rigidBody;

    // set the player speed
    [SerializeField] private float speed = 5f;

    // keep track of key collection
    private bool hasKey;

    // handle player text
    public GameObject playerTextObject;
    private TextMeshPro playerText;

    // flashlight controls
    private bool flashlight;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        hasKey = false;

        playerText = playerTextObject.GetComponent<TextMeshPro>();

        flashlight = true;
    }

    // Update is called once per frame
    void Update()
    {

        // handle player inputs
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
        else {
            
        }

        // handle sprite facing direction
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 mouseDirection = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);

        transform.right = mouseDirection;

    }

    // handling collision with guard vision cones
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "VisionCone") {
            
            // string currentSceneName = SceneManager.GetActiveScene().name;
            // SceneManager.LoadScene(currentSceneName);

            GameObject guardCollidedWith = collision.transform.parent.gameObject;
            Vector2 guardDirection = new Vector2(guardCollidedWith.transform.position.x - transform.position.x, guardCollidedWith.transform.position.y - transform.position.y);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, guardDirection, guardDirection.magnitude, LayerMask.GetMask("Wall"));


            // Debug.Log(LayerMask.GetMask("Wall").ToString());
            // Debug.Log(guardDirection.magnitude.ToString());
            // Debug.DrawRay(transform.position, guardDirection, Color.green, 60f);

            if (hit) {
                // wall in the way, dont reload scene
                Debug.Log("detected hit");
            }
            else {
                Debug.Log("no wall detected");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            
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

    IEnumerator clearText() {
        yield return new WaitForSeconds(5f);
        playerText.text = "";
    }
}

// for help with player rotation:
// https://www.youtube.com/watch?v=9_i6S_rDZuA