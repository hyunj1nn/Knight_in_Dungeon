using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordBullet : MonoBehaviour
{
    [SerializeField] public GameObject attackObject;

    public int id;  // ���� ID
    public int prefabId;  // ������ ID
    public int damage;  // ���� ������
    public int count;  // ���� ����
    public float speed;  // ���Ⱑ �߻�Ǵ� �ð� (�߻�Ǵ� ��)
    public float bulletspeed; // ���Ⱑ �߻�Ǵ� �ӵ�
    public int projectileCount = 1;

    Rigidbody2D rigid;

    float timer;
    Player player;


    void Awake()
    {
        player = GameManager.instance.player;
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            default:
                timer += Time.deltaTime;
                if (timer > speed)
                {
                    timer = 0f;
                    Fire();
                }
                break;
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
        name = "SwordBullet" + data.itemId;
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
                speed = 2f;
                bulletspeed = 15f;
                break;
        }
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    void Fire()
    {
        // �÷��̾ ������ ���� ������ ��������, �������� ���� ������ ���������� �߻�
        Vector3 dir = player.spriter.flipX ? new Vector3(-1, 0, 0) : new Vector3(1, 0, 0);
        dir = dir.normalized;

        // �߻�ü ������ �������� �����մϴ�. �߻�ü�� ���� ���� ����
        float incrementAngle = 90f / projectileCount;

        for (int i = 0; i < projectileCount; i++)
        {
            Transform Sword = GameManager.instance.pool.Get(prefabId).transform;
            Sword.position = transform.position;
            // �� �߻�ü�� ���� ���� ����
            float currentAngle = incrementAngle * i; 
            Vector3 projectileDir = new Vector3(dir.x * Mathf.Cos(currentAngle * Mathf.Deg2Rad) - dir.y * Mathf.Sin(currentAngle * Mathf.Deg2Rad),
                dir.x * Mathf.Sin(currentAngle * Mathf.Deg2Rad) + dir.y * Mathf.Cos(currentAngle * Mathf.Deg2Rad), 0).normalized;
            Sword.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(projectileDir.y, projectileDir.x) * Mathf.Rad2Deg);
            Sword.GetComponent<Sword>().Init(damage, count, projectileDir, bulletspeed);
        }
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }
    }


    void ApplyDamage(Collider2D[] colliders)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            Enemy enemy = colliders[i].gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
