using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDirector : MonoBehaviour
{
    private LevelManager levelManager;

    private void Awake()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(References.Tags.LevelBound))
        {
            // Debug.Log("Level entered");
            levelManager.EnterLevel(collision);
        }

        if (collision.CompareTag("AccessBound"))
        {
            // Debug.Log("Level accessed");
            levelManager.AccessedLevel(collision);
        }
    }
}
