using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DayCycleUI : MonoBehaviour
{
    public TextMeshProUGUI text;

    private void Awake()
    {
      //  text = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateUI(int dayCount)
    {
        text.text = $"Day {dayCount}";
    }
}
