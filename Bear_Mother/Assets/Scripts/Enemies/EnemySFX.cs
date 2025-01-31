using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySFX : MonoBehaviour
{
    [SerializeField] private List<Enemy> enemies;
    [SerializeField] private float delayBetweenSFX;
    [SerializeField] private float delayBetweenCycle;

    private void Start()
    {
        enemies = FindObjectsOfType<Enemy>().ToList();
        StartCoroutine(SFXRoutine());
    }

    private IEnumerator SFXRoutine()
    {
        var index = 0;

        while (true)
        {
            if (!enemies[index].visible)
            {
                RuntimeManager.PlayOneShot(enemies[index].SFX, enemies[index].gameObject.transform.position);
            }

            index++;

            if (index >= enemies.Count)
            {
                index = 0;

                yield return new WaitForSeconds(delayBetweenCycle - delayBetweenSFX);
            }

            yield return new WaitForSeconds(delayBetweenSFX);
        }
    }
}
