using System.Collections;
using UnityEngine;

public class GroundGenerator:MonoBehaviour
{
    public Vector3 startPos = Vector3.zero;
    public Vector3 firstStartPos;
    public float moveTime = 5f;
    public float groundSize = 32f;
    private MoveGround lastObject;
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
        startPos = transform.position;

        for (int i = 0 ; i < 2 ; i++)
        {
            lastObject = GroundPooler.Instance.GetPooledObject(firstStartPos + Vector3.right * 32 * i);
            lastObject.speed = groundSize / moveTime;
            lastObject.disablePosX = player.transform.position.x - playerOffset;
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
            obj.disablePosX = player.transform.position.x - playerOffset;
            yield return new WaitForSeconds(moveTime);
        }
    }
}
