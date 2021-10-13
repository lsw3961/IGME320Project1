//Author: Michael Chan

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
    private int _health = 3;
    private int damage = 1;

    //Vectors used in calculating movement
    private Vector2 position;
    private Vector2 velocity;
    private Vector2 push;
    private Vector2 targetVelocity;
    private bool shouldSeek = true;

    //Getter for health field
    public int Health
    { get { return _health; } }

    // Start is called before the first frame update
    void Start()
    {
        //Populate values to minimize errors
        targetVelocity = new Vector2(player.gameObject.transform.position.x, player.gameObject.transform.position.y);
        position = new Vector2(transform.position.x, transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        //Update properties to reflect actual gamestate
        targetVelocity = player.gameObject.transform.position - transform.position;
        targetVelocity = targetVelocity.normalized * maxSpeed;
        position.x = transform.position.x;
        position.y = transform.position.y;
        if (shouldSeek)
        {
            Chase();
            Move();
        }

        //Set actual position to calculated position
        transform.position = new Vector3(position.x, position.y, 0);
    }

    //Unity Collision Detection 
    private void OnCollisionEnter2D(Collision2D other)
    {
        //If bullet hits enemy
        if (other.gameObject.tag == "Projectile")
        {
            TakeDamage(other.gameObject);
        }
        //If enemy hits player
        if (other.gameObject.tag == "Player")
        {
            //Destroy the player and stop behavior
            //other.gameObject.SetActive(false);
            //shouldSeek = false;
            player.GetComponent<Player>().TakeDamage(damage);
        }
    }

    /// <summary>
    /// Causes the enemy to lose a health point
    /// </summary>
    /// <returns>Returns true if this enemy survives the hit</returns>
    public void TakeDamage(GameObject other)
    {
        //Destroy the bullet and decrement health
        other.SetActive(false);
        _health--;

        //Kill enemy if no health
        if (_health <= 0)
            Destroy(this.gameObject);
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
        //Accelerates the enemy to alter its course directly towards the player
        push = targetVelocity - velocity;

        //Caps acceleration
        if (push.magnitude > acceleration)
            push /= (push.magnitude / acceleration);
    }
}
