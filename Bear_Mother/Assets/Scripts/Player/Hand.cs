using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hand : PlayerControl
{
    private Hunger selfHunger;
    [SerializeField] private Hunger totemHunger;

    [Header("Monitor")]
    [SerializeField] private Item held;
    public bool Holding => held;
    [SerializeField] private GameObject nearItem;
    [SerializeField] private bool nearTotem;

    [Header("Settings")]
    [SerializeField] private float holdInputThreshold;

    // Management
    private Coroutine actRoutine;

    // =================================================================================================================

    override protected void Init()
    {
        selfHunger = GetComponent<Hunger>();
    }

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
                if (!held) Hold();
                else Drop();

                yield break;
            }

            yield return null;
        }

        if (!held)
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
        held = nearItem.GetComponent<Item>();

        nearItem.transform.SetParent(transform);
        nearItem.gameObject.SetActive(false);

        nearItem = null;
    }

    private void FeedSelf() // press with held
    {
        if (held is Tool) return;

        // var food = held as Food; used in case each food provides diff rates of fullness
        selfHunger.GainFullness();
    }

    private void FeedTotem() // press near totem with held
    {
        if (held is Tool) return;

        // var food = held as Food;
        totemHunger.GainFullness();
    }

    private void Drop(bool reserveReference = false) // hold with held
    {
        held.gameObject.SetActive(true);
        held.transform.SetParent(null);
        held.transform.Translate(Vector3.up * 0.5f);

        if (!reserveReference) held = null;
    }

    public void Throw()
    {
        StartCoroutine(ThrowRoutine());
    }

    private IEnumerator ThrowRoutine()
    {
        var heldRb = held.GetComponent<Rigidbody2D>();
        heldRb.excludeLayers = LayerMask.NameToLayer(References.Layers.Player);

        Drop(true);

        held.Launch(FacingRight);
        held = null;

        yield return new WaitForSeconds(0.5f);
        heldRb.excludeLayers = 0;
    }

    // =================================================================================================================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(References.Tags.Item))
        {
            nearItem = collision.gameObject;
        } 
        else if (collision.gameObject.CompareTag(References.Tags.Totem))
        {
            nearTotem = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(References.Tags.Item))
        {
            if (collision.gameObject == nearItem) nearItem = null;
        }
        else if (collision.gameObject.CompareTag(References.Tags.Totem))
        {
            nearTotem = false;
        }
    }
}
