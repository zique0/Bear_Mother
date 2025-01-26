using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour
{
    // [Header("States")]

    // [Header("Monitor")]
    // [SerializeField] private Grid gridState;

    // References
    [SerializeField] private Tilemap breakableTiles;

    // =================================================================================================================
    public void DeleteTileAtWorld(Vector2 worldPos)
    {
        Debug.Log(breakableTiles.WorldToCell(worldPos));
        Debug.Log(breakableTiles.GetTile(breakableTiles.WorldToCell(worldPos)));

        breakableTiles.SetTile(breakableTiles.WorldToCell(worldPos) + Vector3Int.down, null);
    }
} 
