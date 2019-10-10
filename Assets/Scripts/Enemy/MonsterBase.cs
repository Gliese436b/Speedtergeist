using UnityEngine;

/// <summary>
/// Types of monsters.
/// </summary>
public enum EMonsterType { GHOST, DEMON, ZOMBIE, BANSHEE }

public enum EBehaviourType { NORMAL, POSSESSED, WAITING }

public class MonsterBase : MonoBehaviour
{
    // Numbers
    public float movementSpeed = 2f;
    public float disablePosX = -20;

    // Components
    public SpriteRenderer sr;
    private PlayerController player;
    public Color enemyColor;

    // Others
    public EBehaviourType behaviourType;
    public EMonsterType monsterType;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        SwitchState();
        DisableObject();
    }

    private void OnEnable()
    {
        SetUpMonster(monsterType);
        PlayerController.OnPlaying += PlayerController_OnPlaying;
    }

    public void SetUpMonster(EMonsterType Monster)
    {
        if (Monster == EMonsterType.GHOST)
        {
            sr.sprite = Resources.Load<Sprite>("Sprites/[PH]Ghost");
        }

        if (Monster == EMonsterType.DEMON)
        {
            sr.sprite = Resources.Load<Sprite>("Sprites/[PH]Demon");
        }

        if (Monster == EMonsterType.ZOMBIE)
        {
            sr.sprite = Resources.Load<Sprite>("Sprites/[PH]Zombie");
        }

        if (Monster == EMonsterType.BANSHEE)
        {
            sr.sprite = Resources.Load<Sprite>("Sprites/[PH]Banshee");
        }
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
    }

    public virtual void PossessedBehaviour()
    {
        if (sr.flipX)
            sr.flipX = false;
    }

    public virtual void EnemyBehaviour()
    {
        sr.color = enemyColor;
        sr.flipX = true;
        transform.position += Vector3.left * movementSpeed * Time.fixedDeltaTime;
    }

    public virtual void WaitingBehaviour()
    {
        if (sr.flipX)
            sr.flipX = false;
    }

    public void SetNewBehaviour(EBehaviourType NewBehaviour)
    {
        behaviourType = NewBehaviour;
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
