using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Kenneth Rossi
// 10/6/2021
// Class handles all movement for the Player

public class Player : MonoBehaviour
{
    // Declaring all necessary fields
    public Vector3 playerPosition = new Vector3(0, 0, 0);
    // direction that the player is facing
    public Vector3 direction = new Vector3(0, 1, 0);
    private Vector3 velocity = Vector3.zero;
    private Vector3 acceleration = Vector3.zero;
    // setting variables for acceleration and deceleration rates
    public float accelerationRate = 0.001f;
    public float decelerationRate = 0.4f;
    // setting speed-related variables
    public float maxSpeed = 0.01f;
    public float turnSpeed = 0.5f;
    private float trueTurnSpeed = 0f;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] string projectileName;
    // declaring variable 

    private Vector2 lookOffset = new Vector2(0, 1);

    private void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            GameObject temp = ObjectPooler.SharedInstance.GetPooledObject(projectileName);
            if (temp != null)
            {
                temp.transform.position = this.gameObject.transform.position;
                //temp.transform.rotation = this.gameObject.transform.rotation;
                temp.GetComponent<Bullet>().SetDirection(direction,projectileSpeed);
            }
        }
        // check if user is pressing left or right or nothing then applies appropriate turn speed
        if (Input.GetKey("left"))
        {
            trueTurnSpeed = turnSpeed;
        }
        else if (Input.GetKey("right"))
        {
            trueTurnSpeed = -1 * turnSpeed;
        }
        else
        {
            trueTurnSpeed = 0;
        }

        LookAtMouse();
        PushPlayer();

        // Limit our velocity so that the player doesn't go too fast
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        // add our velocity to our position
        playerPosition += velocity;
        // move the player to it's new position
        transform.position = playerPosition;

        // set the player's rotation to match the direction
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
    }

    //Points the player sprite directly towards the mouse
    private void LookAtMouse()
    {
        lookOffset = new Vector2(
            Camera.main.ScreenToWorldPoint(Input.mousePosition).x - playerPosition.x,
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y - playerPosition.y);
        //direction = Mathf.Atan2(lookOffset.y, lookOffset.x);
        direction = lookOffset.normalized;
    }

    //Uses WASD to accelerate the player. Handles deceleration
    private void PushPlayer()
    {
        //Clear acceleration
        acceleration = Vector3.zero;

        //Add acceleration for each direction key pressed
        if (Input.GetKey(KeyCode.W))
            acceleration.y += accelerationRate;
        if (Input.GetKey(KeyCode.A))
            acceleration.x -= accelerationRate;
        if (Input.GetKey(KeyCode.S))
            acceleration.y -= accelerationRate;
        if (Input.GetKey(KeyCode.D))
            acceleration.x += accelerationRate;

        //Cap acceleration
        acceleration = Vector3.ClampMagnitude(acceleration, accelerationRate);
        if (Mathf.Abs(acceleration.x) + Mathf.Abs(acceleration.y) == 0)
            velocity *= decelerationRate;
        //Add acceleration to velocity
        velocity += acceleration;
    }
}