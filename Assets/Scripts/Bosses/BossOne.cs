using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum AttackMode
{
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
        stateTime += Time.deltaTime;
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

        switch (state)
        {
            case AttackMode.Slash:

                //TRANSITION into ShSpread, ShStraight, or ShPlus
                if (cooldownTime > 0.5f)
                    ChangeState(new List<AttackMode> { AttackMode.ShootSpread, AttackMode.ShootStraight, AttackMode.ShootPlus });
                break;
            case AttackMode.Lunge:
                walking = true;

                //TRANSITION into ShSpread or ShPlus
                if (cooldownTime > 2.0f)
                    ChangeState(new List<AttackMode> { AttackMode.ShootSpread, AttackMode.ShootPlus });
                break;
            case AttackMode.ShootSpread:

                //TRANSITION into Slash
                if (cooldownTime > 0.75f)
                    ChangeState(AttackMode.Slash);
                break;
            case AttackMode.ShootStraight:

                //TRANSITION into ShSpread, ShStraight, or ShPlus (Yes, it can transition into itself
                if (cooldownTime > 0.35f)
                    ChangeState(new List<AttackMode> { AttackMode.ShootSpread, AttackMode.ShootStraight, AttackMode.ShootPlus });
                break;
            case AttackMode.ShootPlus:

                //TRANSITION into ShPlus or Lunge
                if (cooldownTime > 1.0f)
                    ChangeState(new List<AttackMode> { AttackMode.ShootSpread, AttackMode.Lunge});
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
        stateTime = 0.0f;
    }

    /// <summary>
    /// Chooses a random state to enter from the given list
    /// </summary>
    /// <param name="possibleStates"></param>
    private void ChangeState(List<AttackMode> possibleStates)
    {
        ChangeState(possibleStates[Random.Range(0, possibleStates.Count)]);
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
