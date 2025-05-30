using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour
{

    [SerializeField]
    protected GameObject prefab;

    [SerializeField]
    int poolSize;

    public List<T> pool { get; private set; }

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
                pool[i].gameObject.SetActive(true);
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

    public List<T> GetObjectsFromPool(int count)
{
    List<T> objects = new List<T>(count);

    for (int i = 0; i < count; i++)
    {
        T obj = GetObjectFromPool();
        if (obj != null)
        {
            objects.Add(obj);
        }
        else
        {
            Debug.LogError("Failed to retrieve object from pool.");
        }
    }

    return objects;
}

    public void ReturnObjectToPool(T objectToReturn)
    {
        if (objectToReturn == null)
        {
            Debug.LogError("Object is null");
            return;
        }

        objectToReturn.gameObject.SetActive(false);
    }

    public void ClearPool()
    {
        foreach (T item in pool)
        {
            if (item != null)
            {
                ReturnObjectToPool(item);
            }
        }
        pool.Clear(); // Clear the pool list
        poolSize = 0;
    }
}