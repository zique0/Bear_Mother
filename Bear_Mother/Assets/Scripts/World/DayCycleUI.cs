using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DayCycleUI : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UpdateUI(int dayCount)
    {
        text.text = $"Day {dayCount}";
    }
}
