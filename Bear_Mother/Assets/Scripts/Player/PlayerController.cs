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

    // =================================================================================================================

    private void Awake()
    {
        move = GetComponent<Move>();
        jump = GetComponent<Jump>();

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
    }

    private void OnDisable()
    {
        controls.Disable();
        moveLeftInput.performed -= move.ActLeft;
        moveLeftInput.canceled -= move.StopLeft;
        moveRightInput.performed -= move.ActRight;
        moveRightInput.canceled -= move.StopRight;
        jumpInput.performed -= jump.Act;
    }
}
