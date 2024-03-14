using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonPuddle : MonoBehaviour
{
    public float damage = 10f; // 독 장판이 주는 데미지
    public float duration = 5f; // 독 장판이 지속되는 시간

    void Start()
    {
        Destroy(gameObject, duration); // 일정 시간 후 장판 제거
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 플레이어와 장판이 충돌하면
        {
            // 데미지 주는 로직
            Character player = other.GetComponent<Character>();
            if (player)
            {
                player.TakeDamage((int)damage);
            }
        }
    }
}
