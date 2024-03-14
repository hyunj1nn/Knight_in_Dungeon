using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    public int damage = 10;
    public float disappearTime = 5.0f; // Į�� ���� �ð�
    public float bulletSpeed = 10f; // Į�� �̵� �ӵ�
    public Vector2 shootDirection = Vector2.up;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rb.velocity = shootDirection.normalized * bulletSpeed;
        Destroy(gameObject, disappearTime); // Į�� ���� �ð� �Ŀ� �ı�
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            // �����ϰ� �ϱ� ���� Į�� �ı��ϴ� �ڵ带 ����
        }
    }
}
