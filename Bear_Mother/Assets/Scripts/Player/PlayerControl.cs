using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(PlayerController))]
public abstract class PlayerControl : MonoBehaviour
{
    // Physics
    protected Rigidbody2D Rb { get; private set; }
    protected Collider2D Col { get; private set; }

    protected static bool FacingRight;
    protected static bool Airborne;
    protected static bool OnBamboo;

    // Graphics
    protected SpriteRenderer Sprite { get; private set; }
    protected Animator Animator { get; private set; }

    // Logic
    protected PlayerStatus Status { get; private set; }
    protected PlayerController Controller { get; private set; }
    protected LevelManager LevelManager { get; private set; }
    protected Tilemap World { get; private set; }

    // =================================================================================================================

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Col = GetComponent<Collider2D>();
        Sprite = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();

        Status = GetComponent<PlayerStatus>();
        Controller = GetComponent<PlayerController>();
        LevelManager = FindObjectOfType<LevelManager>();
        World = LevelManager.CanBreak;

        Init();
    }

    protected virtual void Init() { }
}
