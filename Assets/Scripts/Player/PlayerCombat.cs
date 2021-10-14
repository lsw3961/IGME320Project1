using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public BoxCollider2D swordCollider;
    public SpriteRenderer testHitbox;
    public float coolDownTime = 1.5f;
    public float coolDownTimer;
    private bool onCoolDown = false;
    public float activeTime = 1.5f;
    public float activeTimer;
    private bool active = false;
    // Start is called before the first frame update
    void Start()
    {
        swordCollider = GetComponent<BoxCollider2D>();
        swordCollider.enabled = false;
        testHitbox = GetComponent<SpriteRenderer>();
        testHitbox.enabled = false;
    }

    void Update()
    {
        if (onCoolDown == false && active == false)
        {
            Debug.Log("Hello");
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                swordCollider.enabled = true;
                testHitbox.enabled = true;
                active = true;
                activeTimer = activeTime;
            }
        }

        if (active)
        {
            Debug.Log("HitBox active");
            activeTimer -= Time.deltaTime;
            if (activeTimer <= 0.0f)
            {
                swordCollider.enabled = false;
                testHitbox.enabled = false;
                active = false;
                onCoolDown = true;
                coolDownTimer = coolDownTime;
            }
        }

        if (onCoolDown)
        {
            Debug.Log("HitBox on cooldown");
            coolDownTimer -= Time.deltaTime;
            if (coolDownTimer <= 0.0f)
            {
                onCoolDown = false;
                Debug.Log("Hitbox ready to go again.");
            }
        }

    }

}
