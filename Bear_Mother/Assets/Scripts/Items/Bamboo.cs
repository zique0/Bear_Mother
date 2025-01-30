using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bamboo : Tool
{
    [SerializeField] private TileBase bambooTop;
    [SerializeField] private TileBase bambooBottom;
    [SerializeField] private TileBase bambooMiddle;
    [Space(10), SerializeField] private List<Vector2Int> bamboo;

    private LevelManager levelManager;

    override protected void Init()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    protected override void FunctionOnLand(Vector3 pos)
    {
        var validPos = levelManager.CanBreak.WorldToCell(pos);
        bamboo.Add(new Vector2Int(validPos.x, validPos.y + 1));
        StartCoroutine(BambooRoutine());

        Debug.Log("a");
    }

    private IEnumerator BambooRoutine()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        while (true)
        {
            for  (int i = 0; i < bamboo.Count; i++)
            {
                var pos = bamboo[i];
                var tile = i == bamboo.Count - 1 ? bambooTop : i == 0 ? bambooBottom : bambooMiddle;

                levelManager.Bamboo.SetTile(new Vector3Int(pos.x, pos.y, 0), tile);
            }

            TileBase aboveTile = levelManager.CanBreak.GetTile((Vector3Int)bamboo[^1] + Vector3Int.up);
            while (aboveTile)
            {
                aboveTile = levelManager.CanBreak.GetTile((Vector3Int)bamboo[^1] + Vector3Int.up);
                yield return null;
            }

            bamboo.Add(bamboo[^1] + Vector2Int.up);
            yield return new WaitForSeconds(0.5f);

            yield return null;
        }
    }
}
