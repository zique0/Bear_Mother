using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Claw : PlayerControl
{
    private Hand hand;

    [Header("States")]
    [SerializeField] private float attackCooldownTimer;

    [Header("Settings")]
    [SerializeField] private int damage;
    [SerializeField] private float attackRadius;
    [SerializeField] private float breakRange;
    [SerializeField] private float holdInputThreshold;
    [SerializeField] private float breakDuration;
    [SerializeField] private float attackCooldownDuration;

    // Management
    private Coroutine actRoutine;
    private Coroutine attackCooldownRoutine;

    // TEMP
    [Space(10), SerializeField] private RectTransform highlight;
    private Image highlightImage;

    // =================================================================================================================

    protected override void Init()
    {
        hand = GetComponent<Hand>();
        highlightImage = highlight.GetComponent<Image>();
    }

    private void Start()
    {
        breakTargets.Add(breakTargetFront); // list in order of priority
        breakTargets.Add(breakTargetUp);
        breakTargets.Add(breakTargetDown);
        breakTargets.Reverse();

        targetByDir.Add(BreakDir.FRONT, breakTargetFront);
        targetByDir.Add(BreakDir.UP, breakTargetUp);
        targetByDir.Add(BreakDir.DOWN, breakTargetDown);

        StartCoroutine(UpdateBreakablePositions());
    }

    public void Act(InputAction.CallbackContext context)
    {
        Status.NotHidden();

        if (hand.Holding)
        {
            hand.Throw();
            return;
        }

        if (Airborne) return;
        StartCoroutine(PollAct(context));
    }

    private IEnumerator PollAct(InputAction.CallbackContext context)
    {
        Controller.KillMovement();

        var elapsed = 0f;

        if (currentBreakTarget.Pos != null)
        {
            while (context.performed)
            {
                elapsed += Time.deltaTime;

                if (elapsed >= holdInputThreshold)
                {
                    StartCoroutine(TryBreak(context));
                    yield break;
                }

                yield return null;
            }
        }

        Attack();

        Controller.EnableMovement();
    }

    private void Update()
    {
        if (currentBreakTarget.Pos != null)
        {
            highlight.transform.position = LevelManager.CanBreak.CellToWorld(LevelManager.CanBreak.WorldToCell(GetBreakTargetPos()));
            highlightImage.color = Color.red;
        }
        else
        {
            highlightImage.color = Color.clear;
        }
    }

    // =================================================================================================================

    private void Attack()
    {
        if (attackCooldownTimer < attackCooldownDuration || attackCooldownRoutine != null) return;

        var enemies = Physics2D.OverlapCircleAll(transform.position + (FacingRight ? Vector3.right : Vector3.left), attackRadius, LayerMask.GetMask("Enemy"));
        foreach (var enemy in enemies)
        {
            if (enemy.TryGetComponent<Enemy>(out var logic))
            {
                logic.TakeDamage(damage);
            }
        }

        attackCooldownRoutine = StartCoroutine(CooldownAttack());
    }

    private IEnumerator CooldownAttack()
    {
        attackCooldownTimer = 0;

        while (attackCooldownTimer < attackCooldownDuration)
        {
            attackCooldownTimer += Time.deltaTime;
            yield return null;
        }

        attackCooldownRoutine = null;
    }

    // =================================================================================================================

    public enum BreakDir { FRONT, UP, DOWN }
    private Dictionary<BreakDir, BreakTarget> targetByDir = new();

    [Serializable]
    public class BreakTarget
    {
        [field: SerializeField] public BreakDir Dir { get; private set; }
        [field: SerializeField] public Vector2? Pos { get; private set; }
        [SerializeField] public Vector2 posCheck = Vector2.zero; // debug
        public Vector2 PosCheck => posCheck;

        public BreakTarget(BreakDir dir) => Dir = dir;

        public void SetPos(Vector2? pos, bool facingRight)
        {
            if (pos != null)
            {
                var offset = facingRight ? 0.1f : -0.1f;

                if (Dir == BreakDir.FRONT) pos = new Vector2(pos.Value.x + offset, pos.Value.y);
                else if (Dir == BreakDir.UP) pos = new Vector2(pos.Value.x, pos.Value.y + 0.1f);
                else if (Dir == BreakDir.DOWN) pos = new Vector2(pos.Value.x, pos.Value.y - 0.1f);
            }

            Pos = pos;

            if (pos == null) posCheck = Vector2.zero;
            else posCheck = pos.Value;
        }
    }

    private readonly BreakTarget breakTargetFront = new(BreakDir.FRONT);
    private readonly BreakTarget breakTargetUp = new(BreakDir.UP);
    private readonly BreakTarget breakTargetDown = new(BreakDir.DOWN);

    [SerializeField] private BreakTarget currentBreakTarget;
    public Vector2 CurrentBreakTargetPos => currentBreakTarget.Pos.Value;

    public Vector2 GetBreakTargetPos()
    {
        return currentBreakTarget.Pos.Value + Vector2.up;
    }

    [Header("Monitor")]
    [SerializeField] private List<BreakTarget> breakTargets = new();
    private bool directionSetByInput;

    public void TryUpdateBreakTargetDirection(BreakDir to)
    {
        if (targetByDir[to].Pos != null)
        {
            currentBreakTarget = targetByDir[to];
            directionSetByInput = true;
        }
    }

    private IEnumerator UpdateBreakablePositions()
    {
        while (true)
        {
            foreach (var target in breakTargets)
            {
                Vector2 checkDir;

                if (target.Dir == BreakDir.FRONT) checkDir = FacingRight ? Vector2.right : Vector2.left;
                else checkDir = target.Dir == BreakDir.UP ? Vector2.up : Vector2.down;

                var hit = Physics2D.Raycast(transform.position, checkDir, breakRange, LayerMask.GetMask(References.Layers.CanBreak));

                if (hit) target.SetPos(hit.point, FacingRight);
                else target.SetPos(null, FacingRight);

                if (target.Pos != null && !directionSetByInput)
                {
                    currentBreakTarget = target;
                }
            }

            if (directionSetByInput)
            {
                if (currentBreakTarget.Pos == null)
                {
                    directionSetByInput = false;

                    var newTarget =  breakTargets.FindLast(x => x.Pos != null);
                    if (newTarget != null) currentBreakTarget = newTarget;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator TryBreak(InputAction.CallbackContext context)
    {
        var elapsed = 0f;

        while (context.performed)
        {
            elapsed += Time.deltaTime;

            if (elapsed >= breakDuration)
            {
                if (currentBreakTarget.Pos != null)
                {
                    LevelManager.DestroyWorldTile(GetBreakTargetPos());
                }

                break;
            }

            yield return null;
        }

        Controller.EnableMovement();
    }

    // =================================================================================================================

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (FacingRight ? Vector3.right : Vector3.left) * breakRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * breakRange);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * breakRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + (FacingRight ? Vector3.right : Vector3.left), attackRadius);
    }
}
