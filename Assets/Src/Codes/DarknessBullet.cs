using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarknessBullet : MonoBehaviour
{
    [SerializeField] public GameObject attackObject;
    [SerializeField] public GameObject DarknessSwordPrefab;
    public int projectileCount = 2; // 발사할 무기의 갯수
    public int id;  // 무기 ID
    public int prefabId;  // 프리펩 ID
    public int damage = 100;  // 무기 데미지
    public float detectionRadius = 9.0f;  // 대상 감지 범위를 정의하는 변수
    public float speed = 1.7f; // 데미지를 입히는 간격(초)

    private float lastDamageTime = 0.0f;
    private bool isActivated = false;  // 아이템이 활성화 되었는지 나타내는 변수
    Player player;

    void Awake()
    {
        player = GameManager.instance.player;
    }

    void Update()
    {
        if (!isActivated) return;  // 아이템이 활성화되지 않았다면 Update를 끝냅니다.

        if (!player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        float distanceToTarget = Vector3.Distance(targetPos, transform.position);
        if (distanceToTarget <= detectionRadius) // detectionRadius는 원하는 감지 범위를 정의한 변수입니다.
        {
            if (Time.time >= lastDamageTime + speed)
            {
                Fire();
                lastDamageTime = Time.time;  // 마지막으로 데미지를 입힌 시간을 현재 시간으로 업데이트
            }
        }
    }

    void Fire()
    {
        if (!player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = (targetPos - transform.position).normalized;

        for (int i = 0; i < projectileCount; i++)
        {
            GameObject hammer = Instantiate(DarknessSwordPrefab, transform.position, Quaternion.identity);

            Vector3 currentDir = dir;
            if (projectileCount == 2 && i == 1)
            {
                // 2D에서의 180도 회전
                currentDir = new Vector3(-dir.x, -dir.y, dir.z);
            }

            // 칼의 발사 방향 설정
            Knife knifeScript = hammer.GetComponent<Knife>();
            if (knifeScript)
            {
                knifeScript.shootDirection = currentDir;
            }

            // 칼의 회전 설정 (적의 방향을 향하게 함)
            float angle = Mathf.Atan2(currentDir.y, currentDir.x) * Mathf.Rad2Deg;
            hammer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); // -90은 칼이 위를 향하도록 회전하는 데 필요한 오프셋입니다.

            // Rigidbody2D를 사용하여 무기 발사
            Rigidbody2D rb = hammer.GetComponent<Rigidbody2D>();
            rb.velocity = currentDir * speed;
        }
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }
        //AudioManager.instance.PlaySfx(AudioManager.Sfx.Darkness);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DarknessBullet playerHammerItem = other.gameObject.GetComponent<DarknessBullet>();
            if (playerHammerItem == null)
            {
                playerHammerItem = other.gameObject.AddComponent<DarknessBullet>();
                playerHammerItem.DarknessSwordPrefab = this.DarknessSwordPrefab; // 여기에서 HammerPrefab 값을 복사
            }
            playerHammerItem.isActivated = true;  // 아이템 활성화

            // BossDropUi를 표시
            DarknessUi.instance.ShowBossDrop();

            // 아이템을 지웁니다.
            Destroy(gameObject);
        }
    }
}
