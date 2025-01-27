using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hunger : MonoBehaviour
{
    [SerializeField] private UnityEvent onFullnessDepleted;

    [Header("Monitor")]
    [SerializeField] private int fullness;

    [Header("Settings")]
    [SerializeField] private int maxFullness;
    // [SerializeField] UI

    private void Awake()
    {
        fullness = maxFullness;
    }

    public void GainFullness()
    {
        Debug.Log("Gained fullness");

        if (fullness < 3)
        {
            fullness++;
        }
    }

    public void LoseFullness()
    {
        Debug.Log("Lost fullness");

        if (fullness > 1)
        {
            fullness--;
        }
        else
        {
            onFullnessDepleted.Invoke();
        }
    }

    private void OnEnable()
    {
        DayCycle.OnHalfDay += LoseFullness;
    }

    private void OnDisable()
    {
        DayCycle.OnHalfDay -= LoseFullness;
    }
}
