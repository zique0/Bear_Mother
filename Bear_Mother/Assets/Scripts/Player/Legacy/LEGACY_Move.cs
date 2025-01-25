using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class LEGACY_Move : PlayerControl
{
    [Header("States")]
    [SerializeField] private float speed;

    [Header("Settings")]
    [SerializeField] private float accelRate = 3;
    [SerializeField] private float decelRate = 5;
    [SerializeField] private float maxSpeed = 5;
    [SerializeField] private float minSpeed = 2;

    // Management
    private Coroutine actRoutine;
    private Coroutine stopRoutine;

    // =================================================================================================================

    public void ActLeft(InputAction.CallbackContext context) => Act(Vector2.left);
    public void ActRight(InputAction.CallbackContext context) => Act(Vector2.right);
    public void StopLeft(InputAction.CallbackContext context) => Stop(Vector2.left);
    public void StopRight(InputAction.CallbackContext context) => Stop(Vector2.right);

    // =================================================================================================================

    public void Act(Vector2 dir)
    {
        if (actRoutine != null) StopCoroutine(actRoutine);
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
                var brake = speed * (speed - maxSpeed);
                Rb.AddForce(new Vector2(-brake * dir.x * Time.fixedDeltaTime, 0), ForceMode2D.Impulse);
            }

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

            yield return null;
        }
    }
}
