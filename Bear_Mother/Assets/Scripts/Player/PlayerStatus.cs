using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������ ����ؿ�!!

public class PlayerStatus : MonoBehaviour
{
    [field: Header("Monitor")]
    [field: SerializeField] public int Health { get; private set; }
    [field: SerializeField] public bool Hidden { get; private set; }

    [Header("Settings")]
    [SerializeField] private int maxHealth = 10;

    private SpriteRenderer sprite;
    private HealthUI healthUI;

    private Boss boss;

    private void Awake()
    {
        Health = maxHealth;
        sprite = GetComponent<SpriteRenderer>();
        healthUI = FindObjectOfType<HealthUI>();

        boss = FindObjectOfType<Boss>();
    }

    public void TakeDamage(int deltaVal)
    {
        Health -= deltaVal;
        if (Health < 0) TEMP_Death();

        healthUI.UpdateHealthUI(); // 체력 변경 시 UI 업데이트
    }

    public void TEMP_Death()
    {
        Debug.Log("Player died");
    }
     public void Heal(int deltaVal)
    {
        Health = Mathf.Min(Health + deltaVal, maxHealth);
        healthUI.UpdateHealthUI(); // 체력 회복 시 UI 업데이트
    }

    public void SetHidden()
    {
        Hidden = true;
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.7f);

        boss.Clear();
    }

    public void NotHidden()
    {
        Hidden = false;
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
    }
}
