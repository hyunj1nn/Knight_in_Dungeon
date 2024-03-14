using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    public GameObject weaponItemPrefab; // 드랍할 무기 아이템 프리팹
 
    public bool isFinalBoss = false; // 마지막 보스인지 여부를 나타내는 플래그. 기본값은 false입니다.

    private bool isDead = false; // 보스가 이미 죽었는지 확인하는 플래그
    private AudioSource audioSource;

    [Header("파이어볼")]
    public GameObject fireballPrefab;       // 파이어볼 프리팹
    public Transform fireballSpawnPoint;    // 파이어볼 발사 지점
    public float fireballSpeed = 10f;       // 파이어볼 발사 속도
    public AudioClip fireballSound;

    [Header("독 장판")]
    public GameObject poisonPitPrefab; // 독 장판 프리팹 참조
    public int numberOfPits = 5; // 생성할 독 장판의 수
    public float range = 5f; // 보스 위치로부터의 최대 생성 거리
    public GameObject poisonWarningPrefab; // 빨간 원판 프리팹 참조
    public float warningDuration = 2f;     // 경고 장판이 표시되는 시간
    public float minDistanceFromPlayer = 1.0f;
    public float maxDistanceFromPlayer = 5.0f;
    public AudioClip poisonAttackSound;

    [Header("돌진")]
    public GameObject warningPrefab;       // 빨간색 경고 장판 프리팹
    public float dashSpeed = 10f;     // 돌진 속도
    public float dashDuration = 2f;   // 돌진 지속 시간
    public int dashCount = 2;         // 돌진 횟수. 기본값은 2로 설정됨
    private Transform player;         // 플레이어 참조
    public AudioClip dashAttackSound;

    [Header("발사체")]
    [SerializeField] private float shotDuration = 5f;  // 발사할 지속 시간
    public GameObject projectilePrefab;    // 발사할 발사체 프리팹
    public float shootInterval = 1f;       // 발사 간격
    public float projectileSpeed = 5f;
    public AudioClip guidedMissileSound;

    // 패턴 변수
    public BossState[] patternOrder = { BossState.Fireball, BossState.PoisonAttack, BossState.DashAttack, BossState.GuidedMissile, BossState.PlayerTargetedShot }; // 원하는 패턴 순서
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

    // 패턴 변경 주기 (예: 5초)
    public float patternChangeInterval = 5f;
    private float patternTimer;

    void Start()
    {
        StartCoroutine(StartPatternAfterDelay(5f));  // 5초 후에 패턴을 시작하도록 설정

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

        // 체력이 0 이하이고 아직 죽지 않았다면
        if (health <= 0 && !isDead)
        {
            if (isFinalBoss)  // 마지막 보스라면
            {
                BossDeath();  // BossDeath 함수를 호출합니다.
            }
            DropWeaponItem();
            isDead = true; // 보스가 죽었음을 표시
            Destroy(gameObject, 1f); // 1초 후에 보스 오브젝트 제거
        }

        // 패턴 변경 조건
        if (patternTimer >= patternChangeInterval)
        {
            ChangePattern();
            patternTimer = 0;
        }
    }
    void BossDeath()
    {
        // 보스의 체력이 0이 되면 호출됩니다.
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
        // 배열에서 다음 패턴 가져오기
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

        // 패턴 배열의 다음 인덱스로 이동
        currentPatternIndex = (currentPatternIndex + 1) % patternOrder.Length;
    }

    void CastFireball()
    {
        // 프리팹이나 스폰 포인트가 할당되지 않았다면 메서드를 빠져나옵니다.
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
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right; // 오른쪽 기준으로 회전

            // 파이어볼 객체 생성
            GameObject fireballInstance = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);

            // 파이어볼을 계산된 방향으로 발사
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
        for (int i = 0; i < dashCount; i++)  // dashCount 횟수만큼 돌진
        {
            // 경고 장판 생성 및 위치 설정
            GameObject warningInstance = Instantiate(warningPrefab, transform.position, Quaternion.identity);
            warningInstance.transform.SetParent(transform);

            // 장판이 일정 시간 동안 보스 위치에 머물도록 함
            yield return new WaitForSeconds(warningDuration);

            // 장판 제거
            Destroy(warningInstance);

            // 대쉬 시작
            Vector2 targetDirection = (player.position - transform.position).normalized;
            yield return StartCoroutine(DashTowardsTarget(targetDirection));  // 대쉬가 완료될 때까지 기다립니다.

            if (i < dashCount - 1)  // 마지막 돌진 전까지는 딜레이를 추가
            {
                yield return new WaitForSeconds(0.5f);  // 예: 1.5초 동안 대기. 필요한 시간으로 조절 가능
            }
        }
    }

    IEnumerator DashTowardsTarget(Vector2 direction)
    {
        float startTime = Time.time;

        // 대쉬 중에는 기존의 움직임을 중지합니다. (예: 움직임 스크립트 비활성화)

        while (Time.time < startTime + dashDuration)
        {
            transform.position += (Vector3)direction * dashSpeed * Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 대쉬 완료 후 원래의 움직임을 재개합니다. (예: 움직임 스크립트 활성화)
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

        int numberOfMissiles = 20; // 원하는 미사일의 수를 지정합니다.
        float angleStep = 360f / numberOfMissiles; // 360도를 미사일의 수로 나눠 각도 스텝을 구합니다.
        float delayBetweenMissiles = 0.1f; // 발사 간격 설정 (예: 0.1초)

        for (int i = 0; i < numberOfMissiles; i++)
        {
            float angle = angleStep * i;
            Vector2 missileDirection = Quaternion.Euler(0, 0, -angle) * Vector2.right; // 시계 방향으로 회전하기 위해 음수로 각도를 설정합니다.

            GameObject missileInstance = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);
            Rigidbody2D missileRb = missileInstance.GetComponent<Rigidbody2D>();
            missileRb.velocity = missileDirection * fireballSpeed;

            yield return new WaitForSeconds(delayBetweenMissiles); // 설정한 시간만큼 대기
        }
    }

    void PlayerTargetedShot()
    {
        StartCoroutine(PlayerTargetedShotCoroutine());
    }

    IEnumerator PlayerTargetedShotCoroutine()
    {
        float elapsedTime = 0f;  // 경과 시간을 저장할 변수

        while (elapsedTime < shotDuration)  // 지정된 시간 동안만 발사
        {
            // 플레이어의 방향 계산
            Vector2 directionToPlayer = (player.position - transform.position).normalized;

            // 발사체 생성 및 설정
            GameObject projectileInstance = Instantiate(projectilePrefab, fireballSpawnPoint.position, Quaternion.identity);
            Rigidbody2D projectileRb = projectileInstance.GetComponent<Rigidbody2D>();
            projectileRb.velocity = directionToPlayer * projectileSpeed;

            // 일정 시간 동안 대기
            yield return new WaitForSeconds(shootInterval);

            elapsedTime += shootInterval;  // 대기 시간만큼 경과 시간을 더해줍니다.
        }
    }
}
