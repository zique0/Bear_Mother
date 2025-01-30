using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Tool
{
    [Space(10), SerializeField] private int damage;
    [SerializeField] private float radius;

    protected override void Init()
    {

    }

    protected override void FunctionOnLand(Vector3 pos)
    {
        var range = Physics2D.OverlapCircleAll(pos, radius);

        foreach (var col in range)
        {
            if (col.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}
