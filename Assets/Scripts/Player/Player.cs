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
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] string projectileName;
    // declaring variable 

    private Vector2 lookOffset = new Vector2(0, 1);

    //invulnerability fields
    private int health = 3;
    public float invTime = 2.0f;        //time spent invincible
    private float invTimer;             //invincibility timer
    private bool invulnerable = false;  //is player invincible
    private SpriteRenderer sprite;

    //dodge roll/dash fields
    public float dashDist = .05f;
    private bool dashing = false;
    public float dashTime = .3f;
    private float dashTimer;
    public float dashCooldown = .5f;
    private float dashCooldownTimer;

    public int Health { get { return health; } }

    private void Start()
    {
        invTimer = invTime;
        dashTimer = dashTime;
        sprite = GetComponent<SpriteRenderer>();
        dashCooldownTimer = 0.0f;
    }
    // Update is called once per frame
    void Update()
    {
        //Spawn bullet
        if (Input.GetKeyDown(KeyCode.Mouse0)) 
        {
            GameObject temp = ObjectPooler.SharedInstance.GetPooledObject(projectileName);
            if (temp != null)
            {
                temp.transform.position = this.gameObject.transform.position;
                //temp.transform.rotation = this.gameObject.transform.rotation;
                temp.GetComponent<Bullet>().SetDirection(direction,projectileSpeed);
            }
        }

        LookAtMouse();

        //invlnerability frames
        if (invulnerable && invTimer >= 0.0f)
            invTimer -= Time.deltaTime;
        else
        {
            invulnerable = false;
            invTimer = invTime;
        }

        //player flashes grey while invincible
        if (invulnerable && sprite.color == Color.white)
            sprite.color = Color.grey;
        else if (invulnerable && sprite.color == Color.grey)
            sprite.color = Color.white;
        else
            sprite.color = Color.white;
    }

    void FixedUpdate()
    {
        if(!dashing)
            PushPlayer();
        // Limit our velocity so that the player doesn't go too fast
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        //player dodges in direction they are moving
        ProcessDash();

        // add our velocity to our position
        playerPosition += velocity;
        // move the player to it's new position
        transform.position = playerPosition;

        // set the player's rotation to match the direction
        if(!dashing)
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

    //Player takes damage and becomes invincible when they are hit
    public void TakeDamage(int amount)
    {
        if (!invulnerable && !dashing)
        {
            invulnerable = true;
            health -= amount;
        }
    }

    //Handle dash calculations and timers
    private void ProcessDash()
    {
        if (!dashing && Input.GetKey(KeyCode.Space) && dashCooldownTimer <= 0.0f)
        {
            dashing = true;
            dashTimer = dashTime;
            //Debug.Log("Dash, direction: " + direction.x + ", " + direction.y);
        }

        if (dashing && dashTimer >= 0.0f)
        {
            dashTimer -= Time.deltaTime;

            //send player backwards if standing still or slowing down
            //BUG: since this checks for if the player has a low velocity, the player will dash backwards
            //if attempting to dash when changing directions (left <-> right / up <-> down), since the players
            //speed gets below the threshold when pressing an opposing direction button, prevents using
            //a dash to change directions quickly
            if (velocity.sqrMagnitude < Mathf.Pow(maxSpeed, 2) / 16)
                direction *= -1;
            else
                direction = velocity.normalized;

            velocity = (dashDist / dashTime) * direction;

            if (dashTimer <= 0.0f)
            {
                //Debug.Log("dash over");
                dashing = false;
                dashCooldownTimer = dashCooldown;
                velocity = Vector3.zero;
            }
        }

        if (dashCooldownTimer > 0.0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }
}