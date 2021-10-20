using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Vector2 direction = Vector2.zero;
    private float projectileSpeed = 0.0f;

    //out of bounds vars
    private Camera mainCamera;
    private float screenHeight;
    private float screenWidth;
    private float xOffset;
    private float yOffset;

    public float bulletLife = 1.0f;
    private float bulletLifeTimer;

    private void Awake()
    {
        mainCamera = Camera.main;
        screenHeight = mainCamera.orthographicSize;
        screenWidth = screenHeight * mainCamera.aspect;
        xOffset = transform.lossyScale.x;
        yOffset = transform.lossyScale.y;
    }

    private void OnEnable()
    {
        //only set velocity when bullet is enabled
        gameObject.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
        bulletLifeTimer = bulletLife;
    }

    
    void Update()
    {
        //OutOfBounds();

        //bullets disappear after certain time
        if (bulletLifeTimer > 0.0f)
            bulletLifeTimer -= Time.deltaTime;
        else
            gameObject.SetActive(false);
    }

    public void SetDirection(Vector2 dir, float speed) 
    {
        direction = dir;
        projectileSpeed = speed;
        gameObject.SetActive(true);

    }

    private void OutOfBounds()
    {
        //make bullets disappear once they leave the screen
        if (transform.position.x > screenWidth + xOffset || transform.position.x < -screenWidth - xOffset
            || transform.position.y > screenHeight + yOffset || transform.position.y < -screenHeight - yOffset)
        {
            gameObject.SetActive(false);
        }
    }
}
