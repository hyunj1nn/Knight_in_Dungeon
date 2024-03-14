using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDropItem : MonoBehaviour
{
    public GameObject summonedCreaturePrefab; // 소환수 프리팹

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Character character = other.GetComponent<Character>();
            if (character != null)
            {
                // BossDropUi를 표시
                GuardianUi.instance.ShowBossDrop();

                character.ActivateSummon();

                // 소환수 오브젝트 생성
                Instantiate(summonedCreaturePrefab, transform.position, Quaternion.identity);

                // 아이템 오브젝트 파괴
                Destroy(gameObject);
            }
        }
    }
}
