using UnityEngine;

public enum EEnemyType { FODDER }

public enum EBehaviourType { NORMAL, POSSESSED, WAITING }

public class EnemyBase : MonoBehaviour
{
    // Numbers
    public float movementSpeed = 2f;
    public float disablePosX = -20;

    // Components
    [HideInInspector]
    public SpriteRenderer sr;
    private PlayerController player;
    public Color enemyColor;


    // Others
    public EBehaviourType behaviourType;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
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

    void FixedUpdate()
    {
        EnemyBehaviour();
    }

    public virtual void PossessedBehaviour()
    {
        if (sr.flipX)
            sr.flipX = false;

        if (sr.color != player.playerColor)
            sr.color = player.playerColor;

        
    }

    public virtual void EnemyBehaviour()
    {
        sr.color = enemyColor;
        sr.flipX = true;
        transform.position += Vector3.left * movementSpeed * Time.fixedDeltaTime;
    }

    public virtual void WaitingBehaviour()
    {
        sr.flipX = true;
        sr.color = player.waitColor;
    }

    private void Update()
    {
        DisableObject();
        SwitchState();
    }

    public void SetNewBehaviour(EBehaviourType NewBehaviour)
    {
        behaviourType = NewBehaviour;
        player.PossessEnemy(this);
    }

    public void SwitchState()
    {
        switch (behaviourType)
        {
            case EBehaviourType.NORMAL:
                EnemyBehaviour();
                break;
            case EBehaviourType.POSSESSED:
                PossessedBehaviour();
                break;
            case EBehaviourType.WAITING:
                WaitingBehaviour();
                break;
            default:
                break;
        }
    }

    public void DisableObject()
    {
        if (transform.position.x < disablePosX)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
