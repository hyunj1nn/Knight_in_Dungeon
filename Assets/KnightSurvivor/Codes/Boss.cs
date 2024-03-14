using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    public GameObject weaponItemPrefab; // ����� ���� ������ ������
 
    public bool isFinalBoss = false; // ������ �������� ���θ� ��Ÿ���� �÷���. �⺻���� false�Դϴ�.

    private bool isDead = false; // ������ �̹� �׾����� Ȯ���ϴ� �÷���
    private AudioSource audioSource;

    [Header("���̾")]
    public GameObject fireballPrefab;       // ���̾ ������
    public Transform fireballSpawnPoint;    // ���̾ �߻� ����
    public float fireballSpeed = 10f;       // ���̾ �߻� �ӵ�
    public AudioClip fireballSound;

    [Header("�� ����")]
    public GameObject poisonPitPrefab; // �� ���� ������ ����
    public int numberOfPits = 5; // ������ �� ������ ��
    public float range = 5f; // ���� ��ġ�κ����� �ִ� ���� �Ÿ�
    public GameObject poisonWarningPrefab; // ���� ���� ������ ����
    public float warningDuration = 2f;     // ��� ������ ǥ�õǴ� �ð�
    public float minDistanceFromPlayer = 1.0f;
    public float maxDistanceFromPlayer = 5.0f;
    public AudioClip poisonAttackSound;

    [Header("����")]
    public GameObject warningPrefab;       // ������ ��� ���� ������
    public float dashSpeed = 10f;     // ���� �ӵ�
    public float dashDuration = 2f;   // ���� ���� �ð�
    public int dashCount = 2;         // ���� Ƚ��. �⺻���� 2�� ������
    private Transform player;         // �÷��̾� ����
    public AudioClip dashAttackSound;

    [Header("�߻�ü")]
    [SerializeField] private float shotDuration = 5f;  // �߻��� ���� �ð�
    public GameObject projectilePrefab;    // �߻��� �߻�ü ������
    public float shootInterval = 1f;       // �߻� ����
    public float projectileSpeed = 5f;
    public AudioClip guidedMissileSound;

    // ���� ����
    public BossState[] patternOrder = { BossState.Fireball, BossState.PoisonAttack, BossState.DashAttack, BossState.GuidedMissile, BossState.PlayerTargetedShot }; // ���ϴ� ���� ����
    private int currentPatternIndex = 0;

    public enum BossState
    {
        Idle,
        Fireball,
        PoisonAttack,
        DashAttack,
        GuidedMissile,
        PlayerTargetedShot
    }

    public BossState currentState = BossState.Idle;

    // ���� ���� �ֱ� (��: 5��)
    public float patternChangeInterval = 5f;
    private float patternTimer;

    void Start()
    {
        StartCoroutine(StartPatternAfterDelay(5f));  // 5�� �Ŀ� ������ �����ϵ��� ����

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    IEnumerator StartPatternAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ChangePattern();
    }

    void Update()
    {
        patternTimer += Time.deltaTime;

        // ü���� 0 �����̰� ���� ���� �ʾҴٸ�
        if (health <= 0 && !isDead)
        {
            if (isFinalBoss)  // ������ �������
            {
                BossDeath();  // BossDeath �Լ��� ȣ���մϴ�.
            }
            DropWeaponItem();
            isDead = true; // ������ �׾����� ǥ��
            Destroy(gameObject, 1f); // 1�� �Ŀ� ���� ������Ʈ ����
        }

        // ���� ���� ����
        if (patternTimer >= patternChangeInterval)
        {
            ChangePattern();
            patternTimer = 0;
        }
    }
    void BossDeath()
    {
        // ������ ü���� 0�� �Ǹ� ȣ��˴ϴ�.
        GameManager.instance.OnBossDefeated();
    }

    void DropWeaponItem()
    {
        if (weaponItemPrefab != null)
        {
            Instantiate(weaponItemPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Weapon item prefab is not assigned!");
        }
    }

    void ChangePattern()
    {
        // �迭���� ���� ���� ��������
        currentState = patternOrder[currentPatternIndex];

        switch (currentState)
        {
            case BossState.Fireball:
                CastFireball();
                break;
            case BossState.PoisonAttack:
                CreatePoisonPits((int)numberOfPits);
                break;
            case BossState.DashAttack:
                DashAttack();
                break;
            case BossState.GuidedMissile:
                StartCoroutine(MissileCoroutine());
                break;
            case BossState.PlayerTargetedShot:
                PlayerTargetedShot();
                break;
        }

        // ���� �迭�� ���� �ε����� �̵�
        currentPatternIndex = (currentPatternIndex + 1) % patternOrder.Length;
    }

    void CastFireball()
    {
        // �������̳� ���� ����Ʈ�� �Ҵ���� �ʾҴٸ� �޼��带 �������ɴϴ�.
        if (fireballPrefab == null || fireballSpawnPoint == null)
        {
            // Debug.LogWarning("Fireball prefab or spawn point is not assigned!");
            return;
        }

        int numberOfFireballs = 8;
        float angleStep = 360f / numberOfFireballs;

        for (int i = 0; i < numberOfFireballs; i++)
        {
            float angle = angleStep * i;
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right; // ������ �������� ȸ��

            // ���̾ ��ü ����
            GameObject fireballInstance = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);

            // ���̾�� ���� �������� �߻�
            Rigidbody2D fireballRb = fireballInstance.GetComponent<Rigidbody2D>();
            fireballRb.velocity = direction * fireballSpeed;
        }

        if (fireballSound != null)
        {
            audioSource.PlayOneShot(fireballSound);
        }
    }

    void CreatePoisonPits(int numberOfPits)
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }

        for (int i = 0; i < numberOfPits; i++)
        {
            Vector2 position = GetRandomPositionAroundPlayer();
            StartCoroutine(ShowWarningAndSpawnPoisonPit(position));
        }
    }

    Vector2 GetRandomPositionAroundPlayer()
    {
        float distance = Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);
        float angle = Random.Range(0, 2 * Mathf.PI);

        float x = player.transform.position.x + distance * Mathf.Cos(angle);
        float y = player.transform.position.y + distance * Mathf.Sin(angle);

        return new Vector2(x, y);
    }

    IEnumerator ShowWarningAndSpawnPoisonPit(Vector2 position)
    {
        GameObject warningInstance = Instantiate(poisonWarningPrefab, position, Quaternion.identity);
        yield return new WaitForSeconds(warningDuration);
        Destroy(warningInstance);
        Instantiate(poisonPitPrefab, position, Quaternion.identity);
    }

    void DashAttack()
    {
        if (dashAttackSound != null)
        {
            audioSource.PlayOneShot(dashAttackSound);
        }

        StartCoroutine(WarningThenDash());
    }

    IEnumerator WarningThenDash()
    {
        for (int i = 0; i < dashCount; i++)  // dashCount Ƚ����ŭ ����
        {
            // ��� ���� ���� �� ��ġ ����
            GameObject warningInstance = Instantiate(warningPrefab, transform.position, Quaternion.identity);
            warningInstance.transform.SetParent(transform);

            // ������ ���� �ð� ���� ���� ��ġ�� �ӹ����� ��
            yield return new WaitForSeconds(warningDuration);

            // ���� ����
            Destroy(warningInstance);

            // �뽬 ����
            Vector2 targetDirection = (player.position - transform.position).normalized;
            yield return StartCoroutine(DashTowardsTarget(targetDirection));  // �뽬�� �Ϸ�� ������ ��ٸ��ϴ�.

            if (i < dashCount - 1)  // ������ ���� �������� �����̸� �߰�
            {
                yield return new WaitForSeconds(0.5f);  // ��: 1.5�� ���� ���. �ʿ��� �ð����� ���� ����
            }
        }
    }

    IEnumerator DashTowardsTarget(Vector2 direction)
    {
        float startTime = Time.time;

        // �뽬 �߿��� ������ �������� �����մϴ�. (��: ������ ��ũ��Ʈ ��Ȱ��ȭ)

        while (Time.time < startTime + dashDuration)
        {
            transform.position += (Vector3)direction * dashSpeed * Time.deltaTime;
            yield return null; // ���� �����ӱ��� ���
        }

        // �뽬 �Ϸ� �� ������ �������� �簳�մϴ�. (��: ������ ��ũ��Ʈ Ȱ��ȭ)
    }

    IEnumerator MissileCoroutine()
    {
        if (guidedMissileSound != null)
        {
            audioSource.PlayOneShot(guidedMissileSound);
        }

        if (fireballPrefab == null || fireballSpawnPoint == null)
        {
            yield break;
        }

        int numberOfMissiles = 20; // ���ϴ� �̻����� ���� �����մϴ�.
        float angleStep = 360f / numberOfMissiles; // 360���� �̻����� ���� ���� ���� ������ ���մϴ�.
        float delayBetweenMissiles = 0.1f; // �߻� ���� ���� (��: 0.1��)

        for (int i = 0; i < numberOfMissiles; i++)
        {
            float angle = angleStep * i;
            Vector2 missileDirection = Quaternion.Euler(0, 0, -angle) * Vector2.right; // �ð� �������� ȸ���ϱ� ���� ������ ������ �����մϴ�.

            GameObject missileInstance = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);
            Rigidbody2D missileRb = missileInstance.GetComponent<Rigidbody2D>();
            missileRb.velocity = missileDirection * fireballSpeed;

            yield return new WaitForSeconds(delayBetweenMissiles); // ������ �ð���ŭ ���
        }
    }

    void PlayerTargetedShot()
    {
        StartCoroutine(PlayerTargetedShotCoroutine());
    }

    IEnumerator PlayerTargetedShotCoroutine()
    {
        float elapsedTime = 0f;  // ��� �ð��� ������ ����

        while (elapsedTime < shotDuration)  // ������ �ð� ���ȸ� �߻�
        {
            // �÷��̾��� ���� ���
            Vector2 directionToPlayer = (player.position - transform.position).normalized;

            // �߻�ü ���� �� ����
            GameObject projectileInstance = Instantiate(projectilePrefab, fireballSpawnPoint.position, Quaternion.identity);
            Rigidbody2D projectileRb = projectileInstance.GetComponent<Rigidbody2D>();
            projectileRb.velocity = directionToPlayer * projectileSpeed;

            // ���� �ð� ���� ���
            yield return new WaitForSeconds(shootInterval);

            elapsedTime += shootInterval;  // ��� �ð���ŭ ��� �ð��� �����ݴϴ�.
        }
    }
}
