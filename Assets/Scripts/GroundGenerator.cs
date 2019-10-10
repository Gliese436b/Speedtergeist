using System.Collections;
using UnityEngine;

public class GroundGenerator : MonoBehaviour
{
    public Vector3 startPos = Vector3.zero;
    public Vector3 firstStartPos;
    public float moveTime = 5f;
    public float groundSize = 32f;
    private MoveGround lastObject;

    private void Start()
    {
        startPos = transform.position;        

        for (int i = 0; i < 2; i++)
        {
            lastObject = GroundPooler.Instance.GetPooledObject(firstStartPos + Vector3.right * 32 * i);
            lastObject.speed = groundSize / moveTime;
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
            MoveGround obj = GroundPooler.Instance.GetPooledObject(startPos);
            lastObject = obj;
            obj.speed = groundSize / moveTime;
            yield return new WaitForSeconds(moveTime);
        }
    }
}
