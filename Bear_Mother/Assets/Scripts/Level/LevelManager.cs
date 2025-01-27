using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public Dictionary<TileBase, BlockDataset.BlockData> BlockData { get; private set; } = new();
    [SerializeField] private List<BlockDataset> dataset;

    [Header("Load Level")]
    [SerializeField] private Tilemap world;
    public Tilemap World => world;
    [SerializeField] private List<GameObject> levelPrefabs;
    [SerializeField] private TileBase dirt;

    [field: Header("Monitor")]
    [SerializeField] private CinemachineVirtualCamera _camera;
    [field: SerializeField] public Level CurrentLevel { get; private set; }

    // =================================================================================================================

    public void LoadWorld()
    {
        var placeholders = FindObjectsOfType<LevelPlaceholder>().ToList();
        var dontFillWithDirt = new List<Vector3Int>();

        var levelCount = 0;

        foreach (var placeholder in placeholders)
        {
            var level = Instantiate(levelPrefabs[Random.Range(0, levelPrefabs.Count)], placeholder.transform.position, Quaternion.identity);
            level.name = $"Level {levelCount++}";

            var placeholderTilemap = placeholder.GetComponentInChildren<Tilemap>();

            foreach (var point in placeholderTilemap.cellBounds.allPositionsWithin)
            {
                dontFillWithDirt.Add(world.WorldToCell(placeholderTilemap.GetCellCenterWorld(point)));
            }
        }

        placeholders.ForEach(x => Destroy(x.gameObject));

        foreach (var point in world.cellBounds.allPositionsWithin)
        {
            world.SetTile(point, null);

            if (dontFillWithDirt.Contains(point)) continue;
            world.SetTile(point, dirt);
        }
    }

    public void DestroyWorldTile(Vector2 worldPos)
    {
        Debug.Log($"try destroy world at {world.WorldToCell(worldPos)}");

        if (world.GetTile(world.WorldToCell(worldPos)) == dirt)
        {
            Debug.Log($"destroy world at {world.WorldToCell(worldPos)}");
            world.SetTile(world.WorldToCell(worldPos) + Vector3Int.down, null);
        }
        else
        {
            Debug.Log($"destroy level at {CurrentLevel.BreakableTiles.WorldToCell(worldPos)}");
            CurrentLevel.BreakableTiles.SetTile(CurrentLevel.BreakableTiles.WorldToCell(worldPos) + Vector3Int.down, null);
        }
    }

    public void EnterLevel(Collider2D bound)
    {
        CurrentLevel = bound.GetComponent<Level>();

        var confiner = _camera.GetComponent<CinemachineConfiner2D>();
        confiner.m_BoundingShape2D = bound;
    }

    // =================================================================================================================

    private void Awake()
    {
        foreach (var set in dataset)
        {
            foreach (var tile in set.Tiles)
            {
                BlockData.Add(tile, set.Data);
            }
        }

        LoadWorld();
    }

    // =================================================================================================================
}
