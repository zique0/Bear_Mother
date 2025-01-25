using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public abstract class PlayerControl : MonoBehaviour
{
    // Physics
    protected Rigidbody2D Rb { get; private set; }
    protected Collider2D Col { get; private set; }

    // Graphics
    protected SpriteRenderer Sprite { get; private set; }
    protected Animator Animator { get; private set; }

    // =================================================================================================================

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Col = GetComponent<Collider2D>();
        Sprite = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();

        Init();
    }

    protected void Init() { }
}
