using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Components
    private SpriteRenderer sr;

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
}
