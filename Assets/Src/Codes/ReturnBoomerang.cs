using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnBoomerang  : MonoBehaviour
{
    [SerializeField] public float damage;
    [SerializeField] public int per;
    [SerializeField] public float rotationSpeed = 360.0f;  // 부메랑 무기의 회전력
    [SerializeField] public float acceleration = 1.0f; // 가속도 설정

    Rigidbody2D rigid;
    private Vector3 direction; 
    [SerializeField] private float returnTime = 1.3f; 
    private float elapsedTime = 0.0f; 
    private bool isReturning = false; 
    private Coroutine disappearCoroutine;

    [SerializeField] public float disappearanceTime = 3.5f;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    IEnumerator DisappearAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        if (!isReturning)
        {
            rigid.AddForce(direction * acceleration, ForceMode2D.Force);
        }

        if (elapsedTime >= returnTime && !isReturning)
        {
            isReturning = true;
            rigid.velocity = -direction * 10f;
        }
    }

    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;
        this.direction = dir;
        this.isReturning = false;
        this.elapsedTime = 0.0f;

        if (per > -1 && rigid != null)
        {
            rigid.velocity = dir * 10f;
        }

        if (disappearCoroutine != null)
        {
            StopCoroutine(disappearCoroutine);
        }
        disappearCoroutine = StartCoroutine(DisappearAfterTime(disappearanceTime));
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
            rigid.velocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }
}
