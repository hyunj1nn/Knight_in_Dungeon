using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemPickUpObject : MonoBehaviour, IPickUpObject
{
    [SerializeField] int amount;

    [SerializeField] int healAmount;

    public void OnPickUp(Character character)
    {
        GameManager.instance.GetExp(amount);

        character.Heal(healAmount);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 아이템 먹기 로직
            //Destroy(gameObject); // 아이템 오브젝트 제거

            // RespawnManager에게 아이템 재생성 요청
            FindObjectOfType<RespawnManager>().StartCoroutine(FindObjectOfType<RespawnManager>().RespawnAfterDelay());
        }
    }
}
