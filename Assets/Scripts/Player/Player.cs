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

    // declaring variable 
    // Update is called once per frame
    void Update()
    {
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

        // rotate the direction vector by turn speed degrees each frame
        direction = Quaternion.Euler(0, 0, trueTurnSpeed) * direction;

        // if user is pressing the up key or "accelerating"
        if (Input.GetKey("up"))
        {
            // calculate the acceleration vector 
            acceleration = direction * accelerationRate;
            // Velocity is direction * speed
            velocity += acceleration;
        }
        else if (Input.GetKey("down"))
        {
            // calculate the acceleration vector 
            acceleration = -1 * direction * accelerationRate;
            // Velocity is direction * speed
            velocity += acceleration;
        }
        else // decelerating
        {
            velocity *= decelerationRate;
        }

        // Limit our velocity so that the player doesn't go too fast
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        // add our velocity to our position
        playerPosition += velocity;
        // move the player to it's new position
        transform.position = playerPosition;

        // set the player's rotation to match the direction
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
    }
}