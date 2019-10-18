using System;
using System.Collections;
using UnityEngine;

public enum EBehaviourType { NORMAL, POSSESSED, WAITING }

public class MonsterBase:MonoBehaviour
{
    // Bools
    public bool isUsingAbility;

    // Numbers
    public float movementSpeed = 2f;
    public float disablePosX = -20;   

    // Components
    public SpriteRenderer sr;
    public Sprite monsterSprite;
    private PlayerController player;
    public Color enemyColor;

    // Others
    public EBehaviourType behaviourType;
    //public EMonsterType monsterType;

    public virtual void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    public virtual void Start()
    {
        sr.sprite = monsterSprite;
    }

    public virtual void Update()
    {
        SwitchState();
        DisableObject();
        PlayerInput();
    }

    private void PlayerInput()
    {        
        if (Input.GetButton("Fire1"))
        {
            isUsingAbility = true;
        }
        else isUsingAbility = false;
    }

    public void MonsterAbility()
    {
        
    }

    private void OnEnable()
    {
        //SetUpMonster(monsterType);
        PlayerController.OnPlaying += PlayerController_OnPlaying;
    }

    public virtual void OnDisable()
    {
        PlayerController.OnPlaying -= PlayerController_OnPlaying;
    }

    public virtual void PlayerController_OnPlaying(PlayerController Player)
    {
        player = Player;
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

    public virtual void SetNewBehaviour(EBehaviourType NewBehaviour)
    {
        behaviourType = NewBehaviour;
    }

    public virtual void SwitchState()
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


    public virtual void DisableObject()
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
