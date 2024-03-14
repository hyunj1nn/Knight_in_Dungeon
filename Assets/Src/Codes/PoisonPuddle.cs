using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonPuddle : MonoBehaviour
{
    public float damage = 10f; // �� ������ �ִ� ������
    public float duration = 5f; // �� ������ ���ӵǴ� �ð�

    void Start()
    {
        Destroy(gameObject, duration); // ���� �ð� �� ���� ����
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // �÷��̾�� ������ �浹�ϸ�
        {
            // ������ �ִ� ����
            Character player = other.GetComponent<Character>();
            if (player)
            {
                player.TakeDamage((int)damage);
            }
        }
    }
}
