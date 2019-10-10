using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Vector3[] rails = new Vector3[3];
    public int railIndex = 1;
    public MonsterBase ghostPrefab;
    
    public Vector3 GetRailPos()
    {
        railIndex = Random.Range(0, rails.Length);
        Vector3 tmp;
        tmp = new Vector3 (rails[railIndex].x, rails[railIndex].y, rails[railIndex].z);
        //print(tmp);
        return tmp;
    }

    public MonsterBase GenerateGhost()
    {
        // Initialize ghost
        MonsterBase ghostObject = Instantiate(ghostPrefab, transform);
        return ghostObject;
    }
}
