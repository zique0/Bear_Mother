using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hide : PlayerControl
{
    private bool canHide;

    public void Act(InputAction.CallbackContext _)
    {
        // Debug.Log(canHide);

        if (!canHide) return;
        Status.SetHidden();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(References.Tags.Bush))
        {
            canHide = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(References.Tags.Bush))
        {
            canHide = false;
        }
    }
}
