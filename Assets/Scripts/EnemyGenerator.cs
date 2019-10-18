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
    public PlayerController player;
    public float playerOffset = 20f;

    private void Update()
    {
        transform.position = new Vector3(player.transform.position.x + playerOffset, transform.position.y, transform.position.z);
    }

    private void OnEnable()
    {
        PlayerController.OnPlaying += PlayerController_OnPlaying;
    }

    private void OnDisable()
    {
        PlayerController.OnPlaying -= PlayerController_OnPlaying;
    }

    private void PlayerController_OnPlaying(PlayerController Player)
    {
        player = Player;
    }

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
                startPos = GameManager.Instance.GetRailPos() + new Vector3(player.transform.position.x + playerOffset, 0, 0);
            }

            MonsterBase obj = EnemyPooler.Instance.GetPooledObject(GameManager.Instance.GetRailPos() + new Vector3(player.transform.position.x + playerOffset, 0, 0));
            lastObject = obj;
            moveTime = Random.Range(moveTimemin, moveTimeMax);
            yield return new WaitForSeconds(moveTime);
        }
    }
}
