using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TEMP_Look : PlayerControl
{
    private Claw claw;
    [SerializeField] private float bambooClimbRb;
    private Coroutine climbRoutine;

    // =================================================================================================================

    protected override void Init()
    {
        claw = GetComponent<Claw>();
    }

    public void ActUp(InputAction.CallbackContext context)
    {
        if (climbRoutine != null) StopCoroutine(climbRoutine);
        if (OnBamboo) climbRoutine = StartCoroutine(Climb(context, Vector2.up));

        claw.TryUpdateBreakTargetDirection(Claw.BreakDir.UP);
    }

    public void ActDown(InputAction.CallbackContext context)
    {
        if (climbRoutine != null) StopCoroutine(climbRoutine);
        if (OnBamboo) climbRoutine = StartCoroutine(Climb(context, Vector2.down));

        claw.TryUpdateBreakTargetDirection(Claw.BreakDir.DOWN);
    }

    private IEnumerator Climb(InputAction.CallbackContext context, Vector2 dir)
    {
        while (OnBamboo && context.performed)
        {
            Rb.velocity = dir * bambooClimbRb;
            yield return null;
        }

        Rb.velocity = Vector2.zero;
    }
}
