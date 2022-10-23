using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    enum Direction
    {
        back,
        right,
        front,
        left
    }

    public float moveSpeed = 5f;

    public Camera mainCamera;

    public Rigidbody2D rb;
    public Animator animator;
    Direction direction;

    Vector2 movement;

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if(movement.y == -1)
        {
            direction = Direction.front;
        }
        else if (movement.y == 1)
        {
            direction = Direction.back;
        }
        else if (movement.x == -1)
        {
            direction = Direction.left;
        }
        else if (movement.x == 1)
        {
            direction = Direction.right;
        }

        animator.SetFloat("Speed", movement.sqrMagnitude);
        animator.SetInteger("Direction", (int)direction);

        if (movement.sqrMagnitude > 1)
        {
            movement /= movement.sqrMagnitude;
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * movement);
        mainCamera.transform.position = new Vector3(rb.position.x, rb.position.y, -10f);
    }
}
