using System.Collections.Generic;
using UnityEngine;

public class GroundPooler : Singleton<GroundPooler>
{
    public MoveGround prefab;
    public HideFlags flag = HideFlags.HideInHierarchy;
    private List<MoveGround> pooledObjects = new List<MoveGround>();

    public MoveGround GetPooledObject()
    {
        return GetPooledObject(transform.position);
    }

    public MoveGround GetPooledObject(Vector3 Position)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].gameObject.activeInHierarchy)
            {
                pooledObjects[i].transform.position = Position;
                pooledObjects[i].gameObject.SetActive(true);
                return pooledObjects[i];
            }
        }

        MoveGround tmpObj = Instantiate(prefab, Position, Quaternion.identity);
        tmpObj.hideFlags = flag;
        pooledObjects.Add(tmpObj);        
        return tmpObj;
    }
}
