using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Michael Chan


public class AttackBullet : BossAttack
{
    private Vector2 travel;
    private float timeAlive;

    // Start is called before the first frame update
    protected override void Start()
    {
        timeAlive = 0;
    }

    // Update is called once per frame
    protected override void Update()
    {
        //Move the projectile
        gameObject.transform.position += new Vector3(travel.x, travel.y, 0.0f) * Time.deltaTime;

        //Spin the projectile around
        transform.rotation = transform.rotation * Quaternion.AngleAxis(1.0f, Vector3.forward);

        timeAlive += Time.deltaTime;
        //Destroy bullet if it has been on screen for long enough
        if (timeAlive > 5.0f)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    public void SetVelocity(Vector2 direction, float speed)
    {
        travel = direction.normalized * speed;
    }
}
