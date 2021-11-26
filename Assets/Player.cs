using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 100f;
    public LayerMask groundLayer;

    protected Rigidbody2D rb;
    protected Animator anim;

    public void Start()
    {
        // Save the Rigidbody2D component to a simply variable
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    public void Update()
    {
        Move();
        if(OnGround() && 
            Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        } else if(OnGround() &&
            Input.GetAxis("Vertical") < 0)
        {
            Duck();
        }
        else
        {
            ResetDuck();
        }
    }

    public void Move()
    {
        // Add a force to player in direction of the input
        rb.velocity = new Vector2(GetInputAxisRaw().x * speed, rb.velocity.y);
    }

    public void Jump()
    {
        // Add a force to player in direction of the input
        rb.AddForce(new Vector2(0, 1) * jumpForce, ForceMode2D.Force);
    }

    public virtual void Duck()
    {
        transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
    }

    public void ResetDuck()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    protected virtual Vector3 GetInputAxis()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // calculate direction vectors based on orientation
        Vector3 forward = transform.forward * v;
        Vector3 sideways = transform.right * h;

        Vector3 re = forward + sideways;
        return re;
    }

    // Returns either 1 or -1 for keyboard. No values in between
    protected virtual Vector3 GetInputAxisRaw()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // calculate direction vectors based on orientation
        Vector3 forward = transform.forward * v;
        Vector3 sideways = transform.right * h;

        Vector3 re = forward + sideways;
        return re;
    }

    protected virtual bool OnGround()
    {
        return Physics2D.OverlapCircle((Vector2)transform.position + Vector2.zero, 0.1f, groundLayer);
    }

    protected virtual bool ShouldMove()
    {
        if (GetInputAxisRaw() != Vector3.zero)
        {
            return true;
        }
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position + Vector2.zero, 0.1f);
    }
}
