using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    [Space(10), SerializeField] private float chargeForce;
    [SerializeField] private float chargeCooldown;
    [SerializeField] private float chargeDuration;
    [Space(10), SerializeField] private float patrolSpeed;
    [SerializeField] private int elevation;
    private float elevationInRoom;
    [SerializeField] private List<float> wayPoints = new();

    private bool changeDir;
    private bool coolingDown;
    private bool attacking;

    protected override IEnumerator Aggro()
    {
        StartCoroutine(base.Aggro());

        animator.SetTrigger("Attack");

        rb.velocity = Vector2.zero;

        var elapsed = 0f;
        while (true)
        {
            elapsed += Time.deltaTime;

            if (elapsed > chargeCooldown)
            {
                var playerPos = playerStatus.transform.position;
                var dirToPlayer = (playerPos - transform.position).normalized;

                var chargeElapsed = 0f;

                while (Vector2.Distance(playerPos, transform.position) > 0.05f)
                {
                    chargeElapsed += Time.deltaTime;

                    if (chargeElapsed >= chargeDuration)
                    {
                        coolingDown = true;
                    }

                    if (changeDir)
                    {
                        coolingDown = true;
                    }

                    if (coolingDown)
                    {
                        animator.SetTrigger("Idle");

                        rb.velocity = Vector2.zero;
                        col.enabled = false;

                        var reposition = transform.DOMove(new Vector2(transform.position.x, currentLevel.Bound.CellToWorld(Vector3Int.up * elevation).y), chargeCooldown * 0.8f);
                        yield return reposition.WaitForCompletion();

                        yield return new WaitForSeconds(chargeCooldown * 0.2f);
                        coolingDown = false;

                        col.enabled = true;
                        break;
                    }

                    attacking = true;
                    rb.velocity = dirToPlayer * chargeForce;

                    yield return null;
                }

                elapsed = 0f;
            }

            yield return null;
        }
    }

    protected override IEnumerator Idle()
    {
        StartCoroutine(base.Idle());

        animator.SetTrigger("Idle");

        transform.position = new Vector2(transform.position.x, currentLevel.Bound.CellToWorld(Vector3Int.up * elevation).y);

        var minX = currentLevel.Bound.CellToWorld(currentLevel.Bound.cellBounds.min).x + 5;
        var maxX = currentLevel.Bound.CellToWorld(currentLevel.Bound.cellBounds.max).x - 5;

        wayPoints.Clear();
        wayPoints.Add(minX);
        wayPoints.Add(maxX);

        var currentWaypoint = 0;

        while (true)
        {
            var dir = (wayPoints[currentWaypoint] - transform.position.x) > 0 ? 1 : -1;
            while (Mathf.Abs(transform.position.x - wayPoints[currentWaypoint]) > 0.05f)
            {
                rb.velocity = new Vector2(dir * patrolSpeed, 0);

                if (changeDir)
                {
                    changeDir = false;
                    break;
                }

                yield return null;
            }

            currentWaypoint = (currentWaypoint + 1) % wayPoints.Count;

            yield return null;
        }
    }

    // =================================================================================================================

    protected override bool CanAccessLevel(Level level)
    {
        if (level.transform.position.y > currentLevel.transform.position.y) return false;
        else return true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(References.Layers.CanBreak))
        {
            // Debug.Log("hit wall");

            changeDir = true;
        }

        if (attacking && collision.gameObject.TryGetComponent(out PlayerStatus player))
        {
            // Debug.Log("hit player");

            player.TakeDamage(damage);
            coolingDown = true;

            return;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(References.Layers.CanBreak))
        {
            changeDir = true;
        }
    }
}
