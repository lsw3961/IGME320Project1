using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    [SerializeField] int Health = 3;
    [SerializeField] private string[] collisionNames;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        for (int i = 0;i<collisionNames.Length;i++) 
        {
            if (collision.gameObject.tag == collisionNames[i])
            {
                if (collision.gameObject.tag == "Projectile")
                    collision.gameObject.SetActive(false);
                Health--;
                if (Health <= 0) 
                {
                    //play destrction animation
                    //Debug.Log("Hit");
                    //destroy the object
                    this.gameObject.SetActive(false);
                }
                //play hit animation
                return;
            }
        }

    }
}
