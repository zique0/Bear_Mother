using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected Rigidbody2D Rb;

    [Header("Settings")]
    [SerializeField] protected float launchForce;
    [SerializeField] protected float launchAngle;
    [Space(10), SerializeField] private float extraGravity;
    [SerializeField] private AnimationCurve extraGravityAccel;
    [SerializeField] private float extraGravityDelay;
    [SerializeField] private float extraGravityAccelDuration;

    // Management
    private Coroutine gravityRoutine;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Init();
    }

    protected abstract void Init();

    public virtual void Launch(bool rightward)
    {
        var dir = rightward ? Vector2.right : Vector2.left;
        var force = Quaternion.Euler(0, 0, launchAngle) * dir * launchForce;

        Rb.AddForce(force, ForceMode2D.Impulse);

        if (gravityRoutine != null) StopCoroutine(gravityRoutine);
        gravityRoutine = StartCoroutine(GravityRoutine());
    }

    private IEnumerator GravityRoutine()
    {
        var elapsed = 0f;
        var elapsedRatio = 0f;

        yield return new WaitForSeconds(extraGravityDelay);

        while (elapsed < extraGravityAccelDuration)
        {
            var gravity = extraGravity + (extraGravity * extraGravityAccel.Evaluate(elapsedRatio));
            Rb.AddForce(new Vector2(0, -gravity), ForceMode2D.Impulse);

            elapsed += Time.deltaTime;
            elapsedRatio = elapsed / extraGravityAccelDuration;

            yield return null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (gravityRoutine != null) StopCoroutine(gravityRoutine);
    }
}
