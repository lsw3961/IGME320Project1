using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Michael Chan

enum AttackMode
{
    Wander,
    Slash,
    Lunge,
    ShootStraight,
    ShootSpread,
    ShootPlus
}

public class BossOne : MonoBehaviour
{
    //Player to target
    [SerializeField] private GameObject player;
    //Attack prefabs
    [SerializeField] private GameObject sweep;
    [SerializeField] private GameObject moonBullet;
    //Attack properties
    [SerializeField] private float sweepOffset;
    [SerializeField] private float sweepSize;
    [SerializeField] private float bulletSpeed;

    //Fields
    private bool _isAlive = true;
    [SerializeField] private float maxSpeed = 1;
    [SerializeField] private float acceleration = 1;
    [SerializeField] private int maxHealth = 10;
    private int health = 10;
    private Transform parent;

    //AI control variables
    private float stateTime;
    private float stateDuration;
    private bool coolingDown;
    private float cooldownTime;
    private AttackMode state;
    private bool walking = true;

    //Attack management
    private Vector2 aimDirection;
    private Animator animController;
    [SerializeField] private float lungeVelocity;
    private Vector2 lungeDirection;

    //Vectors used in calculating movement
    private Vector2 position;
    private Vector2 velocity;
    private Vector2 push;
    private Vector2 targetVelocity;

    //Properties
    public bool IsAlive
    { get { return _isAlive; } }

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        targetVelocity = new Vector2(player.gameObject.transform.position.x, player.gameObject.transform.position.y);
        position = new Vector2(transform.position.x, transform.position.y);
        parent = transform.parent;
        animController = GetComponentInParent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Tracks the time a state has been active
        stateTime += Time.deltaTime;

        //If an animation is cooling down, add time so it knows when to transition
        if (coolingDown)
            cooldownTime += Time.deltaTime;

        //Mark dead if dead
        if (health <= 0)
            _isAlive = false;
        //Skip if dead
        if (!_isAlive)
            return;

        //Update the aim direction to be a unit vector directly towards the player
        aimDirection = new Vector2(player.gameObject.transform.position.x - position.x, player.gameObject.transform.position.y - position.y).normalized;

        //Update properties to reflect actual gamestate
        targetVelocity = player.gameObject.transform.position - transform.position;
        targetVelocity = targetVelocity.normalized * maxSpeed;
        //parent.position

        /*
        //If the boss has more than half health, it should not walk by default during attack (unless something like Lunge overrides it).
        // Otherwise, it walks.
        if (maxHealth / health >= 2 && !coolingDown)
            walking = true;
        else
            walking = true;
        */

        //Freezes the boss if it is doing something besides walking around
        if (state != AttackMode.Wander)
            velocity = new Vector2(0.0f, 0.0f);


        switch (state)
        {
            case AttackMode.Wander:
                animController.enabled = false;
                if (!coolingDown)
                {
                    stateDuration = Mathf.Pow(Random.Range(1.0f, 1.5f), 2) + 2;
                    coolingDown = true;
                }
                Chase();
                Move();
                //TRANSITION into Slash, Lunge, or ShStraight (uses a different detection due to flexible state duration
                if (stateTime > stateDuration)
                    ChangeState(new List<AttackMode> { AttackMode.Slash, AttackMode.Lunge, AttackMode.ShootStraight });
                break;
            case AttackMode.Slash:
                if (!coolingDown)
                {
                    animController.enabled = true;
                    animController.Play("Slash");
                    coolingDown = true;
                }
                //TRANSITION into ShSpread, ShStraight, or ShPlus
                if (cooldownTime > 2.5f)
                    ChangeState(new List<AttackMode> { AttackMode.ShootSpread, AttackMode.ShootStraight, AttackMode.ShootPlus });
                break;
            case AttackMode.Lunge:
                if (!coolingDown)
                {

                    animController.enabled = true;
                    animController.Play("Lunge");
                    lungeDirection = aimDirection;
                    coolingDown = true;
                }
                position += lungeVelocity * lungeDirection * Time.deltaTime;
                //TRANSITION into ShSpread or ShPlus

                //Try to read the length of the clip and use that as a duration
                foreach (AnimationClip anim in animController.runtimeAnimatorController.animationClips)
                {
                    if (anim.name == "Lunge")
                        stateDuration = anim.length;
                }
                if (cooldownTime > stateDuration)
                    ChangeState(new List<AttackMode> { AttackMode.ShootSpread, AttackMode.ShootPlus });
                break;
            case AttackMode.ShootSpread:
                animController.enabled = false;
                if (!coolingDown)
                {
                    //START ANIMATION HERE

                    //Shoot three bullets in a cone at the player
                    Shoot(aimDirection);
                    //These are matrix rotations
                    Shoot(new Vector2(
                        aimDirection.x * Mathf.Cos(Mathf.PI / 9.0f) - aimDirection.y * Mathf.Sin(Mathf.PI / 9.0f),
                        aimDirection.x * Mathf.Sin(Mathf.PI / 9.0f) + aimDirection.y * Mathf.Cos(Mathf.PI / 9.0f)
                        ));
                    Shoot(new Vector2(
                        aimDirection.x * Mathf.Cos(-Mathf.PI / 9.0f) - aimDirection.y * Mathf.Sin(-Mathf.PI / 9.0f),
                        aimDirection.x * Mathf.Sin(-Mathf.PI / 9.0f) + aimDirection.y * Mathf.Cos(-Mathf.PI / 9.0f)
                        ));
                    coolingDown = true;
                }
                //TRANSITION into Wander (biased 3x) or Slash
                if (cooldownTime > 0.75f)
                    ChangeState(3, new List<AttackMode> { AttackMode.Wander, AttackMode.Slash });
                break;
            case AttackMode.ShootStraight:
                animController.enabled = false;
                if (!coolingDown)
                {
                    //START ANIMATION HERE
                    //Shoot a bullet directly at the player
                    Shoot(aimDirection);
                    coolingDown = true;
                }
                //TRANSITION into ShSpread, ShStraight, ShPlus, or Wander (Yes, it can transition into itself)
                if (cooldownTime > 0.35f)
                    ChangeState(3, new List<AttackMode> { AttackMode.Wander, AttackMode.ShootSpread, AttackMode.ShootStraight, AttackMode.ShootPlus });
                break;
            case AttackMode.ShootPlus:
                animController.enabled = false;
                if (!coolingDown)
                {
                    //START ANIMATION HERE
                    //Shoot a moon in all 4 directions
                    Shoot(Vector3.up);
                    Shoot(Vector3.down);
                    Shoot(Vector3.left);
                    Shoot(Vector3.right);
                    coolingDown = true;
                }
                //TRANSITION into Wander (biased 3x), ShPlus, or Lunge
                if (cooldownTime > 2.0f)
                    ChangeState(3, new List<AttackMode> { AttackMode.Wander, AttackMode.ShootPlus, AttackMode.Lunge });
                break;
            default:
                state = AttackMode.Slash;
                break;
        }

