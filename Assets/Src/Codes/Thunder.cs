using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : MonoBehaviour
{
    [SerializeField] public GameObject attackObject;

    [SerializeField] public GameObject lightningPrefab;

    public int id;  // ���� ID
    public int prefabId;  // ������ ID
    public int damage;  // ���� ������
    public float damageRadius = 2.0f;
    public int count;  // ���� ����
    public int projectileCount = 1;

    // �������� ������ ����(��)
    public float speed = 3.0f;

    // �ֱٿ� �������� ���� �ð�
    private float lastDamageTime = 0.0f;

    public float detectionRadius = 9.0f;  // �߰�: ��� ���� ������ �����ϴ� ����

    float timer;
    Player player;

    void Awake()
    {
        player = GameManager.instance.player;
    }

    void Update()
    {
        if (!player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        float distanceToTarget = Vector3.Distance(targetPos, transform.position);
        if (distanceToTarget <= detectionRadius) // detectionRadius�� ���ϴ� ���� ������ ������ ����
        {
            // ������ ���� ������ ����Ʈ�� �غ�
            List<Transform> detectedEnemies = new List<Transform>();

            // ù ��° ���� ����Ʈ�� �߰�
            detectedEnemies.Add(player.scanner.nearestTarget);

            // �ֺ��� ������ �˻�
            Collider2D[] allEnemies = Physics2D.OverlapCircleAll(targetPos, detectionRadius);
            foreach (var enemyCollider in allEnemies)
            {
                if (enemyCollider.transform != player.scanner.nearestTarget)
                {
                    detectedEnemies.Add(enemyCollider.transform);
                }
            }

            if (Time.time >= lastDamageTime + speed)
            {
                for (int i = 0; i < Mathf.Min(projectileCount, detectedEnemies.Count); i++)
                {
                    ApplyDamage(detectedEnemies[i].GetComponent<Collider2D>());
                }
                lastDamageTime = Time.time;  // ���������� �������� ���� �ð��� ���� �ð����� ������Ʈ
            }
        }
    }

    public void LevelUp(float damage, int count, int projectileCount)
    {
        this.damage = (int)damage;
        this.count += count;
        this.projectileCount = projectileCount;

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    public void Init(ItemData data, int projectileCount)
    {
        name = "Thunder" + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        id = data.itemId;
        damage = (int)data.baseDamage;
        count = data.baseCount;

        for (int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            if (data.projectile == GameManager.instance.pool.prefabs[index])
            {
                prefabId = index;
                break;
            }

        }
        switch (id)
        {
            default:
                //speed = 2f;
                //bulletspeed = 11f;
                break;
        }
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    void Fire()
    {
        if (!player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        float distanceToTarget = Vector3.Distance(targetPos, transform.position);
        if (distanceToTarget <= detectionRadius) // detectionRadius�� ���ϴ� ���� ������ ������ ����
        {
            ApplyDamage(player.scanner.nearestTarget.GetComponent<Collider2D>());
        }
    }


    void ApplyDamage(Collider2D detectedEnemyCollider)
    {
        // ù ��°�� ������ ������ �������� ������ ���� �ִϸ��̼��� ǥ��
        Enemy primaryEnemy = detectedEnemyCollider.gameObject.GetComponent<Enemy>();
        if (primaryEnemy != null)
        {
            primaryEnemy.TakeDamage(damage);
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.PlayOneShot(audioSource.clip);
            }
            Instantiate(lightningPrefab, primaryEnemy.transform.position, Quaternion.identity);
        }

        // ù ��°�� ������ �� ������ �ִ� ������ ã��
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(detectedEnemyCollider.transform.position, damageRadius);

        foreach (Collider2D enemyCollider in enemiesInRange)
        {
            Enemy enemy = enemyCollider.gameObject.GetComponent<Enemy>();

            if (enemy != null && enemy != primaryEnemy)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
