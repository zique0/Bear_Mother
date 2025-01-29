using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tool : Item
{
    protected bool active;

    public override void Launch(bool rightward)
    {
        base.Launch(rightward);
        active = true;
    }

    protected abstract void FunctionOnLand(Vector3 pos);

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (active && collision.contacts[0].normal.y > 0.5f)
        {
            FunctionOnLand(collision.contacts[0].point);
        }
    }
}
