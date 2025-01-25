using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Claw : MonoBehaviour
{
    [Header("States")]
    [SerializeField] private float attackCooldownTimer;

    [Header("Settings")]
    [SerializeField] private float digRadius;
    [SerializeField] private float holdInputThreshold;
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

        if (CheckDiggableBlock())
        {
            while (context.performed)
            {
                elapsed += Time.deltaTime;

                if (elapsed >= holdInputThreshold)
                {
                    TryDig();
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
    }

    // =================================================================================================================

    private bool CheckDiggableBlock()
    {
       return false;
    }

    private void TryDig()
    {
        Debug.Log("Digging");
    }

    // =================================================================================================================

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, digRadius);
    }
}
