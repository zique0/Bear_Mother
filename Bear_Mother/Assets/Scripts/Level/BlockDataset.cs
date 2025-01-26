using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Block", menuName = "Block", order = 0)]
public class BlockDataset : ScriptableObject
{
    [field: SerializeField] public List<TileBase> Tiles { get; private set; }
    [field: SerializeField] public BlockData Data { get; private set; }

    [Serializable]
    public class BlockData
    {
        [field: SerializeField] public bool Dirt { get; private set; }
        [field: SerializeField] public bool Bound { get; private set; }
        [field: SerializeField] public bool Breakable { get; private set; }

        public bool PartOfLevel => !Dirt;
    }
}
