using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarknessBullet : MonoBehaviour
{
    [SerializeField] public GameObject attackObject;
    [SerializeField] public GameObject DarknessSwordPrefab;
    public int projectileCount = 2; // �߻��� ������ ����
    public int id;  // ���� ID
    public int prefabId;  // ������ ID
    public int damage = 100;  // ���� ������
    public float detectionRadius = 9.0f;  // ��� ���� ������ �����ϴ� ����
    public float speed = 1.7f; // �������� ������ ����(��)

    private float lastDamageTime = 0.0f;
    private bool isActivated = false;  // �������� Ȱ��ȭ �Ǿ����� ��Ÿ���� ����
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
            if (Time.time >= lastDamageTime + speed)
            {
                Fire();
                lastDamageTime = Time.time;  // ���������� �������� ���� �ð��� ���� �ð����� ������Ʈ
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
                // 2D������ 180�� ȸ��
                currentDir = new Vector3(-dir.x, -dir.y, dir.z);
            }

            // Į�� �߻� ���� ����
            Knife knifeScript = hammer.GetComponent<Knife>();
            if (knifeScript)
            {
                knifeScript.shootDirection = currentDir;
            }

            // Į�� ȸ�� ���� (���� ������ ���ϰ� ��)
            float angle = Mathf.Atan2(currentDir.y, currentDir.x) * Mathf.Rad2Deg;
            hammer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); // -90�� Į�� ���� ���ϵ��� ȸ���ϴ� �� �ʿ��� �������Դϴ�.

            // Rigidbody2D�� ����Ͽ� ���� �߻�
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
                playerHammerItem.DarknessSwordPrefab = this.DarknessSwordPrefab; // ���⿡�� HammerPrefab ���� ����
            }
            playerHammerItem.isActivated = true;  // ������ Ȱ��ȭ

            // BossDropUi�� ǥ��
            DarknessUi.instance.ShowBossDrop();

            // �������� ����ϴ�.
            Destroy(gameObject);
        }
    }
}
