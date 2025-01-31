using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    private LevelManager levelManager;
    protected PlayerStatus playerStatus;

    [SerializeField] protected Level currentLevel;
    [SerializeField, Tooltip("Of state change")] private float processRate = 0.1f;

    [Space(10), SerializeField] private int health;
    [SerializeField] private int maxHealth;

    private int stunDuration = 1;
    private bool stunned;

    [Space(10), SerializeField] protected int damage;

    [SerializeField] protected List<Level> levelsInReach = new();
    [SerializeField] private float getNewLevelCooldown;
    // private float newLevelMaxDistance;

    protected Rigidbody2D rb;
    protected Collider2D col;
    protected SpriteRenderer sprite;
    protected Animator animator;

    private void Awake()
    {
        levelManager = FindObjectOfType<LevelManager>();
        playerStatus = FindObjectOfType<PlayerStatus>();

        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        health = maxHealth;
    }

    private void Start()
    {
        StartCoroutine(BehaviourRoutine());
    }

    public void TakeDamage(int deltaVal)
    {
        health -= deltaVal;
        stunned = true;

        if (health <= 0) Death();
    }

    public void Death()
    {
        Debug.Log("death");
    }

    public void UpdateLevelsInReach(List<Level> levelsAccessedByPlayer)
    {
        if (!levelsAccessedByPlayer.Contains(currentLevel)) return;

        foreach (var level in levelsAccessedByPlayer)
        {
            if (!levelsInReach.Contains(level) && level != levelManager.levelWithMother && CanAccessLevel(level))
            {
                levelsInReach.Add(level);
            }
        }
    }

    protected virtual bool CanAccessLevel(Level level)
    {
        return true;
    }

    private IEnumerator BehaviourRoutine()
    {
        yield return new WaitUntil(() => levelManager.CurrentLevel != null && levelManager.levelWithMother != null && currentLevel != null);
        levelsInReach.Add(currentLevel);

        while (true)
        {
            if (stunned)
            {
                if (culledRoutine != null)
                {
                    StopCoroutine(culledRoutine);
                    culledRoutine = null;
                }

                if (idleRoutine != null)
                {
                    StopCoroutine(idleRoutine);
                    idleRoutine = null;
                }

                if (aggroRoutine != null)
                {
                    StopCoroutine(aggroRoutine);
                    aggroRoutine = null;
                }

                yield return new WaitForSeconds(stunDuration);
                stunned = false;
            }

            if (levelManager.CurrentLevel == currentLevel) // if in same level as player
            {
                var nearestObstacle = Physics2D.Raycast(transform.position, playerStatus.transform.position - transform.position, 100, LayerMask.GetMask(References.Layers.CanBreak));
                var viewObstructed = Vector2.Distance(nearestObstacle.point, transform.position) < Vector2.Distance(transform.position, playerStatus.transform.position);

                // Debug.Log($"{currentLevel.Bound.WorldToCell(nearestObstacle.point)} {nearestObstacle.point}");

                if (playerStatus.Hidden || viewObstructed)
                {
                    if (idleRoutine == null)
                    {
                        if (culledRoutine != null)
                        {
                            StopCoroutine(culledRoutine);
                            culledRoutine = null;
                        }

                        if (aggroRoutine != null)
                        {
                            StopCoroutine(aggroRoutine);
                            aggroRoutine = null;
                        }

                        idleRoutine = StartCoroutine(Idle());
                    }
                }
                else
                {
                    if (aggroRoutine == null)
                    {
                        if (culledRoutine != null)
                        {
                            StopCoroutine(culledRoutine);
                            culledRoutine = null;
                        }

                        if (idleRoutine != null)
                        {
                            StopCoroutine(idleRoutine);
                            idleRoutine = null;
                        }

                        aggroRoutine = StartCoroutine(Aggro());
                    }
                }
            }
            else
            {
                if (culledRoutine == null)
                {
                    if (idleRoutine != null)
                    {
                        StopCoroutine(idleRoutine);
                        idleRoutine = null;
                    }

                    if (aggroRoutine != null)
                    {
                        StopCoroutine(aggroRoutine);
                        aggroRoutine = null;
                    }

                    culledRoutine = StartCoroutine(Culled());
                }
            }

            yield return new WaitForSeconds(processRate);
        }
    }

    private Coroutine culledRoutine;
    protected virtual IEnumerator Culled()
    {
        Debug.Log("Culled");
        sprite.color = Color.gray;

        var elapsed = 0f;
        while (true)
        {
            elapsed += Time.deltaTime;

            if (elapsed >= getNewLevelCooldown)
            {
                var levelsInReachWithoutPlayer = new List<Level>(levelsInReach);
                levelsInReachWithoutPlayer.Remove(levelManager.CurrentLevel);

                var newLevel = levelsInReachWithoutPlayer[Random.Range(0, levelsInReachWithoutPlayer.Count)];
                currentLevel = newLevel;

                transform.position = currentLevel.transform.position;
                elapsed = 0;

                Debug.Log($"{gameObject.name} moves to {currentLevel}");
            }

            yield return null;
        }
    }


    private Coroutine idleRoutine;
    protected virtual IEnumerator Idle()
    {
        Debug.Log("Idle");
        sprite.color = Color.white;

        yield return null;
    }


    private Coroutine aggroRoutine;
    protected virtual IEnumerator Aggro()
    {
        Debug.Log("Aggro");
        sprite.color = Color.red;

        yield return null;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(References.Tags.LevelBound))
        {
            // Debug.Log("Enemy level entered");
            currentLevel = collision.GetComponent<Level>();
        }
    }

    // =================================================================================================================
}
