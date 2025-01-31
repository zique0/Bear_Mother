using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FocusAudio : PlayerControl
{
    [SerializeField] private StudioEventEmitter _emitter;

    public void Focus(InputAction.CallbackContext _)
    {
        if (OnBamboo || Airborne) return;

        _emitter.EventInstance.setParameterByName("Focus", 1);

        Focusing = true;
        Controller.KillMovement(false);
    }

    public void Unfocus(InputAction.CallbackContext _)
    {
        _emitter.EventInstance.setParameterByName("Focus", 0);

        Focusing = false;
        Controller.EnableMovement(false);
    }
}
