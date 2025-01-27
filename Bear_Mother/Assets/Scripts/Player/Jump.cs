using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Jump : PlayerControl
{
    [field: Header("States")]
    // [field: SerializeField] public bool Airborne { get; private set; }
    [field: SerializeField] public bool Grounded { get; private set; }

    [Header("Settings")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float extraGravity;
    [SerializeField] private AnimationCurve extraGravityAccel;
    [SerializeField] private float extraGravityAccelDuration;

    // Management
    private Coroutine actRoutine;

    // =================================================================================================================

    public void Act(InputAction.CallbackContext _)
    {
        Status.NotHidden();

        if (Airborne) return;

        if (actRoutine != null) StopCoroutine(actRoutine);
        actRoutine = StartCoroutine(ActRoutine());
    }

    private IEnumerator ActRoutine()
    {
        Airborne = true;
        Rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        var elapsed = 0f;
        var elapsedRatio = 0f;

        while (Airborne)
        {
            var gravity = extraGravity + (extraGravity * extraGravityAccel.Evaluate(elapsedRatio));
            Rb.AddForce(new Vector2(0, -gravity), ForceMode2D.Impulse);

            elapsed += Time.deltaTime;
            elapsedRatio = elapsed / extraGravityAccelDuration;

            yield return null;
        }

        /*
        var yVel = Rb.velocity.y;

        while (yVel < 0)
        {
            var brake = yVel * (yVel - 0);

            Rb.AddForce(new Vector2(-brake * Time.fixedDeltaTime, 0), ForceMode2D.Impulse);
            yVel = Rb.velocity.y;

            yield return null;
        } */

        actRoutine = null;
    }

    // =================================================================================================================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
        {
            Grounded = true;
            Airborne = false;
        }
    }
}
