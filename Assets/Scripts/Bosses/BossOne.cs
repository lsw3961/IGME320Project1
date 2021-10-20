using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private GameObject bullet;
    //Swipe properties
    [SerializeField] private float sweepOffset;
    [SerializeField] private float sweepSize;

    //Fields
    private bool _isAlive = true;
    [SerializeField] private float maxSpeed = 1;
    [SerializeField] private float acceleration = 1;
    [SerializeField] private int maxHealth = 10;
    private int health = 10;

    //AI control variables
    private float stateTime;
    private float stateDuration;
    private bool coolingDown;
    private float cooldownTime;
    private AttackMode state;
    private bool walking = true;

    //Attack management

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

        //If the boss has more than half health, it should not walk by default during attack (unless something like Lunge overrides it).
        // Otherwise, it walks.
        if (maxHealth / health >= 2 && !coolingDown)
            walking = true;
        else
            walking = true;

        if (state != AttackMode.Wander)
            velocity = new Vector2(0.0f, 0.0f);

        Debug.Log(state);
        switch (state)
        {
            case AttackMode.Wander:
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
                    //START SLASH ANIMATION HERE
                    coolingDown = true;
                }
                //TRANSITION into ShSpread, ShStraight, or ShPlus
                if (cooldownTime > 2.5f)
                    ChangeState(new List<AttackMode> { AttackMode.ShootSpread, AttackMode.ShootStraight, AttackMode.ShootPlus });
                break;
            case AttackMode.Lunge:
                if (!coolingDown)
                {
                    //START ANIMATION HERE
                    coolingDown = true;
                }
                //TRANSITION into ShSpread or ShPlus
                if (cooldownTime > 1.2f)
                    ChangeState(new List<AttackMode> { AttackMode.ShootSpread, AttackMode.ShootPlus });
                break;
            case AttackMode.ShootSpread:
                if (!coolingDown)
                {
                    //START ANIMATION HERE
                    coolingDown = true;
                }
                //TRANSITION into Wander (biased 3x) or Slash
                if (cooldownTime > 0.75f)
                    ChangeState(3, new List<AttackMode> { AttackMode.Wander, AttackMode.Slash });
                break;
            case AttackMode.ShootStraight:
                if (!coolingDown)
                {
                    //START ANIMATION HERE
                    coolingDown = true;
                }
                //TRANSITION into ShSpread, ShStraight, ShPlus, or Wander (Yes, it can transition into itself)
                if (cooldownTime > 0.35f)
                    ChangeState(3, new List<AttackMode> { AttackMode.Wander, AttackMode.ShootSpread, AttackMode.ShootStraight, AttackMode.ShootPlus });
                break;
            case AttackMode.ShootPlus:
                if (!coolingDown)
                {
                    //START ANIMATION HERE
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


        //Update properties to reflect actual gamestate
        targetVelocity = player.gameObject.transform.position - transform.position;
        targetVelocity = targetVelocity.normalized * maxSpeed;
        position.x = transform.position.x;
        position.y = transform.position.y;
        if (walking)
        {
            Chase();
            Move();
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
        state = targetState;
        stateTime = 0.0f;
        coolingDown = false;
        cooldownTime = 0;
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
        for(int i = 0; i < possibleStates.Count; i++)
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

    //Moves the position of the boss on the screen based on current velocity and acceleration
    private void Move()
    {
        Debug.Log(velocity);
        //Applies acceleration, normalized to program speed
        velocity += push * Time.deltaTime;

        //Caps speed
        if (velocity.magnitude > maxSpeed)
            velocity /= (velocity.magnitude / maxSpeed);

        //Applies movement, normalized to program speed
        position += velocity * Time.deltaTime;
    }

    //Modifies the push vector to direct the boss towards the player
    private void Chase()
    {
        //Accelerates the enemy to alter its course directly towards the player
        push = targetVelocity - velocity;

        //Caps acceleration
        if (push.magnitude > acceleration)
            push /= (push.magnitude / acceleration);
    }
}
