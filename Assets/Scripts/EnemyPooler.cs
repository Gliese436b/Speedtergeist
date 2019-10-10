using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyPooler : Singleton<EnemyPooler>
{
    public MonsterBase prefab;
    public HideFlags flag = HideFlags.HideInHierarchy;
    private List<MonsterBase> pooledObjects = new List<MonsterBase>();

    public MonsterBase GetPooledObject()
    {
        return GetPooledObject(transform.position);
    }

    public MonsterBase GetPooledObject(Vector3 Position)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].gameObject.activeInHierarchy)
            {
                pooledObjects[i].transform.position = Position;
                pooledObjects[i].gameObject.SetActive(true);
                // In the cast number inside the parameter of Random.Range, + 1 is added since Linq.Max returns 1 less than the actual amount of entries.
                pooledObjects[i].monsterType = (EMonsterType)Random.Range(1, (int)System.Enum.GetValues(typeof(EMonsterType)).Cast<EMonsterType>().Max() + 1);
                pooledObjects[i].SetUpMonster(pooledObjects[i].monsterType);
                return pooledObjects[i];
            }
        }

        MonsterBase tmpObj = Instantiate(prefab, Position, Quaternion.identity);
        var aux = (EMonsterType)Random.Range(1, (int)System.Enum.GetValues(typeof(EMonsterType)).Cast<EMonsterType>().Max());
        tmpObj.monsterType = aux;
        tmpObj.SetUpMonster(aux);
        tmpObj.hideFlags = flag;
        pooledObjects.Add(tmpObj);
        return tmpObj;
    }
}
