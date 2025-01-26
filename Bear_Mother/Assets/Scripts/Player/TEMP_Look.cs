using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TEMP_Look : PlayerControl
{
    private Claw claw;

    // =================================================================================================================

    protected override void Init()
    {
        claw = GetComponent<Claw>();
    }

    public void ActUp(InputAction.CallbackContext _)
    {
        claw.TryUpdateBreakTargetDirection(Claw.BreakDir.UP);
    }

    public void ActDown(InputAction.CallbackContext _)
    {
        claw.TryUpdateBreakTargetDirection(Claw.BreakDir.DOWN);
    }
}
