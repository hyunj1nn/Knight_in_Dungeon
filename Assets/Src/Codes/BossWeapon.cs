using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWeapon : MonoBehaviour
{
    public float shoulddisappear = 4f;

    Character character;

    [SerializeField] public int damage = 10; // 무기가 플레이어에게 주는 데미지

    void Start()
    {
        Destroy(gameObject, shoulddisappear); // 몇 초 후에 이 게임 오브젝트를 파괴
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
