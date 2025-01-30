using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour
{
    private static Vector2 shrinkedSize = new(19, 10);
    private static Vector2 enlargedSize = new(20, 11);

    // References
    [SerializeField] private Tilemap breakableTiles;
    public Tilemap BreakableTiles => breakableTiles;
    [SerializeField] private Tilemap bound;
    public Tilemap Bound => bound;

    [Space(10), SerializeField] private BoxCollider2D col;

    public void Shrink()
    {
        col.size = shrinkedSize;
    }

    public void Enlarge()
    {
        col.size = enlargedSize;
    }
} 
