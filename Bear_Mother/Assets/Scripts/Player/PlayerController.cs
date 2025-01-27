using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Controls
    private Controls controls;
    private InputAction moveLeftInput;
    private InputAction moveRightInput;
    private InputAction jumpInput;
    private InputAction actionInput1;
    private InputAction actionInput2;

    //

    private InputAction peakUp;
    private InputAction peakDown;

    // Modules
    private Move move;
    private Jump jump;
    private Claw claw;
    private Hand hand;
    private Hide hide;
    private TEMP_Look look;

    // =================================================================================================================

    private void Awake()
    {
        move = GetComponent<Move>();
        jump = GetComponent<Jump>();
        claw = GetComponent<Claw>();
        hand = GetComponent<Hand>();
        hide = GetComponent<Hide>();
        look = GetComponent<TEMP_Look>();

        controls = new();

        moveLeftInput = controls.Player.MoveLeft;
        moveRightInput = controls.Player.MoveRight;
        jumpInput = controls.Player.Jump;
        actionInput1 = controls.Player.Action1;
        actionInput2 = controls.Player.Action2;

        peakUp = controls.Player.PeakUp;
        peakDown = controls.Player.PeakDown;
    }

    private void OnEnable()
    {
        controls.Enable();
        EnableMovement();
    }

    private void OnDisable()
    {
        controls.Disable();
        KillMovement();
    }

    public void KillMovement()
    {
        jumpInput.performed -= jump.Act;
        actionInput1.performed -= claw.Act;
        actionInput2.performed -= hand.Act;
        peakDown.performed -= hide.Act;

        moveLeftInput.performed -= move.ActLeft;
        moveLeftInput.canceled -= move.StopLeft;
        moveRightInput.performed -= move.ActRight;
        moveRightInput.canceled -= move.StopRight;

        peakUp.performed -= look.ActUp;
        peakDown.performed -= look.ActDown;
    }

    public void EnableMovement()
    {
        jumpInput.performed += jump.Act;
        actionInput1.performed += claw.Act;
        actionInput2.performed += hand.Act;
        peakDown.performed += hide.Act;

        moveLeftInput.performed += move.ActLeft;
        moveLeftInput.canceled += move.StopLeft;
        moveRightInput.performed += move.ActRight;
        moveRightInput.canceled += move.StopRight;

        peakUp.performed += look.ActUp;
        peakDown.performed += look.ActDown;
    }
}
