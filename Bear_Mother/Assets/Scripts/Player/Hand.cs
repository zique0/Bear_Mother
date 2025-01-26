using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hand : PlayerControl
{
    [Header("States")]
    [SerializeField] private Item held;
    private bool Holding => held != null;
    [SerializeField] private bool nearItem;
    [SerializeField] private bool nearTotem;

    [Header("Settings")]
    [SerializeField] private float holdInputThreshold;

    // Management
    private Coroutine actRoutine;

    // =================================================================================================================

    public void Act(InputAction.CallbackContext context)
    {
        if (!nearItem && !held) return;

        if (actRoutine != null) StopCoroutine(actRoutine);
        actRoutine = StartCoroutine(PollAct(context));
    }

    private IEnumerator PollAct(InputAction.CallbackContext context)
    {
        var elapsed = 0f;

        while (context.performed)
        {
            elapsed += Time.deltaTime;

            if (elapsed >= holdInputThreshold)
            {
                if (!Holding) Hold();
                else Drop();

                yield break;
            }

            yield return null;
        }

        if (!Holding)
        {
            Hold();
            FeedSelf();
        }
        else
        {
            if (!nearTotem) FeedSelf();
            else FeedTotem();
        }
    }

    private void Hold() // hold without held
    {
        Debug.Log("Held");
    }

    private void FeedSelf() // press with held
    {
        Debug.Log("Fed self");
    }

    private void FeedTotem() // press near totem with held
    {
        Debug.Log("Fed totem");
    }

    private void Drop() // hold with held
    {
        Debug.Log("Dropped");
    }

    // =================================================================================================================

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(References.Tags.Item))
        {
            nearItem = true;
        } 
        else if (collision.gameObject.CompareTag(References.Tags.Totem))
        {
            nearTotem = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(References.Tags.Item))
        {
            nearItem = false;
        }
        else if (collision.gameObject.CompareTag(References.Tags.Totem))
        {
            nearTotem = false;
        }
    }
}
