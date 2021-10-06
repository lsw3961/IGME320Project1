using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    [SerializeField] private string[] collisionNames;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        for (int i = 0;i<collisionNames.Length;i++) 
        {
            if (collision.gameObject.tag == collisionNames[i])
            {
                if (collision.gameObject.tag == "Projectile")
                    collision.gameObject.SetActive(false);

                //play destrction animation
                Debug.Log("Hit");
                //destroy the object
                Destroy(this.gameObject);
                return;
            }
        }

    }
}
