using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image[] hearts; // 하트 이미지 배열
    [SerializeField] private Sprite fullHeart;  // 체력 있는 하트
    [SerializeField] private Sprite emptyHeart; // 체력 없는 하트
    [SerializeField] private PlayerStatus playerStatus;

    private void Start()
    {
        UpdateHealthUI();
    }   

    public void UpdateHealthUI()
    {
        int health = playerStatus.Health;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }
    private void ApplyShakeEffect(int health)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].rectTransform.DOKill(); // 기존 애니메이션 정지

            if (health <= 0)
            {
              
                  hearts[i].rectTransform.DOShakePosition(1.5f, new Vector3(10f * ((i % 2 == 0) ? 1 : -1), 5f, 0), 20, 90, false, true);
            }
            else if (health <= 1)
            {
                
                 hearts[i].rectTransform.DOShakePosition(1.2f, new Vector3(5f * ((i % 2 == 0) ? 1 : -1), 3f, 0), 15, 90, false, true);
            }
            else if (health <= 2)
            {
                
                 hearts[i].rectTransform.DOShakePosition(10.0f, new Vector3(1.1f * ((i % 2 == 0) ? 1 : -1), 0.25f, 0), 1, 90, true, true);
            }
        }
    }
}