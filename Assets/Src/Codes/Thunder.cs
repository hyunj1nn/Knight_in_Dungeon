using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : MonoBehaviour
{
    [SerializeField] public GameObject attackObject;

    [SerializeField] public GameObject lightningPrefab;

    public int id;  // 무기 ID
    public int prefabId;  // 프리펩 ID
    public int damage;  // 무기 데미지
    public float damageRadius = 2.0f;
    public int count;  // 무기 갯수
    public int projectileCount = 1;

    // 데미지를 입히는 간격(초)
    public float speed = 3.0f;

    // 최근에 데미지를 입힌 시간
    private float lastDamageTime = 0.0f;

    public float detectionRadius = 9.0f;  // 추가: 대상 감지 범위를 정의하는 변수

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
        if (distanceToTarget <= detectionRadius) // detectionRadius는 원하는 감지 범위를 정의한 변수
        {
            // 감지된 적을 저장할 리스트를 준비
            List<Transform> detectedEnemies = new List<Transform>();

            // 첫 번째 적을 리스트에 추가
            detectedEnemies.Add(player.scanner.nearestTarget);

            // 주변의 적들을 검색
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
                lastDamageTime = Time.time;  // 마지막으로 데미지를 입힌 시간을 현재 시간으로 업데이트
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
        if (distanceToTarget <= detectionRadius) // detectionRadius는 원하는 감지 범위를 정의한 변수
        {
            ApplyDamage(player.scanner.nearestTarget.GetComponent<Collider2D>());
        }
    }


    void ApplyDamage(Collider2D detectedEnemyCollider)
    {
        // 첫 번째로 감지된 적에게 데미지를 입히고 번개 애니메이션을 표시
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

        // 첫 번째로 감지된 적 주위에 있는 적들을 찾기
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
