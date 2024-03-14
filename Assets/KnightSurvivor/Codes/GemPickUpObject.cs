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
            // ������ �Ա� ����
            //Destroy(gameObject); // ������ ������Ʈ ����

            // RespawnManager���� ������ ����� ��û
            FindObjectOfType<RespawnManager>().StartCoroutine(FindObjectOfType<RespawnManager>().RespawnAfterDelay());
        }
    }
}
