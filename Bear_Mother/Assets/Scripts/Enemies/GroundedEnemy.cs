using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedEnemy : Enemy
{
    [Space(10), SerializeField] private float chargeForce;
    [SerializeField] private float cooldown;
    [Space(10), SerializeField] private float patrolSpeed;
    [SerializeField] private List<float> wayPoints = new();

    private bool grounded;
    private bool changeDir;
    private bool coolingDown;

    protected override IEnumerator Aggro()
    {
        yield return new WaitUntil(() => grounded);

        StartCoroutine(base.Aggro());

        animator.SetTrigger("Aggro");

        while (true)
        {
            var dirToPlayer = (playerStatus.transform.position - transform.position).normalized;
            var velocity = new Vector2(dirToPlayer.x * chargeForce, 0);
            rb.velocity = velocity;

            if (coolingDown)
            {
                rb.velocity = Vector2.zero;
                yield return new WaitForSeconds(cooldown);
                coolingDown = false;
            }

            yield return null;
        }
    }

    protected override IEnumerator Idle()
    {
        yield return new WaitUntil(() => grounded);

        StartCoroutine(base.Idle());

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
            grounded = true;
        }

        if (collision.gameObject.TryGetComponent(out PlayerStatus player))
        {
            player.TakeDamage(damage);
            coolingDown = true;
            return;
        }

        changeDir = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(References.Layers.CanBreak))
        {
            grounded = false;
        }
    }
}
