using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBullet : BossAttack
{
    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (Mathf.Abs(transform.position.x) + Mathf.Abs(transform.position.y) > 20)
        {
            Destroy(this.gameObject);
        }
    }
}
