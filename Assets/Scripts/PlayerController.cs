using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // used for moving the character and handling physics
    private Rigidbody2D rigidBody;

    // set the player speed
    [SerializeField] private float speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
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
}

// for help with player rotation:
// https://www.youtube.com/watch?v=9_i6S_rDZuA