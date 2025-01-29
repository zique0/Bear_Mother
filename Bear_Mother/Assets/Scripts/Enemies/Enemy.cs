using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    private LevelManager levelManager;
    private PlayerStatus playerStatus;

    [SerializeField] private Level currentLevel;
    [SerializeField] private float processRate = 0.1f;

    [Space(10), SerializeField] private int health;
    [SerializeField] private int maxHealth;

    private void Awake()
    {
        levelManager = FindObjectOfType<LevelManager>();
        playerStatus = FindObjectOfType<PlayerStatus>();
    }

    private void Start()
    {
        StartCoroutine(BehaviourRoutine());
    }

    public void TakeDamage(int deltaVal)
    {
        health -= deltaVal;
        if (health <= 0) Death();
    }

    public void Death()
    {
        Debug.Log("death");
    }

    private IEnumerator BehaviourRoutine()
    {
        while (true)
        {
            if (levelManager.CurrentLevel == currentLevel) // if in same level as player
            {
                if (playerStatus.Hidden)
                {
                    if (idleRoutine == null)
                    {
                        if (culledRoutine != null) StopCoroutine(culledRoutine);
                        if (aggroRoutine != null) StopCoroutine(aggroRoutine);

                        idleRoutine = StartCoroutine(Idle());
                    }
                }
                else
                {
                    if (aggroRoutine == null)
                    {
                        if (culledRoutine != null) StopCoroutine(culledRoutine);
                        if (idleRoutine != null) StopCoroutine(idleRoutine);

                        aggroRoutine = StartCoroutine(Aggro());
                    }
                }
            }
            else
            {
               if (culledRoutine == null)
               {
                    if (idleRoutine != null) StopCoroutine(idleRoutine);
                    if (aggroRoutine != null) StopCoroutine(aggroRoutine);

                    culledRoutine = StartCoroutine(Culled());
               }
            }

            yield return new WaitForSeconds(processRate);
        }
    }

    protected abstract IEnumerator Culled();
    private Coroutine culledRoutine;

    protected abstract IEnumerator Idle();
    private Coroutine idleRoutine;

    protected abstract IEnumerator Aggro();
    private Coroutine aggroRoutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(References.Tags.LevelBound))
        {
            currentLevel = collision.GetComponent<Level>();
        }
    }
}
