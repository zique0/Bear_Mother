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
    [SerializeField] private SpriteRenderer heldIcon;

    public bool Holding => held;
    [Space(10), SerializeField] private Item inChest;

    [Space(10), SerializeField] private GameObject nearItem;
    [SerializeField] private bool nearTotem;
    [SerializeField] private Transform nearChest;

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
        if (!nearItem && !held && (!nearChest || !inChest)) return;

        if (actRoutine != null) StopCoroutine(actRoutine);
        actRoutine = StartCoroutine(PollAct(context));
    }

    private IEnumerator PollAct(InputAction.CallbackContext context)
    {
        var elapsed = 0f;

        while ((nearItem || held) && context.performed)
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
            if (nearChest && inChest)
            {
                DropFromChest();
            }
            else
            {
                Hold();
                FeedSelf();
            }
        }
        else
        {
            if (nearTotem) FeedTotem();
            else if (nearChest != null)
            {
                if (inChest) DropFromChest();
                PlaceInChest();
            }
            else FeedSelf();
        }
    }

    private void Hold() // hold without held
    {
        if (!nearItem) return;

        held = nearItem.GetComponent<Item>();
        heldIcon.sprite = held.GetComponent<SpriteRenderer>().sprite;

        nearItem.transform.SetParent(transform);
        nearItem.transform.localPosition = Vector3.up;
        nearItem.SetActive(false);

        nearItem = null;
    }

    private void FeedSelf() // press with held
    {
        if (!held || held is Tool) return;

        // var food = held as Food; used in case each food provides diff rates of fullness
        selfHunger.GainFullness();

        Destroy(held.gameObject);

        held = null;
        heldIcon.sprite = null;
    }

    private void FeedTotem() // press near totem with held
    {
        if (!held || held is Tool) return;

        // var food = held as Food;
        totemHunger.GainFullness();

        Destroy(held.gameObject);

        held = null;
        heldIcon.sprite = null;
    }

    private void PlaceInChest()
    {
        if (!nearChest || !held) return;

        inChest = held;

        held = null;
        heldIcon.sprite = null;

        inChest.transform.SetParent(nearChest);
        inChest.gameObject.SetActive(false);
    }

    private void DropFromChest()
    {
        if (!nearChest || !inChest) return;

        // Debug.Log("hi");

        var go = inChest.gameObject;
        inChest = null;

        go.SetActive(true);
        go.transform.SetParent(null);
    }

    private void Drop(bool reserveReference = false) // hold with held
    {
        if (!held) return;

        held.gameObject.SetActive(true);
        held.transform.SetParent(null);

        if (!reserveReference)
        {
            held = null;
            heldIcon.sprite = null;
        }
    }

    public void Throw()
    {
        if (!held || held is Food) return;

        StartCoroutine(ThrowRoutine());
    }

    private IEnumerator ThrowRoutine()
    {
        var heldRb = held.GetComponent<Rigidbody2D>();
        heldRb.excludeLayers = LayerMask.NameToLayer(References.Layers.Player);

        Drop(true);

        held.Launch(FacingRight);

        held = null;
        heldIcon.sprite = null;

        yield return new WaitForSeconds(0.5f);
        if (heldRb) heldRb.excludeLayers = 0;
    }

    // =================================================================================================================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(References.Tags.Item))
        {
            nearItem = collision.gameObject;
        } 
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(References.Tags.Item))
        {
            if (collision.gameObject == nearItem)
            {
                nearItem = null;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(References.Tags.Totem))
        {
            nearTotem = true;
        } 
        else if (collision.gameObject.CompareTag("Chest"))
        {
            nearChest = collision.gameObject.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(References.Tags.Totem))
        {
            nearTotem = false;
        } 
        else if (collision.gameObject.CompareTag("Chest"))
        {
            nearChest = null;
        }
    }
}
