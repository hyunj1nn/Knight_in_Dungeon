using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDropItem : MonoBehaviour
{
    public GameObject summonedCreaturePrefab; // ��ȯ�� ������

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Character character = other.GetComponent<Character>();
            if (character != null)
            {
                // BossDropUi�� ǥ��
                GuardianUi.instance.ShowBossDrop();

                character.ActivateSummon();

                // ��ȯ�� ������Ʈ ����
                Instantiate(summonedCreaturePrefab, transform.position, Quaternion.identity);

                // ������ ������Ʈ �ı�
                Destroy(gameObject);
            }
        }
    }
}