        //Set actual position to calculated position
        transform.position = new Vector3(position.x, position.y, 0);
    }

    /// <summary>
    /// Switches the boss state to the selected state
    /// </summary>
    /// <param name="targetState"></param>
    private void ChangeState(AttackMode targetState)
    {
        //Save the distance between the boss and its container
        Vector2 offset = transform.position - transform.parent.position;
        //Move the container to the boss's position (while zeroing out the Z position)
        //This also moves the boss the same amount, but the second line fixes that
        transform.parent.position = new Vector3(transform.position.x, transform.position.y, 0.0f);
        transform.position -= new Vector3(offset.x, offset.y, transform.position.z);

        state = targetState;
        stateTime = 0.0f;
        coolingDown = false;
        cooldownTime = 0;
        Debug.Log(state);
    }

    /// <summary>
    /// Chooses a random state to enter from the given list
    /// </summary>
    /// <param name="possibleStates"></param>
    private void ChangeState(List<AttackMode> possibleStates)
    {
        ChangeState(possibleStates[Random.Range(0, possibleStates.Count)]);
    }

    /// <summary>
    /// Chooses a random state to enter from the given list
    /// </summary>
    /// <param name="firstBias">Integer multiplier for how much more likely the first option is to be chosen</param>
    /// <param name="possibleStates"></param>
    private void ChangeState(int firstBias, List<AttackMode> possibleStates)
    {
        List<AttackMode> biasedList = new List<AttackMode>();
        for (int i = 0; i < possibleStates.Count; i++)
        {
            //Adds the first entry as many times as the bias calls for
            if (i == 0)
            {
                for (int j = 0; j < firstBias; j++)
                    biasedList.Add(possibleStates[0]);
            }
            //Adds the rest of the entries from the original list
            else
                biasedList.Add(possibleStates[i]);
        }
        ChangeState(biasedList);
    }

    /// <summary>
    /// 
    /// </summary>
    private void Shoot(Vector2 direction)
    {
        GameObject shotBullet = GameObject.Instantiate(moonBullet);
        shotBullet.transform.position = position;
        if (shotBullet.GetComponent<AttackBullet>() != null)
            shotBullet.GetComponent<AttackBullet>().SetVelocity(direction, bulletSpeed);
        else
            Destroy(shotBullet);
    }

    /// <summary>
    /// Overloaded shoot method that takes a Vector3 and ignores the Z component
    /// </summary>
    private void Shoot(Vector3 direction)
    {
        Shoot(new Vector2(direction.x, direction.y));
    }

    /// <summary>
    /// Moves the position of the boss on the screen based on current velocity and acceleration
    /// </summary>
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

    /// <summary>
    /// Modifies the push vector to direct the boss towards the player
    /// </summary>
    private void Chase()
    {
        //Accelerates the enemy to alter its course directly towards the player
        push = targetVelocity - velocity;

        //Caps acceleration
        if (push.magnitude > acceleration)
            push /= (push.magnitude / acceleration);
    }
}
