using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author: Andrew Bradbury
/// Purpose: Pool objects to reduce Instantiate and Destroy calls during runtime
/// Notes: Uses code from https://www.raywenderlich.com/847-object-pooling-in-unity
/// </summary>

[System.Serializable]
public class ObjectPoolItem
{
    public int poolAmount;                      //How many items should be in the pool?
    public GameObject objectToPool;             //What is being pooled?
    public bool expandable = true;            //Can this pool exceed the poolAmount?
    public GameObject parentObject;             //Parent container (hierarchy organization)
}
public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler SharedInstance;  //Reference for other scripts
    private List<GameObject> pooledObjects;     //"Pool" of objects
    public List<ObjectPoolItem> objectsToPool;  //What objects are pooled?

    private void Awake()
    {
        SharedInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //make the pool and populate with selected object
        pooledObjects = new List<GameObject>();
        foreach (ObjectPoolItem item in objectsToPool)
        {
            for (int i = 0; i < item.poolAmount; i++)
            {
                GameObject obj;

                //Child to parent if a parent object is given
                if (item.parentObject != null) { obj = Instantiate(item.objectToPool, item.parentObject.transform); }
                else { obj = Instantiate(item.objectToPool); }

                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    //PURPOSE: Retrieve a game object from the object pool
    //RETURNS: The first inactive game object in the pool (if there is one)
    public GameObject GetPooledObject(string tag)
    {
        //find first inactive object with matching tag
        for(int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag)
                return pooledObjects[i];
        }

        //if all objects are active, find a "poolable" object of the same type and add it
        foreach (ObjectPoolItem item in objectsToPool) {
            if (item.objectToPool.tag == tag)
            {
                if (item.expandable)
                {
                    GameObject obj;

                    //Child to parent if a parent object is given
                    if (item.parentObject != null) { obj = Instantiate(item.objectToPool, item.parentObject.transform); }
                    else { obj = Instantiate(item.objectToPool); }

                    obj.SetActive(false);
                    pooledObjects.Add(obj);
                    return obj;
                }
            }
        }
        return null;
    }

    /* **PLACE THIS CODE WHEREVER A POOLED OBJECT IS SPAWNED**
    GameObject [OBJECT] = ObjectPooler.SharedInstance.GetPooledObject("[OBJECT TAG]"); 
    if ([OBJECT] != null) {
        [OBJECT].transform.position = [SPAWN POSITION];
        [OBJECT].transform.rotation = [SPAWN POSITION];
        [OBJECT].SetActive(true);
    }

    NEED OTHER CODE FOR SETTING ACTIVE TO FALSE WHEN OBJECTS WOULD BE DESTROYED
    MAKE SURE POOLED OBJECTS ARE PROPERLY INSTANTIATED USING OnEnable(), NOT Start()!
    */
}
