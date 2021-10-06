using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //Enemy properties
    //  Sole player to target
    [SerializeField] private GameObject player;
    [SerializeField] private float maxSpeed = 1;
    [SerializeField] private float acceleration = 1;

    //Vectors used in calculating movement
    private Vector2 position;
    private Vector2 velocity;
    private Vector2 push;
    private Vector2 targetLocation;

    // Start is called before the first frame update
    void Start()
    {
        //Populate values to minimize errors
        targetLocation = new Vector2(player.gameObject.transform.position.x, player.gameObject.transform.position.y);
        position = new Vector2(transform.position.x, transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        //Update properties to reflect actual gamestate
        targetLocation.x = player.gameObject.transform.position.x;
        targetLocation.y = player.gameObject.transform.position.y;
        position.x = transform.position.x;
        position.y = transform.position.y;

        Chase();
        Move();

        //Set actual position to calculated position
        transform.position = new Vector3(position.x, position.y, 0);
    }

    //Unity Collision Detection 
    private void OnCollisionEnter2D(Collision2D other)
    {
        //If bullet hits enemy
        if (other.gameObject.tag == "Projectile")
        {
            //Destroy the bullet and the enemy
            other.gameObject.SetActive(false);
            Destroy(this.gameObject);
        }
    }

    //Moves the position of the enemy on the screen based on current velocity and acceleration
    private void Move()
    {
        //Applies acceleration, normalized to program speed
        velocity += push * Time.deltaTime;

        //Caps speed
        if (velocity.magnitude > maxSpeed)
            velocity /= (velocity.magnitude / maxSpeed);

        //Applies movement, normalized to program speed
        position += velocity * Time.deltaTime;
    }

    //Modifies the push vector to direct the enemy towards the player
    private void Chase()
    {
        push = targetLocation - position;

        //Caps acceleration
        if (push.magnitude > acceleration)
            push /= (push.magnitude / acceleration);
    }
}
