using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ±èÁöÈÆ »ç¶ûÇØ¿ä!!

public class PlayerStatus : MonoBehaviour
{
    [field: Header("Monitor")]
    [field: SerializeField] public int Health { get; private set; }
    [field: SerializeField] public bool Hidden { get; private set; }

    [Header("Settings")]
    [SerializeField] private int maxHealth = 10;

    private SpriteRenderer sprite;

    private void Awake()
    {
        Health = maxHealth;
        sprite = GetComponent<SpriteRenderer>();
    }

    public void TEMP_Death()
    {
        Debug.Log("Player died");
    }

    public void SetHidden()
    {
        Hidden = true;
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.7f);
    }

    public void NotHidden()
    {
        Hidden = false;
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
    }
}
