using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour
{

    [SerializeField]
    protected GameObject prefab;

    [SerializeField]
    int poolSize;

    List<T> pool;

    public void CreatePool(int amount = 0)
    {
        //Check for invalid pool size
        if (amount < 0)
        {
            Debug.LogError("Invalid pool size");
        }

        //Set the pool size
        this.poolSize = amount;   

        //Create a new list of objects
        pool = new List<T>(amount);

        //Create the pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject poolElement = Instantiate(prefab, transform);
            pool.Add(poolElement.GetComponent<T>());
            
            //Deactivate the object
            poolElement.SetActive(false);
        }
    }

    public T GetObjectFromPool()
    {
        //Look for an inactive (ready) object in the pool
        for (int i = 0; i < poolSize; i++)
        {
            if (!pool[i].gameObject.activeInHierarchy)
            {
                return pool[i].GetComponent<T>();
            }
        }
        
        //If there are no inactive objects create a new one, add it to the pool and return it
        GameObject poolElement = Instantiate(prefab, transform);
        pool.Add(poolElement.GetComponent<T>());
        poolSize++;
        poolElement.SetActive(true);
        return poolElement.GetComponent<T>();
    }

    public void ReturnObjectToPool(T objectToReturn)
    {
        if(objectToReturn == null)
        {
            Debug.LogError("Object is null");
            return;
        }

        objectToReturn.gameObject.SetActive(false);
    }
}