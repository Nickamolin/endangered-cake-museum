using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject stationList;

    int stationIndex;
    int numStations;

    private enum State {looking, walking, concerned};
    private State state;

    // used for moving the character and handling physics
    private Rigidbody2D rigidBody;

    // set the enemy speed
    [SerializeField] private float speed = 1f;

    // for player flashlight detection
    public GameObject playerObject;
    private Vector2 lastPlayerPosition;
    private bool inPlayerVisionCone;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        stationIndex = 0;
        numStations = stationList.transform.childCount;
        Debug.Log(numStations.ToString());

        state = State.walking;

        inPlayerVisionCone = false;
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 stationPosition = stationList.transform.GetChild(stationIndex).position;

        Vector2 stationDirection = new Vector2(stationPosition.x - transform.position.x, stationPosition.y - transform.position.y);

        if (inPlayerVisionCone) {
            Vector2 currentlayerPosition = playerObject.transform.position;
            Vector2 playerDirection = new Vector2(currentlayerPosition.x - transform.position.x, currentlayerPosition.y - transform.position.y);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, playerDirection, playerDirection.magnitude, LayerMask.GetMask("Wall"));

            if (hit) {
                // wall in the way, dont reload scene
                Debug.Log("detected hit");
            }
            else {
                Debug.Log("no wall detected");
                state = State.concerned;
                lastPlayerPosition = currentlayerPosition;
                StartCoroutine(lookForPlayer());
            }
        }

        if (state == State.walking) {

            if (stationDirection.magnitude > 0.3f) {
                rigidBody.velocity = speed * stationDirection / stationDirection.magnitude;

                float angle = Mathf.Atan2(stationPosition.y - transform.position.y, stationPosition.x - transform.position.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 100 * Time.deltaTime);

            }
            else {
                state = State.looking;

                stationIndex++;
                if (stationIndex == numStations) {
                    stationIndex = 0;
                }

                StartCoroutine(idle());
            }
            
        }
        else if (state == State.looking) {
            float angle = Mathf.Atan2(stationPosition.y - transform.position.y, stationPosition.x - transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 100 * Time.deltaTime);
        }
        else if (state == State.concerned) {
            float angle = Mathf.Atan2(lastPlayerPosition.y - transform.position.y, lastPlayerPosition.x - transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 100 * Time.deltaTime);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "PlayerVisionCone") {
            
            // lastPlayerPosition = playerObject.transform.position;
            // Vector2 playerDirection = new Vector2(lastPlayerPosition.x - transform.position.x, lastPlayerPosition.y - transform.position.y);

            // RaycastHit2D hit = Physics2D.Raycast(transform.position, playerDirection, playerDirection.magnitude, LayerMask.GetMask("Wall"));

            // if (hit) {
            //     // wall in the way, dont reload scene
            //     Debug.Log("detected hit");
            // }
            // else {
            //     Debug.Log("no wall detected");
            //     state = State.concerned;
            // }
            inPlayerVisionCone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "PlayerVisionCone") {
            inPlayerVisionCone = false;
        }
    }

    IEnumerator idle() {
        yield return new WaitForSeconds(5f);
        
        if (state != State.concerned) {
            state = State.walking;
        }    
    }

    IEnumerator lookForPlayer() {
        yield return new WaitForSeconds(5f);
        
        if (state == State.concerned) {
            state = State.walking;
        }
    }
}
