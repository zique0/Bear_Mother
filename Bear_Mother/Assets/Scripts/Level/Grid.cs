using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Grid 
{
    public enum BlockType { EMPTY = 0, BREAKABLE = 1, UNBREAKABLE = 2 }
    private static Dictionary<int, BlockType> blockID = new Dictionary<int, BlockType> 
    { { 0 , BlockType.EMPTY }, { 1, BlockType.BREAKABLE }, { 2, BlockType.UNBREAKABLE } };

    [Serializable] public struct RowData { public int[] row; }
    public RowData[] rows = new RowData[LevelProperties.ROW_LENGTH];

    [Serializable] public struct ColumnData { public int[] column; }
    public ColumnData[] columns = new ColumnData[LevelProperties.COLUMN_LENGTH];

    public static List<BlockType> FlattenGridData(Grid data)
    {
        var trueData = new List<BlockType>();

        for (int i = 0; i < data.rows.Length; i++)
        {
            for (int j = 0; j < data.rows[i].row.Length; j++)
            {
                trueData.Add(blockID[data.rows[i].row[j]]);
            }
        }

        return trueData;
    }
}
