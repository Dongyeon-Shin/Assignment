using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rigidbody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private float jumpPower;
    [SerializeField]
    private float chargeSpeed;
    [SerializeField]
    private float limitJumpPower;
    private float jumpDirection;
    [SerializeField]
    private float chargedJump;
    private bool isJumpable;
    private bool isCharging;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    private void Update()
    {
        if (rigidbody.velocity.y > 0)
        {
            animator.SetBool("IsSoaring", true);
        }
        else if (rigidbody.velocity.y < 0)
        {
            animator.SetBool("IsSoaring", false);
        }
    }
    private void Jump()
    {
        rigidbody.AddForce(Vector2.right * jumpPower * jumpDirection, ForceMode2D.Impulse);
    }
    private void OnJump(InputValue value)
    {
        jumpDirection = value.Get<float>();
        switch (jumpDirection)
        {
            case -1:
                spriteRenderer.flipX = false;
                break;
            case 1:
                spriteRenderer.flipX = true;
                break;
            default:
                break;
        }
        if (isJumpable && !isCharging)
        {
            rigidbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            Jump();
            isJumpable = false;
        }
    }
    private void ChargeJump()
    {
        if (isCharging)
        {
            rigidbody.AddForce(Vector2.up * chargedJump, ForceMode2D.Impulse);
            rigidbody.AddForce(Vector2.right * jumpDirection * jumpPower, ForceMode2D.Impulse);
            isCharging = false;
            isJumpable = false;
        }
    }
    private Coroutine chargeJumpRoutine;
    private void OnCharge(InputValue value)
    {
        if (isJumpable)
        {
            if (value.isPressed)
            {
                chargeJumpRoutine = StartCoroutine("ChargeJumpRoutine");
                isCharging = true;
            }
            else
            {
                StopCoroutine("ChargeJumpRoutine");
                ChargeJump();
            }
        }
    }
    IEnumerator ChargeJumpRoutine()
    {
        chargedJump = 7;
        while (chargedJump < limitJumpPower)
        {
            yield return new WaitForSeconds(0.1f);
            chargedJump += chargeSpeed;
        }
        yield return null;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        isJumpable = true;
        animator.SetBool("IsFloating", false);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        animator.SetBool("IsFloating", true);
    }
}
