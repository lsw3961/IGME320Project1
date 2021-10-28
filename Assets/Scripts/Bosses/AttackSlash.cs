using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSlash : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //If the slash hits the player, damage the player
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Slash touched player");
            //If the player is not invulnerable, damage them
            if (!other.gameObject.GetComponent<Player>().IsInvulnerable)
                other.gameObject.GetComponent<Player>().TakeDamage(1);
        }
    }
}
