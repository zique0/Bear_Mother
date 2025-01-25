using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Grid))]
public class GridDrawer : PropertyDrawer
{
    public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PrefixLabel(pos, label);

        var newPos = pos;
        newPos.y += 250;

        var rowData = property.FindPropertyRelative("rows");

        if (rowData.arraySize != LevelProperties.ROW_LENGTH)
        {
            rowData.arraySize = LevelProperties.ROW_LENGTH;
        }

        // Debug.Log(rowData.arraySize);

        for (int i = 0; i < LevelProperties.ROW_LENGTH; i++)
        {
            var row = rowData.GetArrayElementAtIndex(i).FindPropertyRelative("row");

            newPos.height = 20;
            newPos.width = pos.width / 20;

            if (row.arraySize != LevelProperties.COLUMN_LENGTH)
            {
                row.arraySize = LevelProperties.COLUMN_LENGTH;
            }

            for (int j = 0; j < LevelProperties.COLUMN_LENGTH; j++)
            {
                EditorGUI.PropertyField(newPos, row.GetArrayElementAtIndex(j), GUIContent.none);
                newPos.x += newPos.width;
            }

            newPos.x = pos.x;
            newPos.y -= 25;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 300;
    }
}
