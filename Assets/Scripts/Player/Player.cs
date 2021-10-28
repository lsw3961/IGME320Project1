using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Kenneth Rossi
// 10/6/2021
// Class handles all movement for the Player

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    [SerializeField] Animator anim;
    private Rigidbody2D playerBody;
    [SerializeField] Transform bulletTarget;
    [SerializeField] Transform bulletTarget2;
    [SerializeField] Transform bulletTarget3;


    public float projectileSpeed = 5f;
    public string projectileName;
    private Vector3 direction;
    private float rotateAngle;

    
    private Vector2 movement;
    private float dashMovementX;
    private float dashMovementY;
    private Vector2 dashMovement;

    //invulnerability fields
    // public int health = 10;
    public float invTime = 1.0f;        //time spent invincible
    private float invTimer;             //invincibility timer
    private bool invulnerable = false;  //is player invincible
    private SpriteRenderer sprite;

    //dodge roll/dash fields
    public float dashSpeed = 10f;
    public float coolDownTime = 1.5f;
    private float coolDownTimer;
    private bool onCoolDown = false;
    public float activeTime = 1.5f;
    private float activeTimer;
    private bool active = false;
    //shooting fields
    private bool isShooting = false;
    [SerializeField] float shootingTime = .5f;
    private float shootingCounter = .5f;

    //sound fields
    private AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip hurtSound;
    public AudioClip dashSound;

    public Image[] lives;
    public Sprite emptyHeart;

    // Multihit shot
    private bool multiShot;

    // Start is called before the first frame update
    void Start()
    {
        multiShot = false;
        shootingCounter = shootingTime;
        invTimer = invTime;
        sprite = GetComponent<SpriteRenderer>();
        playerBody = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        anim.SetBool("isHurt", false);

        for (int i = HealthManager.instance.health; i < 10; i++)
        {
            lives[i].GetComponent<Image>().sprite = emptyHeart;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Player Walking Animation
        Walk();
        //Shoot Gun
        Shoot();

        //invlnerability frames
        if (invulnerable && invTimer >= 0.0f)
            invTimer -= Time.deltaTime;
        else
        {
            anim.SetBool("isHurt",false);
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
                PlaySound(dashSound, 1.0f, .85f);
            }
            dashMovementX = movement.x;
            dashMovementY = movement.y;
            dashMovement = new Vector2(dashMovementX, dashMovementY);
        }
    }
    //Walk Method
    public void Walk() 
    {
        // Grab input and store in vector
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize();
        if (movement.x != 0.0f || movement.y != 0.0f)
        {
            anim.SetBool("isWalking", true);
        }
        else 
        {
            anim.SetBool("isWalking", false);
        }
        // Rotate player to mouse position
        direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        direction.Normalize();
        rotateAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.Euler(0f, 0f, rotateAngle);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

    }
    //this is just a method to keep track of shooting and to make it automatic
    private void Shoot() 
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            isShooting = true;
            PlaySound(shootSound);
            GameObject temp = ObjectPooler.SharedInstance.GetPooledObject(projectileName);
            if (temp != null)
            {
                temp.transform.position = bulletTarget.position;
                //temp.transform.rotation = this.gameObject.transform.rotation;
                temp.GetComponent<Bullet>().SetDirection(direction, projectileSpeed);
            }
            if (multiShot)
            {
                multiShot = false;
                GameObject temp2 = ObjectPooler.SharedInstance.GetPooledObject(projectileName);
                if (temp2 != null)
                {
                    temp2.transform.position = bulletTarget2.position;
                    //temp.transform.rotation = this.gameObject.transform.rotation;
                    temp2.GetComponent<Bullet>().SetDirection(direction, projectileSpeed);
                }
                GameObject temp3 = ObjectPooler.SharedInstance.GetPooledObject(projectileName);
                if (temp3 != null)
                {
                    temp3.transform.position = bulletTarget3.position;
                    //temp.transform.rotation = this.gameObject.transform.rotation;
                    temp3.GetComponent<Bullet>().SetDirection(direction, projectileSpeed);
                }
            }
            return;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            isShooting = false;
        }
        if (isShooting) 
        {
            if (shootingCounter <= 0) 
            {
                shootingCounter = shootingTime;
                PlaySound(shootSound);
                GameObject temp = ObjectPooler.SharedInstance.GetPooledObject(projectileName);
                if (temp != null)
                {
                    temp.transform.position = bulletTarget.position;
                    //temp.transform.rotation = this.gameObject.transform.rotation;
                    temp.GetComponent<Bullet>().SetDirection(direction, projectileSpeed);
                }
                if (multiShot)
                {
                    multiShot = false;
                    GameObject temp2 = ObjectPooler.SharedInstance.GetPooledObject(projectileName);
                    if (temp2 != null)
                    {
                        temp2.transform.position = bulletTarget2.position;
                        //temp.transform.rotation = this.gameObject.transform.rotation;
                        temp2.GetComponent<Bullet>().SetDirection(direction, projectileSpeed);
                    }
                    GameObject temp3 = ObjectPooler.SharedInstance.GetPooledObject(projectileName);
                    if (temp3 != null)
                    {
                        temp3.transform.position = bulletTarget3.position;
                        //temp.transform.rotation = this.gameObject.transform.rotation;
                        temp3.GetComponent<Bullet>().SetDirection(direction, projectileSpeed);
                    }
                }
            }
            shootingCounter -= Time.deltaTime;
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
            //Debug.Log("In dash");
            activeTimer -= Time.fixedDeltaTime;
            if (activeTimer <= 0.0f)
            {
                multiShot = true;
                active = false;
                onCoolDown = true;
                coolDownTimer = coolDownTime;
            }
        }
        // If on cooldown, you may not dash
        if (onCoolDown)
        {
            //Debug.Log("Dash on cooldown");
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
            if (HealthManager.instance.health > 1)
            {
                lives[HealthManager.instance.health - 1].GetComponent<Image>().sprite = emptyHeart;
            }
            anim.SetBool("isHurt", true);
            invulnerable = true;
            HealthManager.instance.health -= amount;
            PlaySound(hurtSound, 1.0f, .9f);
        }

        if (HealthManager.instance.health <= 0)
        {
            SceneManager.LoadScene("Game Over");
        }
    }

    //Play an audio clip (sound effects only)
    public void PlaySound(AudioClip audioClip, float volume = 1.0f, float pitch = 1.0f)
    {
        audioSource.volume = volume;
        audioSource.pitch = pitch;

        if(audioClip != null)
            audioSource.PlayOneShot(audioClip);
    }
}