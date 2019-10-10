using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 firstStartPos;
    private float moveTime;
    public float moveTimemin = 1f;
    public float moveTimeMax = 3f;
    MonsterBase lastObject;

    private void Start()
    {  
        StartCoroutine(Generate());
    }

    public IEnumerator Generate()
    {
        while (gameObject.activeInHierarchy)
        {
            if (lastObject != null)
            {
                startPos = GameManager.Instance.GetRailPos();
            }

            MonsterBase obj = EnemyPooler.Instance.GetPooledObject(GameManager.Instance.GetRailPos());
            lastObject = obj;
            moveTime = Random.Range(moveTimemin, moveTimeMax);
            yield return new WaitForSeconds(moveTime);
        }
    }
}
