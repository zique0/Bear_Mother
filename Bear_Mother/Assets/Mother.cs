using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mother : MonoBehaviour
{
    private LevelManager levelManager;
    [SerializeField] private bool init;

    private void Awake()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    public void Death()
    {
        levelManager.levelWithMother = null;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (init) return;

        // Debug.Log("um");

        if (collision.CompareTag(References.Tags.LevelBound))
        {
            // Debug.Log("junsik");

            levelManager.levelWithMother = collision.GetComponent<Level>();
            init = true;
        }
    }
}
