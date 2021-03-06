using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 100f;
    public LayerMask groundLayer;
    public Color chargingColor;

    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator anim;
    protected Tween colorTween;
    protected Coroutine charge;
    protected bool secondJumpReady = true; 

    public void Start()
    {
        // Save the Rigidbody2D component to a simply variable
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    public void Update()
    {
        Move();
        if(OnGround() && 
            GetJumpInput())
        {
            Jump();
        } else if(!OnGround() && secondJumpReady 
            && GetJumpInput())
        {
            Jump();
            secondJumpReady = false;
        }
        else if(OnGround() &&
            Input.GetAxis("Vertical") < 0)
        {
            Duck();
            if(charge == null)
            {
                charge = StartCoroutine(PowerJump());
            }
        }
        else
        {
            ResetDuck();
        }

        if(OnGround())
        {
            secondJumpReady = true;
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

    protected virtual IEnumerator PowerJump()
    {
        float chargeTime = 1f;
        colorTween = sr.DOColor(chargingColor, chargeTime);
        yield return new WaitForSeconds(chargeTime);
        if(OnGround() && GetInputAxis().z < 0)
        {
            sr.color = new Color(chargingColor.r, 0, chargingColor.b);
            yield return new WaitWhile(() => GetInputAxisRaw().z < 0);
            Jump();
            Jump();
        }            
    }

    public virtual void Duck()
    {
        transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
    }

    public void ResetDuck()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
        sr.color = new Color(255, 255, 255);
        colorTween.Kill();
        charge = null;
    }

    protected virtual bool GetJumpInput()
    {
        return Input.GetKeyDown(KeyCode.Space);
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position + Vector2.zero, 0.1f);
    }
}
