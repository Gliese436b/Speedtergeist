using System.Collections;
using UnityEngine;

public class PlayerController:MonoBehaviour
{
    public delegate void FPlayerEnable(PlayerController Player);
    public static event FPlayerEnable OnPlaying;

    // Components
    private SpriteRenderer sr;
    public Color playerColor;
    public Color waitColor;
    public MonsterBase currentMonster;
    public MonsterBase savedMonster;
    public Transform enemyParent;
    public Transform waitParent;

    // Bools
    public bool isMoving;
    private bool bSwitchEnemy;
    public bool isUsingAbility;

    // Numbers
    public float vertDistanceToMove = 1f;
    public float moveSpeedMultiplier = 5f;
    private float vertAxis;
    public int railIndex = 1;
    public Vector3[] rails = new Vector3[3];
    private Vector3 spriteStartPos;

    // Others

    private void Start()
    {
        OnPlaying?.Invoke(this);
        Initialize();
    }

    private void Initialize()
    {
        // Initialize rails using the values from GameManager   
        rails[0] = new Vector3(transform.position.x, GameManager.Instance.rails[0].y, transform.position.z);
        rails[1] = new Vector3(transform.position.x, GameManager.Instance.rails[1].y, transform.position.z);
        rails[2] = new Vector3(transform.position.x, GameManager.Instance.rails[2].y, transform.position.z);

        // Initialize ghost
        var ghost = GameManager.Instance.GenerateGhost();
        ghost.monsterType = EMonsterType.GHOST;
        currentMonster = ghost;
        PossessEnemy(ghost);
    }

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Update()
    {
        PlayerInput();
        if (bSwitchEnemy) SwitchCurrentMonster(currentMonster, savedMonster);
    }

    private void SwitchCurrentMonster(MonsterBase Monster)
    {
        Debug.Log("Saving current monster named " + currentMonster.name);
        savedMonster = currentMonster;
        Monster.SetNewBehaviour(EBehaviourType.WAITING);
        Monster.transform.SetParent(waitParent);
        Monster.transform.position = waitParent.position;
        Monster.sr.color = waitColor;
    }

    private void SwitchCurrentMonster(MonsterBase MonsterToSave, MonsterBase MonsterToUse)
    {
        Debug.Log("Switching current monsters " + MonsterToSave.name + " and " + MonsterToUse.name);

        savedMonster = MonsterToSave;
        MonsterToSave.SetNewBehaviour(EBehaviourType.WAITING);
        MonsterToSave.transform.SetParent(waitParent);
        MonsterToSave.transform.position = waitParent.position;
        MonsterToSave.sr.color = waitColor;

        currentMonster = MonsterToUse;
        MonsterToUse.SetNewBehaviour(EBehaviourType.POSSESSED);
        MonsterToUse.transform.SetParent(enemyParent);
        MonsterToUse.transform.position = enemyParent.position;
        MonsterToUse.sr.color = playerColor;
    }


    public void PossessEnemy(MonsterBase Monster)
    {
        if (currentMonster == null) return;
        Debug.Log("Changing current monster named " + currentMonster.name);
        Monster.SetNewBehaviour(EBehaviourType.POSSESSED);
        Monster.transform.SetParent(enemyParent);
        Monster.transform.position = enemyParent.position;
        Monster.sr.color = playerColor;
    }

    public void PlayerInput()
    {
        if (Input.GetButton("Fire1"))
        {
            isUsingAbility = true;
        }
        else isUsingAbility = false;

        if (Input.GetButtonDown("Fire2"))
        {
            bSwitchEnemy = true;
        }
        else bSwitchEnemy = false;

        vertAxis = Input.GetAxisRaw("Vertical");
    }

    /// <summary>
    /// Funcion que le permite moverse al jugador en el eje X.
    /// </summary>
    private void Movement()
    {
        if (isMoving)
            return;

        Vector3 distToMove = new Vector3(0, vertDistanceToMove, 0);

        if (vertAxis > 0)
        {
            railIndex++;
            railIndex = Mathf.Clamp(railIndex, 0, 2);
            StartCoroutine(MoveRoutine(railIndex));
        }
        else if (vertAxis < 0)
        {
            railIndex--;
            railIndex = Mathf.Clamp(railIndex, 0, 2);
            StartCoroutine(MoveRoutine(railIndex));
        }
    }

    public IEnumerator MoveRoutine(int Index)
    {
        isMoving = true;

        var posInicial = transform.position;
        Vector3 endPos = rails[Index];

        while (transform.position != endPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPos, moveSpeedMultiplier * Time.deltaTime);
            if (transform.position != endPos)
                yield return null;
        }

        isMoving = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (savedMonster == null && isUsingAbility && collision.CompareTag("Enemy"))
        {
            if (currentMonster != null)
            {
                SwitchCurrentMonster(currentMonster);
            }

            print("Collided with enemy named: " + collision.name);
            currentMonster = collision.GetComponent<MonsterBase>();
            PossessEnemy(currentMonster);
        }
    }
}
