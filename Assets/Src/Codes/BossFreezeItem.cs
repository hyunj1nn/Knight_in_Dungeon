using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFreezeItem : MonoBehaviour
{
    [SerializeField] public GameObject attackObject;
    public GameObject icePrefab;  // 얼음 애니메이션 프리팹 참조
    public float freezeCooldown = 15.0f; // 5초 쿨타임
    public float detectionRadius = 9.0f; // 대상 감지 범위
    public int maxDetectionCount = 7;  // 한 번에 감지할 수 있는 최대 적의 수

    private float lastFreezeTime = -100f; // 충분히 과거로 설정
    private float originalMass;
    private bool isActivated = false;  // 아이템이 활성화 되었는지 나타내는 변수
    Player player;
    public ItemWaypoint waypointController;

    void Awake()
    {
        player = GameManager.instance.player;
    }

    private void Start()
    {
        if (waypointController != null)  // waypointController가 null이 아닐 때만 호출
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
        this.enabled = true;  // 스크립트를 활성화
        lastFreezeTime = Time.time - freezeCooldown;  // 쿨타임을 현재 시간에서 빼서 설정
    }

    void Update()
    {
        if (!isActivated) return;  // 아이템이 활성화되지 않았다면 Update를 끝냄

        if (Time.time - lastFreezeTime < freezeCooldown) return;  

        // 주변의 적들을 검색
        Collider2D[] allEnemies = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        if (allEnemies.Length == 0) return;  // 감지된 적이 없다면 그냥 반환

        // 가장 가까운 적을 찾기
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

        // 가장 가까운 적 기준으로 다시 범위 내의 적들을 감지
        Collider2D[] enemiesNearbyNearest = Physics2D.OverlapCircleAll(nearestEnemy.position, detectionRadius);

        bool hasPlayedSound = false;

        // 해당 범위의 적들을 얼림 처리
        foreach (var enemyCollider in enemiesNearbyNearest)
        {
            // 만약 감지된 적의 수가 maxDetectionCount를 초과하면 더 이상 적을 감지하지 않기
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

        // 어떤 적이라도 얼린다면 마지막으로 얼린 시간을 업데이트 
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

        float originalMass = enemyRb.mass;  // 원래의 mass 값을 저장
        enemyRb.mass = 1000;  // 얼려진 적의 mass 값을 아주 높게 설정

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
            // 플레이어가 아이템을 획득했을 때의 로직

            BossFreezeItem playerFreezeItem = other.gameObject.GetComponent<BossFreezeItem>();
            if (playerFreezeItem == null)
            {
                playerFreezeItem = other.gameObject.AddComponent<BossFreezeItem>();
                playerFreezeItem.icePrefab = this.icePrefab; // 여기에서 icePrefab 값을 복사
            }
            playerFreezeItem.isActivated = true;  // 아이템 활성화

            // BossDropUi를 표시
            FreezeUi.instance.ShowBossDrop();

            Destroy(gameObject);
        }
    }
}
