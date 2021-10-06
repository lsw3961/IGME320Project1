using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Vector2 direction = Vector2.zero;
    private float projectileSpeed = 0.0f;
    private void OnEnable()
    {
            this.gameObject.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
    }

    public void SetDirection(Vector2 dir, float speed) 
    {
        direction = dir;
        projectileSpeed = speed;
        this.gameObject.SetActive(true);

    }
}
