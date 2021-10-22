using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Kenneth Rossi
// 10/6/2021
// Class handles all movement for the Player

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 10f;
    public Rigidbody2D playerBody;

    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] string projectileName;
    [SerializeField] public Vector3 direction;
    private float rotateAngle;

    
    [SerializeField] private Vector2 movement;
    private float dashMovementX;
    private float dashMovementY;
    [SerializeField] private Vector2 dashMovement;

    //invulnerability fields
    private int health = 3;
    public float invTime = 2.0f;        //time spent invincible
    private float invTimer;             //invincibility timer
    private bool invulnerable = false;  //is player invincible
    private SpriteRenderer sprite;

    //dodge roll/dash fields
    public float coolDownTime = 1.5f;
    public float coolDownTimer;
    private bool onCoolDown = false;
    public float activeTime = 1.5f;
    public float activeTimer;
    private bool active = false;

    public int Health { get { return health; } }

    // Start is called before the first frame update
    void Start()
    {
        invTimer = invTime;
        sprite = GetComponent<SpriteRenderer>();
        playerBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Grab input and store in vector
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize();

        // Rotate player to mouse position
        direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        direction.Normalize();
        rotateAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.Euler(0f, 0f, rotateAngle);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

        //Spawn bullet
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject temp = ObjectPooler.SharedInstance.GetPooledObject(projectileName);
            if (temp != null)
            {
                temp.transform.position = this.gameObject.transform.position;
                //temp.transform.rotation = this.gameObject.transform.rotation;
                temp.GetComponent<Bullet>().SetDirection(direction, projectileSpeed);
            }
        }

        //invlnerability frames
        if (invulnerable && invTimer >= 0.0f)
            invTimer -= Time.deltaTime;
        else
        {
            invulnerable = false;
            invTimer = invTime;
        }

        //player flashes grey Done in animation

        // If not in dash and space is pressed start dash
        if (!active && !onCoolDown)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                active = true;
                activeTimer = activeTime;
            }
            dashMovementX = movement.x;
            dashMovementY = movement.y;
            dashMovement = new Vector2(dashMovementX, dashMovementY);
        }
    }

    private void FixedUpdate()
    {
        // If not actively dashing, move normally
        if (!active)
        {
            playerBody.MovePosition(playerBody.position + movement * moveSpeed * Time.fixedDeltaTime);
        }

        // If dashing
        if (active)
        {
            playerBody.MovePosition(playerBody.position + dashMovement * dashSpeed * Time.fixedDeltaTime);
            Debug.Log("In dash");
            activeTimer -= Time.fixedDeltaTime;
            if (activeTimer <= 0.0f)
            {
                active = false;
                onCoolDown = true;
                coolDownTimer = coolDownTime;
            }
        }
        // If on cooldown, you may not dash
        if (onCoolDown)
        {
            Debug.Log("Dash on cooldown");
            coolDownTimer -= Time.fixedDeltaTime;
            if (coolDownTimer <= 0.0f)
            {
                onCoolDown = false;
                //Debug.Log("Hitbox ready to go again.");
            }
        }
        
    }

    //Player takes damage and becomes invincible when they are hit
    public void TakeDamage(int amount)
    {
        if (!invulnerable && !active)
        {
            invulnerable = true;
            health -= amount;
        }
    }

}