using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StatePatternPlayer : MonoBehaviour
{
    public State state;
    public string stateName = "_";
    public static Walking walking = new Walking();
    public static Airborn airborn = new Airborn();
    public static DoubleJumped doubleJumped = new DoubleJumped();
    public static Ducking ducking = new Ducking();

    [Space]
    public float speed = 5f;
    public float jumpForce = 100f;
    public LayerMask groundLayer;
    public Color chargingColor;

    [HideInInspector]
    public Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator anim;
    protected Tween colorTween;

    public void Start()
    {
        // Save the Rigidbody2D component to a simply variable
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        state = airborn;
    }

    public void Update()
    {
        Move();
        state.HandleInput(this);
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

    public virtual IEnumerator PowerJump()
    {
        float chargeTime = 1f;
        colorTween = sr.DOColor(chargingColor, chargeTime);
        yield return new WaitForSeconds(chargeTime);
        if (state == ducking)
        {
            sr.color = new Color(chargingColor.r, 0, chargingColor.b);
            yield return new WaitWhile(() => GetInputAxisRaw().z < 0);
            state = airborn;
            ResetDuck();
            Jump();
            Jump();
        }
    }

    public virtual void Duck()
    {
        transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        StartCoroutine(PowerJump());
    }

    public void ResetDuck()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
        sr.color = new Color(255, 255, 255);
        colorTween.Kill();
    }

    public virtual bool GetJumpInput()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public virtual Vector3 GetInputAxis()
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
    public virtual Vector3 GetInputAxisRaw()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // calculate direction vectors based on orientation
        Vector3 forward = transform.forward * v;
        Vector3 sideways = transform.right * h;

        Vector3 re = forward + sideways;
        return re;
    }

    public virtual bool OnGround()
    {
        return Physics2D.OverlapCircle((Vector2)transform.position + Vector2.zero, 0.1f, groundLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position + Vector2.zero, 0.1f);
    }
}
