using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Jump : PlayerControl
{
    [field: Header("States")]
    [SerializeField] private bool nearBamboo;

    [Header("Settings")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float extraGravity;
    [SerializeField] private AnimationCurve extraGravityAccel;
    [SerializeField] private float extraGravityAccelDuration;

    // Management
    private Coroutine actRoutine;
    private bool acting;

    // =================================================================================================================

    public void Act(InputAction.CallbackContext _)
    {
        Status.NotHidden();

        if (Airborne) return;

        if (OnBamboo)
        {
            OnBamboo = false;
            Rb.gravityScale = 1;

            return;
        }

        if (nearBamboo)
        {
            Debug.Log("Bamboo");

            OnBamboo = true;
            Rb.gravityScale = 0;

            Rb.velocity = Vector2.zero;
            Rb.transform.position = LevelManager.CanBreak.CellToWorld(LevelManager.CanBreak.WorldToCell(transform.position)) + new Vector3(0.5f, 0.5f); ;

            return;
        }

        acting = true;

        if (actRoutine != null) StopCoroutine(actRoutine);
        actRoutine = StartCoroutine(ActRoutine());
    }

    private IEnumerator ActRoutine()
    {
        Airborne = true;
        Rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        var elapsed = 0f;
        var elapsedRatio = 0f;

        while (Airborne)
        {
            var gravity = extraGravity + (extraGravity * extraGravityAccel.Evaluate(elapsedRatio));
            Rb.AddForce(new Vector2(0, -gravity), ForceMode2D.Impulse);

            elapsed += Time.deltaTime;
            elapsedRatio = elapsed / extraGravityAccelDuration;

            yield return null;
        }

        actRoutine = null;
    }

    // =================================================================================================================

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!Airborne) return;

        if (acting)
        {
            acting = false;
            return;
        }

        if (collision.contacts[0].normal.y > 0.5f)
        {
            Airborne = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bamboo"))
        {
            nearBamboo = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Bamboo"))
        {
            nearBamboo = false;
        }
    }
}
