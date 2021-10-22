using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] float spawnMax = 2;
    [SerializeField] float spawnMin = .5f;
    private float spawnTime = 2;
    private float spawnCounter = 2;
    [SerializeField] GameObject enemy;

    public void Start()
    {
        spawnTime = Random.Range(spawnMin, spawnMax);
        spawnCounter = spawnTime;
    }
    void FixedUpdate()
    {
        if (spawnCounter <= 0) 
        {
            spawnCounter = spawnTime;
            Instantiate(enemy,this.gameObject.transform.position,Quaternion.identity);
        }
        spawnCounter -= Time.deltaTime;
    }
}
