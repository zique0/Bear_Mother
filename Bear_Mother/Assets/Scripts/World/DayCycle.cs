using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycle : MonoBehaviour
{
    public enum State { DAY, NIGHT }
    public enum Day { ONE = 1, TWO = 2, THREE = 3, FOUR = 4 }

    [Header("Settings")]
    [SerializeField] private float tickPerSec;
    [SerializeField, Tooltip("In ticks")] private float dayDuration;
    [SerializeField, Tooltip("In ticks")] private float nightDuration;

    [field: Header("Monitor")]
    [field: SerializeField] public State CurrentState { get; private set; }
    [SerializeField] private float stateProgress;
    [field: SerializeField] public Day CurrentDay { get; private set; }

    // Modules
    private SceneManager sceneManager;
    private DayCycleUI UI;

    public delegate void HalfDay();
    public static event HalfDay OnHalfDay;
    private bool halfDayRaised = false;

    public delegate void DayEvent();
    public static event DayEvent OnDay;

    public delegate void NightEvent();
    public static event NightEvent OnNight;

    // ===============================================================================================================

    private void Awake()
    {
        CurrentDay = Day.ONE;
        
        CurrentState = State.NIGHT;
        OnNight?.Invoke();

        sceneManager = FindObjectOfType<SceneManager>();
        UI = FindObjectOfType<DayCycleUI>();
    }

    private void Start()
    {
        StartCoroutine(UpdateRoutine());
    }

    private IEnumerator UpdateRoutine()
    {
        var elapsed = 0;

        while (true)
        {
            if (CurrentState == State.DAY)
            {
                stateProgress = elapsed / dayDuration;

                if (elapsed >= dayDuration * 0.5f && !halfDayRaised)
                {
                    OnHalfDay?.Invoke();
                    halfDayRaised = true;
                }

                if (elapsed >= dayDuration)
                {
                    CurrentState = State.NIGHT;
                    OnNight?.Invoke();

                    elapsed = 0;

                    OnHalfDay?.Invoke();
                    halfDayRaised = false;
                }
            } 
            else
            {
                stateProgress = elapsed / nightDuration;

                if (elapsed >= nightDuration * 0.5f && !halfDayRaised)
                {
                    OnHalfDay?.Invoke();
                    halfDayRaised = true;
                }

                if (elapsed >= nightDuration)
                {
                    if (CurrentDay != Day.FOUR)
                    {
                        CurrentState = State.DAY;
                        OnDay?.Invoke();

                        elapsed = 0;

                        CurrentDay++;
                        UI.UpdateUI((int)CurrentDay);

                        OnHalfDay?.Invoke();
                        halfDayRaised = false;
                    }
                    else
                    {
                        RaiseWin();
                        yield break;
                    }
                }
            }

            elapsed++;
            yield return new WaitForSeconds(1 / tickPerSec);
        }
    }

    private void RaiseWin()
    {
        sceneManager.LoadEnding();
    }
}
