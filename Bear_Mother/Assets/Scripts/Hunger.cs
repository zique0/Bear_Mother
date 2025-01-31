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

    //UI 추가가
    private HungerUI hungerUI;


    private void Awake()
    {
        fullness = maxFullness;
        hungerUI = FindObjectOfType<HungerUI>();
    }

    public void GainFullness()
    {
        Debug.Log("Gained fullness");

        if (fullness < 3)
        {
            fullness++;
            hungerUI.UpdateHungerUI(); // 허기 회복 시 UI 업데이트
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
            fullness = 0;
            onFullnessDepleted.Invoke();

        }
        hungerUI.UpdateHungerUI(); // 허기 감소 시 UI 업데이트
    }

    private void OnEnable()
    {
        DayCycle.OnHalfDay += LoseFullness;
        hungerUI.UpdateHungerUI();
    }

    private void OnDisable()
    {
        DayCycle.OnHalfDay -= LoseFullness;
        hungerUI.UpdateHungerUI();
    }

    //허기값 반환환
    public int GetFullness()
    {
        return fullness;
    }
}
