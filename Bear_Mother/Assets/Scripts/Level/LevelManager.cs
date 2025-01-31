using Cinemachine;
using System;
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
    [SerializeField] private Tilemap canBreak;
    public Tilemap CanBreak => canBreak;
    [SerializeField] private Tilemap bamboo;
    public Tilemap Bamboo => bamboo;
    [SerializeField] private List<LevelPreset> levelPrefabs;
    [SerializeField] private TileBase dirt;

    [field: Header("Monitor")]
    [SerializeField] private CinemachineVirtualCamera _camera;
    [field: SerializeField] public Level CurrentLevel { get; private set; }

    [Serializable]
    public class LevelPreset
    {
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public int MinCount { get; private set; }
        [field: SerializeField] public int Count { get; set; }
    }

    private List<Level> levels = new();

    [SerializeField] private List<Level> levelsAccessedByPlayer = new();
    public Level levelWithMother;

    // =================================================================================================================

    public void LoadWorld()
    {
        var placeholders = FindObjectsOfType<LevelPlaceholder>().ToList();
        var preExistingLevels = FindObjectsOfType<Level>().ToList();
        var dontFillWithDirt = new List<Vector3Int>();

        var levelCount = 0;

        foreach (var level in preExistingLevels)
        {
            levelCount++;
            levels.Add(level);

            foreach (var point in level.Bound.cellBounds.allPositionsWithin)
            {
                dontFillWithDirt.Add(canBreak.WorldToCell(level.Bound.GetCellCenterWorld(point)));
            }
        }

        foreach (var placeholder in placeholders)
        {
            var level = Instantiate(levelPrefabs[UnityEngine.Random.Range(0, levelPrefabs.Count)].Prefab, placeholder.transform.position, Quaternion.identity);
            level.name = $"Level {levelCount++}";
            levels.Add(level.GetComponent<Level>());

            var placeholderTilemap = placeholder.GetComponentInChildren<Tilemap>();

            foreach (var point in placeholderTilemap.cellBounds.allPositionsWithin)
            {
                dontFillWithDirt.Add(canBreak.WorldToCell(placeholderTilemap.GetCellCenterWorld(point)));
            }
        }

        placeholders.ForEach(x => Destroy(x.gameObject));

        foreach (var point in canBreak.cellBounds.allPositionsWithin)
        {
            canBreak.SetTile(point, null);

            if (dontFillWithDirt.Contains(point)) continue;
            canBreak.SetTile(point, dirt);
        }

        foreach (var level in levels)
        {
            foreach (var point in level.BreakableTiles.cellBounds.allPositionsWithin)
            {
                var tile = level.BreakableTiles.GetTile(point);

                if (tile != null)
                {
                    level.BreakableTiles.SetTile(point, null);
                    var worldPos = level.BreakableTiles.CellToWorld(point);

                    if (preExistingLevels.Contains(level)) canBreak.SetTile(canBreak.WorldToCell(worldPos) + Vector3Int.up, tile);
                    else canBreak.SetTile(canBreak.WorldToCell(worldPos), tile);
                }
            }
        }
    }

    public void DestroyWorldTile(Vector2 worldPos)
    {
        canBreak.SetTile(canBreak.WorldToCell(worldPos) + Vector3Int.down, null);
    }

    public void EnterLevel(Collider2D bound)
    {
        if (CurrentLevel == null)
        {
            CurrentLevel = bound.GetComponent<Level>();
            CurrentLevel.Shrink();

            var con = _camera.GetComponent<CinemachineConfiner2D>();
            con.m_BoundingShape2D = bound;

            return;
        }

        CurrentLevel.Enlarge();

        CurrentLevel = bound.GetComponent<Level>();
        CurrentLevel.Shrink();

        var confiner = _camera.GetComponent<CinemachineConfiner2D>();
        confiner.m_BoundingShape2D = bound;

        if (!levelsAccessedByPlayer.Contains(CurrentLevel)) levelsAccessedByPlayer.Add(CurrentLevel);
    }

    public void AccessedLevel(Collider2D bound)
    {
        var level = bound.transform.parent.GetComponent<Level>();
        if (!levelsAccessedByPlayer.Contains(level)) levelsAccessedByPlayer.Add(level);

        var enemies = FindObjectsOfType<Enemy>().ToList();
        foreach (var enemy in enemies)
        {
            enemy.UpdateLevelsInReach(levelsAccessedByPlayer);
        }
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
    }

    private void Start()
    {
        StartCoroutine(TEST());
    }

    private IEnumerator TEST()
    {
        yield return new WaitForSeconds(0.1f);
        LoadWorld();
    }

    // =================================================================================================================
}
