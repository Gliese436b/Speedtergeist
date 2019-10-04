using System.Collections.Generic;
using UnityEngine;

public class GroundPooler : MonoBehaviour
{
    public static GroundPooler Instance;
    public GameObject prefab;
    public HideFlags flag = HideFlags.HideInHierarchy;
    private List<GameObject> pooledObjects = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetPooledObject()
    {
        return GetPooledObject(transform.position);
    }

    public GameObject GetPooledObject(Vector3 Position)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                pooledObjects[i].transform.position = Position;
                pooledObjects[i].SetActive(true);
                return pooledObjects[i];
            }
        }

        GameObject tmpObj = Instantiate(prefab, Position, Quaternion.identity);
        tmpObj.hideFlags = flag;
        pooledObjects.Add(tmpObj);        
        return tmpObj;
    }
}
