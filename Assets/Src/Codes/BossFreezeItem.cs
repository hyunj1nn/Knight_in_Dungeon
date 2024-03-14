using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFreezeItem : MonoBehaviour
{
    [SerializeField] public GameObject attackObject;
    public GameObject icePrefab;  // ���� �ִϸ��̼� ������ ����
    public float freezeCooldown = 15.0f; // 5�� ��Ÿ��
    public float detectionRadius = 9.0f; // ��� ���� ����
    public int maxDetectionCount = 7;  // �� ���� ������ �� �ִ� �ִ� ���� ��

    private float lastFreezeTime = -100f; // ����� ���ŷ� ����
    private float originalMass;
    private bool isActivated = false;  // �������� Ȱ��ȭ �Ǿ����� ��Ÿ���� ����
    Player player;
    public ItemWaypoint waypointController;

    void Awake()
    {
        player = GameManager.instance.player;
    }

    private void Start()
    {
        if (waypointController != null)  // waypointController�� null�� �ƴ� ���� ȣ��
        {
            waypointController.CreateWaypoint(transform.position);
        }
        else
        {
            //Debug.LogWarning("WaypointController is not assigned in BossDropItem.");
        }
    }

    public void ActivateFreezeItem()
    {
        this.enabled = true;  // ��ũ��Ʈ�� Ȱ��ȭ
        lastFreezeTime = Time.time - freezeCooldown;  // ��Ÿ���� ���� �ð����� ���� ����
    }

    void Update()
    {
        if (!isActivated) return;  // �������� Ȱ��ȭ���� �ʾҴٸ� Update�� ����

        if (Time.time - lastFreezeTime < freezeCooldown) return;  

        // �ֺ��� ������ �˻�
        Collider2D[] allEnemies = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        if (allEnemies.Length == 0) return;  // ������ ���� ���ٸ� �׳� ��ȯ

        // ���� ����� ���� ã��
        Transform nearestEnemy = null;
        float minDistance = float.MaxValue;

        foreach (var enemyCollider in allEnemies)
        {
            float currentDistance = Vector3.Distance(enemyCollider.transform.position, transform.position);
            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                nearestEnemy = enemyCollider.transform;
            }
        }

        int detectedEnemyCount = 0;

        // ���� ����� �� �������� �ٽ� ���� ���� ������ ����
        Collider2D[] enemiesNearbyNearest = Physics2D.OverlapCircleAll(nearestEnemy.position, detectionRadius);

        bool hasPlayedSound = false;

        // �ش� ������ ������ �� ó��
        foreach (var enemyCollider in enemiesNearbyNearest)
        {
            // ���� ������ ���� ���� maxDetectionCount�� �ʰ��ϸ� �� �̻� ���� �������� �ʱ�
            if (detectedEnemyCount >= maxDetectionCount) break;

            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                StartCoroutine(FreezeEnemy(enemy));

                AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource != null && audioSource.clip != null)
                {
                    audioSource.PlayOneShot(audioSource.clip);
                }
                if (!hasPlayedSound)
                {
                    //AudioManager.instance.PlaySfx(AudioManager.Sfx.Freeze);
                    hasPlayedSound = true;
                }

                detectedEnemyCount++;
            }
        }

        // � ���̶� �󸰴ٸ� ���������� �� �ð��� ������Ʈ 
        if (enemiesNearbyNearest.Length > 0)
        {
            lastFreezeTime = Time.time;
        }
    }

    private IEnumerator FreezeEnemy(Enemy enemy)
    {
        if (enemy.isFrozen) yield break;

        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
        if (enemyRb == null)
        {
            Debug.LogWarning("Enemy does not have a Rigidbody2D.");
            yield break;
        }

        enemy.isFrozen = true;

        float originalMass = enemyRb.mass;  // ������ mass ���� ����
        enemyRb.mass = 1000;  // ����� ���� mass ���� ���� ���� ����

        enemy.speed = 0;

        if (icePrefab == null)
        {
            Debug.LogError("Ice Prefab is not set!");
            yield break;
        }

        GameObject iceInstance = Instantiate(icePrefab, enemy.transform.position, Quaternion.identity);
        iceInstance.transform.SetParent(enemy.transform);

        yield return new WaitForSeconds(3.0f);

        enemy.speed = enemy.originalSpeed;
        Destroy(iceInstance);

        enemyRb.mass = originalMass; 
        enemy.isFrozen = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // �÷��̾ �������� ȹ������ ���� ����

            BossFreezeItem playerFreezeItem = other.gameObject.GetComponent<BossFreezeItem>();
            if (playerFreezeItem == null)
            {
                playerFreezeItem = other.gameObject.AddComponent<BossFreezeItem>();
                playerFreezeItem.icePrefab = this.icePrefab; // ���⿡�� icePrefab ���� ����
            }
            playerFreezeItem.isActivated = true;  // ������ Ȱ��ȭ

            // BossDropUi�� ǥ��
            FreezeUi.instance.ShowBossDrop();

            Destroy(gameObject);
        }
    }
}
