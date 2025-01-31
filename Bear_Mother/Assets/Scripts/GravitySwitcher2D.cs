using System.Collections;
using UnityEngine;

public class GravitySwitcher2D : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isGravityInverted = false; // ✅ 현재 중력 상태 체크
    [SerializeField] private float gravityCooldown = 3.0f; // ✅ 중력 변경 쿨타임 (초)
    [SerializeField] private float gravityForce = -0.005f; // ✅ 기본 중력 크기

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        StartCoroutine(GravitySwitchRoutine());
    }

    private IEnumerator GravitySwitchRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(gravityCooldown); // ✅ 쿨타임 대기

            isGravityInverted = !isGravityInverted; // ✅ 중력 상태 변경
            rb.gravityScale = isGravityInverted ? -gravityForce : gravityForce; // ✅ 중력 반전 적용

            Debug.Log($"Gravity switched! New gravity: {rb.gravityScale}");
        }
    }
}
