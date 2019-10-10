using UnityEngine;

public class MoveGround : MonoBehaviour
{
    public float speed = 2f;
    public float disablePosX = -20;

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {        
        transform.position += Vector3.left * speed * Time.fixedDeltaTime;
    }

    private void Update()
    {
        DisableObject();
    }

    public void DisableObject()
    {
        if (transform.position.x < disablePosX)
        {
            gameObject.SetActive(false);
        }
    }
}
