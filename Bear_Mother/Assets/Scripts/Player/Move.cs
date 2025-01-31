using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Move : PlayerControl
{
    private Claw claw;

    [Header("States")]
    [SerializeField] private float speed;

    [Header("Settings")]
    [SerializeField] private float accelRate;
    [SerializeField] private float decelRate;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float minSpeed;

    // Management
    private Coroutine actRoutine;
    private Coroutine stopRoutine;

    // =================================================================================================================

    protected override void Init()
    {
        claw = GetComponent<Claw>();
    }

    public void ActLeft(InputAction.CallbackContext _)
    {
        FacingRight = false;
        Sprite.flipX = false;

        Act(Vector2.left);
    }

    public void ActRight(InputAction.CallbackContext _)
    {
        FacingRight = true;
        Sprite.flipX = true;

        Act(Vector2.right);
    }

    public void StopLeft(InputAction.CallbackContext _) { if (!FacingRight) Stop(Vector2.left); }
    public void StopRight(InputAction.CallbackContext _) { if (FacingRight) Stop(Vector2.right); }

    // =================================================================================================================

    private void Update()
    {
        if (!Airborne && Rb.velocity.magnitude > 5) Animator.SetTrigger("Run");
        else Animator.SetTrigger("Idle");
    }

    // =================================================================================================================

    public void Act(Vector2 dir)
    {
        Status.NotHidden();
        claw.TryUpdateBreakTargetDirection(Claw.BreakDir.FRONT);

        if (actRoutine != null)
        {
            var decel = Rb.velocity.x;
            Rb.AddForce(new Vector2(-decel, 0), ForceMode2D.Impulse);

            StopCoroutine(actRoutine);
        }

        actRoutine = StartCoroutine(ActRoutine(dir));
    }

    private IEnumerator ActRoutine(Vector2 dir)
    {
        while (OnBamboo)
        {
            // Debug.Log("bamboo");
            yield return null;
        }

        if (stopRoutine != null) StopCoroutine(stopRoutine);

        Vector2 prevPos = Vector2.zero;
        var timeOut = 0f;

        while (!OnBamboo)
        {
            /*
            if (OnBamboo)
            {
                Rb.AddForce(-Rb.velocity, ForceMode2D.Impulse);
                yield break;
            } */

            Rb.AddForce(accelRate * Time.fixedDeltaTime * dir, ForceMode2D.Impulse);
            speed = Mathf.Abs(Rb.velocity.x);

            if (speed > maxSpeed)
            {
                var brake =  speed * (speed - maxSpeed);
                Rb.AddForce(new Vector2(-brake * dir.x * Time.fixedDeltaTime, 0), ForceMode2D.Impulse);
            }

            if (Vector2.Distance(prevPos, transform.position) < 0.05f)
            {
                timeOut += Time.deltaTime;

                if (timeOut > 0.05f)
                {
                    Rb.velocity = Vector2.zero;
                    break;
                }
            }
            else
            {
                timeOut = 0;
            }

            prevPos = transform.position;

            yield return null;
        }
    }

    public void Stop(Vector2 dir)
    {
        if (OnBamboo) return;

        if (stopRoutine != null) StopCoroutine(stopRoutine);
        stopRoutine = StartCoroutine(StopRoutine(dir));
    }

    private IEnumerator StopRoutine(Vector2 dir)
    {
        if (actRoutine != null) StopCoroutine(actRoutine);

        var decel = Rb.velocity.x;
        Rb.AddForce(new Vector2(-decel, 0), ForceMode2D.Impulse);

        /*
        while (!OnBamboo && speed > minSpeed)
        {
            Rb.AddForce(decelRate * Time.fixedDeltaTime * -dir, ForceMode2D.Impulse);
            speed = Mathf.Abs(Rb.velocity.x);

            yield return null;
        } */

        stopRoutine = null;
        yield return null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        /*
        if (actRoutine == null) return;
        if (collision.GetContact(0).normal.y <= 0f)
        {
            Stop(Rb.velocity.x > 0 ? Vector2.right : Vector2.left);
        } */
    }
}
