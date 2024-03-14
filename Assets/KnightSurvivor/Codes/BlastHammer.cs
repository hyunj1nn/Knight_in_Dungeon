using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastHammer : MonoBehaviour
{
    [SerializeField] public GameObject attackObject;

    [SerializeField] public GameObject HammerPrefab;

    public int id;  // ���� ID
    public int prefabId;  // ������ ID
    public int damage = 100;  // ���� ������
    public float damageRadius = 4.0f;
    public int count;  // ���� ����
    //public float speed;  // ���� �ӵ�
    //public float bulletspeed;
    public int projectileCount = 1;

    // �������� ������ ����(��)
    public float speed = 3.0f;

    // �ֱٿ� �������� ���� �ð�
    private float lastDamageTime = 0.0f;
    public float detectionRadius = 9.0f;  // �߰�: ��� ���� ������ �����ϴ� ����

    private bool isActivated = false;  // �������� Ȱ��ȭ �Ǿ����� ��Ÿ���� ����
    float timer;
    Player player;

    void Awake()
    {
        player = GameManager.instance.player;
    }

    void Update()
    {
        if (!isActivated) return;  // �������� Ȱ��ȭ���� �ʾҴٸ� Update�� �����ϴ�.

        if (!player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        float distanceToTarget = Vector3.Distance(targetPos, transform.position);
        if (distanceToTarget <= detectionRadius) // detectionRadius�� ���ϴ� ���� ������ ������ �����Դϴ�.
        {
            // ������ ���� ������ ����Ʈ�� �غ��մϴ�.
            List<Transform> detectedEnemies = new List<Transform>();

            // ù ��° ���� ����Ʈ�� �߰��մϴ�.
            detectedEnemies.Add(player.scanner.nearestTarget);

            // �ֺ��� ������ �˻��մϴ�.
            Collider2D[] allEnemies = Physics2D.OverlapCircleAll(targetPos, detectionRadius);
            foreach (var enemyCollider in allEnemies)
            {
                if (enemyCollider.transform != player.scanner.nearestTarget)
                {
                    detectedEnemies.Add(enemyCollider.transform);
                }
            }

            // ���� �ð��� ���������� �������� ���� �ð� + ������ ���ݺ��� ũ�ų� ������ �������� �����ϴ�.
            if (Time.time >= lastDamageTime + speed)
            {
                for (int i = 0; i < Mathf.Min(projectileCount, detectedEnemies.Count); i++)
                {
                    ApplyDamage(detectedEnemies[i].GetComponent<Collider2D>());

                    //AudioManager.instance.PlaySfx(AudioManager.Sfx.Blast);
                }
                lastDamageTime = Time.time;  // ���������� �������� ���� �ð��� ���� �ð����� ������Ʈ
            }
        }
    }

    void Fire()
    {
        if (!player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        float distanceToTarget = Vector3.Distance(targetPos, transform.position);
        if (distanceToTarget <= detectionRadius) // detectionRadius�� ���ϴ� ���� ������ ������ �����Դϴ�.
        {
            // ���� ������ ��쿡�� ���� �߻縦 ���� Ÿ�̸ӳ� �ӵ� ���� �ʿ� �����ϴ�.
            // ���, �������� �ٷ� ������ �ڵ带 �ۼ��մϴ�.
            ApplyDamage(player.scanner.nearestTarget.GetComponent<Collider2D>());
        }
    }

    void ApplyDamage(Collider2D detectedEnemyCollider)
    {
        // ù ��°�� ������ ������ �������� ������ ���� �ִϸ��̼��� ǥ���մϴ�.
        Enemy primaryEnemy = detectedEnemyCollider.gameObject.GetComponent<Enemy>();
        if (primaryEnemy != null)
        {
            primaryEnemy.TakeDamage(damage);
            GameObject hammerInstance = Instantiate(HammerPrefab, primaryEnemy.transform.position, Quaternion.identity);

            // ���� ȿ���� ����մϴ�.
            AudioSource audioSource = hammerInstance.AddComponent<AudioSource>(); // �ش� ���� ������Ʈ�� AudioSource ������Ʈ �߰�
            if (audioSource != null)
            {
                audioSource.PlayOneShot(audioSource.clip); // ���� ���
            }

            // �����ϰ� 0 �Ǵ� 1�� �����մϴ�.
            int randomFlip = Random.Range(0, 2);
            if (randomFlip == 1)
            {
                // x���� �������� �������� �������ϴ�.
                Vector3 flippedScale = hammerInstance.transform.localScale;
                flippedScale.x *= -1;
                hammerInstance.transform.localScale = flippedScale;
            }
        }

        // ù ��°�� ������ �� ������ �ִ� ������ ã���ϴ�.
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(detectedEnemyCollider.transform.position, damageRadius);

        foreach (Collider2D enemyCollider in enemiesInRange)
        {
            Enemy enemy = enemyCollider.gameObject.GetComponent<Enemy>();

            // ù ��°�� ������ ���� �ƴϸ� �������� �����ϴ�.
            if (enemy != null && enemy != primaryEnemy)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // �÷��̾ �������� ȹ������ ���� ����

            BlastHammer playerHammerItem = other.gameObject.GetComponent<BlastHammer>();
            if (playerHammerItem == null)
            {
                playerHammerItem = other.gameObject.AddComponent<BlastHammer>();
                playerHammerItem.HammerPrefab = this.HammerPrefab; // ���⿡�� icePrefab ���� ����
            }
            playerHammerItem.isActivated = true;  // ������ Ȱ��ȭ

            // BossDropUi�� ǥ��
            BlastUi.instance.ShowBossDrop();

            // �������� ����ϴ�.
            Destroy(gameObject);
        }
    }
}
