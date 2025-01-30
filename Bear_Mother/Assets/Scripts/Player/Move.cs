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
        if (!Airborne && Rb.velocity.magnitude >= 0.5f) Animator.SetTrigger("Run");
        else Animator.SetTrigger("Idle");
    }

    // =================================================================================================================

    public void Act(Vector2 dir)
    {
        if (OnBamboo) return;

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
        if (stopRoutine != null) StopCoroutine(stopRoutine);

        while (true)
        {
            Rb.AddForce(accelRate * Time.fixedDeltaTime * dir, ForceMode2D.Impulse);
            speed = Mathf.Abs(Rb.velocity.x);

            if (speed > maxSpeed)
            {
                var brake =  speed * (speed - maxSpeed);
                Rb.AddForce(new Vector2(-brake * dir.x * Time.fixedDeltaTime, 0), ForceMode2D.Impulse);
            }

            if (OnBamboo) yield break;

            yield return null;
        }
    }

    public void Stop(Vector2 dir)
    {
        if (stopRoutine != null) StopCoroutine(stopRoutine);
        stopRoutine = StartCoroutine(StopRoutine(dir));
    }

    private IEnumerator StopRoutine(Vector2 dir)
    {
        if (actRoutine != null) StopCoroutine(actRoutine);

        while (speed > minSpeed)
        {
            Rb.AddForce(decelRate * Time.fixedDeltaTime * -dir, ForceMode2D.Impulse);
            speed = Mathf.Abs(Rb.velocity.x);

            if (OnBamboo) yield break;

            yield return null;
        }

        stopRoutine = null;
    }
}
