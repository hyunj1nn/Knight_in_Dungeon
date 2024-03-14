using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    public int damage = 10;
    public float disappearTime = 5.0f; // 칼의 생존 시간
    public float bulletSpeed = 10f; // 칼의 이동 속도
    public Vector2 shootDirection = Vector2.up;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rb.velocity = shootDirection.normalized * bulletSpeed;
        Destroy(gameObject, disappearTime); // 칼을 일정 시간 후에 파괴
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            // 관통하게 하기 위해 칼을 파괴하는 코드를 삭제
        }
    }
}
