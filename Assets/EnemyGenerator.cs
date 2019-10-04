using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public Vector3 startPos = Vector3.zero;
    public Vector3 firstStartPos;
    public float moveTime = 5f;
    GameObject lastObject;

    private void Start()
    {
        startPos = transform.position;

        for (int i = 0; i < 2; i++)
        {
            lastObject = EnemyPooler.Instance.GetPooledObject(firstStartPos + Vector3.right * 32 * i);
        }

        StartCoroutine(Generate());
    }

    public IEnumerator Generate()
    {
        while (gameObject.activeInHierarchy)
        {
            if (lastObject != null)
            {
                startPos = lastObject.transform.position + Vector3.right * 32;
            }
            GameObject obj = EnemyPooler.Instance.GetPooledObject(startPos);
            lastObject = obj;
            yield return new WaitForSeconds(moveTime);
        }
    }
}
