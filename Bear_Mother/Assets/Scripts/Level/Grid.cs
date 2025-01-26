using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Grid 
{
    public enum BlockState { EMPTY = 0, BREAKABLE = 1, UNBREAKABLE = 2 }
    private static Dictionary<int, BlockState> blockID = new Dictionary<int, BlockState> 
    { { 0 , BlockState.EMPTY }, { 1, BlockState.BREAKABLE }, { 2, BlockState.UNBREAKABLE } };

    [Serializable] public struct RowData { public int[] row; }
    public RowData[] rows = new RowData[LevelProperties.ROW_LENGTH];

    [Serializable] public struct ColumnData { public int[] column; }
    public ColumnData[] columns = new ColumnData[LevelProperties.COLUMN_LENGTH];

    public static List<BlockState> FlattenGridData(Grid data)
    {
        var trueData = new List<BlockState>();

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
