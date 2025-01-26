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
    [SerializeField] private List<GameObject> levelPrefabs;
    [SerializeField] private TileBase dirt;

    [field: Header("Monitor")]
    [field: SerializeField] public Level CurrentLevel { get; private set; }

    // =================================================================================================================

    public void LoadWorld()
    {
        var placeholders = FindObjectsOfType<LevelPlaceholder>().ToList();
        var dontFillWithDirt = new List<Vector3Int>();

        foreach (var placeholder in placeholders)
        {
            Instantiate(levelPrefabs[Random.Range(0, levelPrefabs.Count)], placeholder.transform.position, Quaternion.identity);

            var placeholderTilemap = placeholder.GetComponentInChildren<Tilemap>();

            foreach (var point in placeholderTilemap.cellBounds.allPositionsWithin)
            {
                dontFillWithDirt.Add(world.WorldToCell(placeholderTilemap.GetCellCenterWorld(point)));
            }
        }

        placeholders.ForEach(x => Destroy(x.gameObject));

        foreach (var point in world.cellBounds.allPositionsWithin)
        {
            if (dontFillWithDirt.Contains(point)) continue;
            world.SetTile(point, dirt);
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

        LoadWorld();
    }

    // =================================================================================================================
}
