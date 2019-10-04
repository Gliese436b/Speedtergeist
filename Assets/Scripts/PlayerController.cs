using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public delegate void FPlayerEnable(PlayerController Player);
    public static event FPlayerEnable OnPlaying;

    // Components
    private SpriteRenderer sr;
    public Color playerColor;
    public Color waitColor;

    // Bools
    public bool isMoving;

    // Numbers
    public float vertDistanceToMove = 1f;
    public float moveSpeedMultiplier = 5f;
    private float vertAxis;
    public int railIndex = 1;

    // Others
    public Vector3[] rails = new Vector3[3];

    private void Start()
    {
        OnPlaying?.Invoke(this);
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
    }

    public void PossessEnemy(EnemyBase Enemy)
    {
        sr.sprite = Enemy.sr.sprite;
    }

    public void PlayerInput()
    {
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
        if (collision.CompareTag("Enemy"))
        {
            print("Collided with enemy " + collision.name);
            collision.GetComponent<EnemyBase>().SetNewBehaviour(EBehaviourType.POSSESSED);
            

        }
    }

}
