using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWeapon : MonoBehaviour
{
    public float shoulddisappear = 4f;

    Character character;

    [SerializeField] public int damage = 10; // ���Ⱑ �÷��̾�� �ִ� ������

    void Start()
    {
        Destroy(gameObject, shoulddisappear); // �� �� �Ŀ� �� ���� ������Ʈ�� �ı�
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<Character>())
        {
            Attack(collider.gameObject);
        }
    }

    private void Attack(GameObject target)
    {
        if (character == null)
        {
            character = target.GetComponent<Character>();
        }

        character.TakeDamage(damage);
    }
}
