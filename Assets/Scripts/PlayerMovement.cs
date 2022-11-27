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
        // move player
        rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * movement);

        // camera follows mouse
        var screenRect = new Rect(0f, 0f, Screen.width, Screen.height);
        if (screenRect.Contains(Input.mousePosition))
        {
            var targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var finalPosition = Vector2.Lerp(rb.position, targetPosition, 0.4f);
            mainCamera.transform.position = new Vector3(finalPosition.x, finalPosition.y, -10f);
        }
        else
        {
            mainCamera.transform.position = new Vector3(rb.position.x, rb.position.y, -10f);
        }
    }
}
