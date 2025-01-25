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

    // Modules
    private Move move;
    private Jump jump;
    private Claw claw;
    private Hand hand;

    // =================================================================================================================

    private void Awake()
    {
        move = GetComponent<Move>();
        jump = GetComponent<Jump>();
        claw = GetComponent<Claw>();
        hand = GetComponent<Hand>();

        controls = new();

        moveLeftInput = controls.Player.MoveLeft;
        moveRightInput = controls.Player.MoveRight;
        jumpInput = controls.Player.Jump;
        actionInput1 = controls.Player.Action1;
        actionInput2 = controls.Player.Action2;
    }

    private void OnEnable()
    {
        controls.Enable();
        moveLeftInput.performed += move.ActLeft;
        moveLeftInput.canceled += move.StopLeft;
        moveRightInput.performed += move.ActRight;
        moveRightInput.canceled += move.StopRight;
        jumpInput.performed += jump.Act;
        actionInput1.performed += claw.Act;
        actionInput2.performed += hand.Act;
    }

    private void OnDisable()
    {
        controls.Disable();
        moveLeftInput.performed -= move.ActLeft;
        moveLeftInput.canceled -= move.StopLeft;
        moveRightInput.performed -= move.ActRight;
        moveRightInput.canceled -= move.StopRight;
        jumpInput.performed -= jump.Act;
        actionInput1.performed -= claw.Act;
        actionInput2.performed -= hand.Act;
    }
}
