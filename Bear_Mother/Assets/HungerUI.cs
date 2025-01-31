using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HungerUI : MonoBehaviour
{
    [SerializeField] private Image[] hungerIcons; // 허기 하트 이미지 배열
    [SerializeField] private Sprite fullHunger;   // 허기 채워진 하트
    [SerializeField] private Sprite emptyHunger;  // 허기 없는 하트
    [SerializeField] private Hunger hungerSystem;


    private void Start()
    {
        UpdateHungerUI();
    }

    public void UpdateHungerUI()
    {
        int fullness = hungerSystem.GetFullness(); // 허기 값 가져오기

        for (int i = 0; i < hungerIcons.Length; i++)
        {
            if (i < fullness)
                hungerIcons[i].sprite = fullHunger;
            else
                hungerIcons[i].sprite = emptyHunger;
        }

        ApplyShakeEffect(fullness);
    }
    private void ApplyShakeEffect(int fullness)
    {
        for (int i = 0; i < hungerIcons.Length; i++)
        {
            hungerIcons[i].rectTransform.DOKill(); // 기존 애니메이션 정지

            if (fullness <= 0)
            {
               
                hungerIcons[i].rectTransform.DOShakePosition(1.5f, new Vector3(10f * ((i % 2 == 0) ? 1 : -1), 5f, 0), 20, 90, false, true);
            }
            else if (fullness <= 1)
            {
               
                hungerIcons[i].rectTransform.DOShakePosition(1.2f, new Vector3(7f * ((i % 2 == 0) ? 1 : -1), 3f, 0), 15, 90, false, true);
            }
            else if (fullness <= 2)
            {
               
                hungerIcons[i].rectTransform.DOShakePosition(10.0f, new Vector3(1.2f * ((i % 2 == 0) ? 1 : -1), 0.25f, 0), 1, 90, true, true);
            }
        }
    }
}
