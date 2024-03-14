using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnBoomerang  : MonoBehaviour
{
    [SerializeField] public float damage;
    [SerializeField] public int per;
    [SerializeField] public float rotationSpeed = 360.0f;  // �θ޶� ������ ȸ����
    [SerializeField] public float acceleration = 1.0f; // ���ӵ� ����

    Rigidbody2D rigid;
    private Vector3 direction; // To store the firing direction
    [SerializeField] private float returnTime = 1.3f; // Time in seconds after which the bullet should start returning
    private float elapsedTime = 0.0f; // Time elapsed since the bullet was fired
    private bool isReturning = false; // To track whether the bullet is returning
    private Coroutine disappearCoroutine;

    [SerializeField] public float disappearanceTime = 3.5f;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        //StartCoroutine(DisappearAfterTime(disappearanceTime));
    }

    IEnumerator DisappearAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        // Add rotation
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Add acceleration
        if (!isReturning)
        {
            rigid.AddForce(direction * acceleration, ForceMode2D.Force);
        }

        // If the elapsed time exceeds the returnTime, start returning the bullet
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

        // Add these lines to stop any running coroutine and start a new one
        if (disappearCoroutine != null)
        {
            StopCoroutine(disappearCoroutine);
        }
        disappearCoroutine = StartCoroutine(DisappearAfterTime(disappearanceTime));
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy") || per == -1)  // || = or
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