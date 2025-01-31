using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool triggered;

    public void Trigger()
    {
        animator.SetTrigger("trigger");
        triggered = true;
    }

    public void Fail()
    {
        if (!triggered) return;

        Debug.Log("fail");
        triggered = false;
    }

    public void Clear()
    {
        if (!triggered) return;

        animator.SetTrigger("clear");
        triggered = false;
    }
}
