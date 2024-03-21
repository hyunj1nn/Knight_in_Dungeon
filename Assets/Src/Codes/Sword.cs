using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public float damage;
    public int per;
    public float despawnTime;
    public bool shouldDisappear;

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }



    public void Init(float damage, int per, Vector3 dir, float bulletspeed)
    {
        this.damage = damage;
        this.per = per;


        if (per > -1 && rigid != null)
        {
            rigid.velocity = dir * bulletspeed;
        }

        if (shouldDisappear) 
        {
            Invoke("Disappear", despawnTime); 
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy") || per == -1) 
            return;

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage((int)damage);
        }

        per--;

        if (per == -1)
        {
            CancelInvoke("Disappear"); 
            rigid.velocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }
    void Disappear()
    {
        gameObject.SetActive(false);
    }

    void Despawn()
    {
        gameObject.SetActive(false);
    }
}
