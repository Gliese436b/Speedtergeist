using System.Collections;
using UnityEngine;

public class PlayerController:MonoBehaviour
{
    public delegate void FPlayerEnable(PlayerController Player);
    public static event FPlayerEnable OnPlaying;

    [Header("Movement Settings")]  
    public float vertDistanceToMove = 1f;
    public float moveSpeedMultiplier = 5f;
    private float vertAxis;
    private int railIndex = 1;
    private Vector3[] rails = new Vector3[3];

    [Header("Dash Settings")]
    public float dashDistance = 5f;
    public float dashSpeedMult = 5f;
    public float recoverSpeedMult = 5f;
    public float dashSpeed;
    public float dashSpeedMax = 5f;
    public float recoverSpeed;
    public float recoverSpeedMax = 1.5f;

    [Header("Dash Smoothing Settings")]
    public float smoothTime = 0;
    public float smoothTimeFinal = 1.5f;
    public float smoothTimeRecover = 0;
    public float smoothTimeRecoverFinal = 1.5f;

    // Components
    [Header("Sprite Settings")]
    public Color playerColor;
    public Color waitColor;
    private SpriteRenderer sr;

    [Header("Monster Settings")]
    public MonsterBase currentMonster;
    public MonsterBase savedMonster;
    public Transform enemyParent;
    public Transform waitParent;

    // Bools
    private bool isMoving;
    private bool bSwitchEnemy;
    private bool isUsingAbility;    
    private bool isDashing;

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
        //ghost.monsterType = EMonsterType.GHOST;
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
        if (isUsingAbility) StartCoroutine(Dash());
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

    Vector3 startPos;
    Vector3 target;

    public IEnumerator Dash()
    {
        if (isDashing) yield break;
        isDashing = true;

        startPos = transform.position;
        target = new Vector3(transform.position.x + dashDistance, transform.position.y, transform.position.z);    
        
        while (transform.position != target)
        {     
            // dashSpeed is gradually increased by adding engine frame time and dashSpeedMult, to have a smooth acceleration instead of a stiff one.
            dashSpeed += Time.fixedDeltaTime * dashSpeedMult;
            // If dashSpeed is the same as the max or above it, keep it in the proper range.
            dashSpeed = Mathf.Clamp(dashSpeed, 0, dashSpeedMax);
            // Lerp the target vector to smooth the end of the dash.
            smoothTime += Time.deltaTime;
            // Clamp the lerp time
            smoothTime = Mathf.Clamp(smoothTime, 0, smoothTimeFinal);
            // Divide the time over the target time each frame to get a "gradual change"
            float t = smoothTime / smoothTimeFinal;
            // Create a vector where the lerping will happen using player's transform, target vector and t.
            Vector3 lerpTarget = Vector3.Lerp(transform.position, target, t);

            transform.position = Vector3.MoveTowards(transform.position, lerpTarget, dashSpeed * Time.fixedDeltaTime);
            if (transform.position != target)
            {                
                yield return null;
            }
        }

        while (transform.position != startPos)
        {
            // Same as above
            recoverSpeed += Time.fixedDeltaTime * recoverSpeedMult;
            recoverSpeed = Mathf.Clamp(recoverSpeed, 0, recoverSpeedMax);

            smoothTimeRecover  += Time.deltaTime;
            smoothTimeRecover = Mathf.Clamp(smoothTimeRecover, 0, smoothTimeRecoverFinal);

            float t = smoothTimeRecover / smoothTimeRecoverFinal;
            Vector3 lerpTarget = Vector3.Lerp(transform.position, startPos, t);

            transform.position = Vector3.MoveTowards(transform.position, lerpTarget, recoverSpeed * Time.fixedDeltaTime);
            if (transform.position != startPos)
            {               
                yield return null;
            }
        }

        isDashing = false;

        dashSpeed = 0;
        recoverSpeed = 0;

        smoothTime = 0;
        smoothTimeRecover = 0;
    }

    public void ResetDash()
    {
        startPos = transform.position;
        target = new Vector3(transform.position.x + dashDistance, transform.position.y, transform.position.z);  

        smoothTime = 0;
        dashSpeed = 0;
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
        Vector3 endPos = new Vector3(transform.position.x, rails[Index].y, transform.position.z);

        while (transform.position != endPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPos, moveSpeedMultiplier * Time.deltaTime);
            if (transform.position != endPos)
                yield return null;
        }

        isMoving = false;
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


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDashing && collision.CompareTag("Enemy"))
        {
            if (savedMonster != null) 
            {
                currentMonster.transform.SetParent(null);
                currentMonster.gameObject.SetActive(false);
                currentMonster = null;
            }

            if (currentMonster != null)
            {
                SwitchCurrentMonster(currentMonster);
            }

            print("Collided with enemy named: " + collision.name);
            currentMonster = collision.GetComponent<MonsterBase>();
            PossessEnemy(currentMonster);
            ResetDash();
        }
    }
}
