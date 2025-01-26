using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Claw : PlayerControl
{
    [Header("States")]
    [SerializeField] private float attackCooldownTimer;

    [Header("Settings")]
    [SerializeField] private float breakRange;
    [SerializeField] private float holdInputThreshold;
    [SerializeField] private float breakDuration;
    [SerializeField] private float attackCooldownDuration;

    // Management
    private Coroutine actRoutine;
    private Coroutine attackCooldownRoutine;

    // =================================================================================================================

    public void Act(InputAction.CallbackContext context)
    {
        StartCoroutine(PollAct(context));
    }

    private IEnumerator PollAct(InputAction.CallbackContext context)
    {
        var elapsed = 0f;
        var breakPos = NearestBreakablePos();

        if (breakPos != null)
        {
            while (context.performed)
            {
                elapsed += Time.deltaTime;

                if (elapsed >= holdInputThreshold)
                {
                    StartCoroutine(TryBreak(context, (Vector2)breakPos));
                    yield break;
                }

                yield return null;
            }
        }

        Attack();
    }

    // =================================================================================================================

    private void Attack()
    {
        if (attackCooldownTimer < attackCooldownDuration || attackCooldownRoutine != null) return;

        Debug.Log("Attacking");

        attackCooldownRoutine = StartCoroutine(CooldownAttack());
    }

    private IEnumerator CooldownAttack()
    {
        attackCooldownTimer = 0;

        while (attackCooldownTimer < attackCooldownDuration)
        {
            attackCooldownTimer += Time.deltaTime;
            yield return null;
        }

        attackCooldownRoutine = null;
    }

    // =================================================================================================================

    private Vector2? NearestBreakablePos()
    {
        var hit = Physics2D.Raycast(transform.position, FacingRight ? Vector2.right : Vector2.left, breakRange,
            LayerMask.GetMask(References.Layers.CanBreak));

        if (!hit) hit = Physics2D.Raycast(transform.position, Vector2.down, breakRange,
            LayerMask.GetMask(References.Layers.CanBreak));

        if (hit) return hit.point;
        else return null;
    }

    private IEnumerator TryBreak(InputAction.CallbackContext context, Vector2 breakPos)
    {
        var elapsed = 0f;

        while (context.performed)
        {
            elapsed += Time.deltaTime;

            if (elapsed >= breakDuration)
            {
                if (breakPos != null)
                {
                    Debug.Log(breakPos);
                    LevelManager.CurrentLevel.DeleteTileAtWorld(breakPos);
                }

                yield break;
            }

            yield return null;
        }
    }

    // =================================================================================================================

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position, transform.position + (FacingRight ? Vector3.right : Vector3.left) * breakRange);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * breakRange);
    }
}
