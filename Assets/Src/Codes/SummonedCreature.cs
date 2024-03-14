using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonedCreature : MonoBehaviour
{
    public float speed;
    public float distance;
    public float randomMoveInterval;
    public float randomMoveRange;
    public float attackRadius;
    public int damage;
    public float attackCooldown;
    public GameObject[] firePrefabs; // ������ ����Ʈ ������ �迭

    private Transform player;
    private Vector3 randomTargetPosition;
    private float lastRandomMoveTime;
    private bool flipX;
    private float lastAttackTime;

    void Start()
    {
        //gameObject.SetActive(false); // ������ �� ��ȯ���� ��Ȱ��ȭ

        player = GameObject.Find("Player").transform;
        UpdateRandomTargetPosition();
        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        if (Mathf.Abs(transform.position.x - player.position.x) > distance)
        {
            transform.Translate((player.position - transform.position).normalized * Time.deltaTime * speed * 2);
        }
        else
        {
            if (Time.time - lastRandomMoveTime > randomMoveInterval)
            {
                UpdateRandomTargetPosition();
                lastRandomMoveTime = Time.time;
                flipX = !flipX;
                transform.localScale = new Vector3(flipX ? -1 : 1, 1, 1);
            }

            transform.Translate((randomTargetPosition - transform.position).normalized * Time.deltaTime * speed);

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                AttackEnemies();
                lastAttackTime = Time.time;
            }
        }
    }

    private void UpdateRandomTargetPosition()
    {
        float randomX = Random.Range(player.position.x - randomMoveRange, player.position.x + randomMoveRange);
        float randomY = Random.Range(player.position.y - randomMoveRange, player.position.y + randomMoveRange);
        randomTargetPosition = new Vector3(randomX, randomY, 0f);
    }

    private void AttackEnemies()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRadius);
        int enemyCount = 0;
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    // ������ ����Ʈ�� �����ϰ� �����Ͽ� ����
                    GameObject firePrefab = firePrefabs[Random.Range(0, firePrefabs.Length)];
                    GameObject fireInstance = Instantiate(firePrefab, enemy.transform.position, Quaternion.identity);
                    Destroy(fireInstance, 1f); // ���� �ð� �Ŀ� �ı�
                    enemyCount++;
                    if (enemyCount >= 7)
                    {
                        break;
                    }
                    AudioSource audioSource = GetComponent<AudioSource>();
                    if (audioSource != null && audioSource.clip != null)
                    {
                        audioSource.PlayOneShot(audioSource.clip);
                    }
                }
            }
        }
    }
}

