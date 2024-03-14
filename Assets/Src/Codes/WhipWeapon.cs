using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipWeapon : MonoBehaviour
{
    [SerializeField] public float timeToAttack = 2f;
    float timer;

    [SerializeField] GameObject leftWhipObject;
    [SerializeField] GameObject rightWhipObject;

    Player player;
    [SerializeField] Vector2 whipAttackSize = new Vector2(5f, 2f);
    [SerializeField] int whipDamage;

    private WhipWeapon backWhip;
    private WhipWeapon clonedWhip;  // ������ ���� ����
    public bool isBossDropItemAcquired = false;

    public int id;  // ���� ID
    public int prefabId;  // ������ ID
    public int damage;  // ���� ������
    public int count;  // ���� ����
    public float speed;  // ���� �ӵ�

    private void Awake()
    {
        player = GameManager.instance.player;
    }

    void Start()
    {
        if (isBossDropItemAcquired)
        {
            this.enabled = false;  // ������ ������ Update�� ��Ȱ��ȭ�մϴ�.
        }
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        timer -= Time.deltaTime;

        // ������ ����(whipweapon) ����
        if (timer < 0f)
        {
            Attack();
        }

        // clonedWhip ���� (������ ����)
        if (clonedWhip != null)
        {
            clonedWhip.timer -= Time.deltaTime;

            if (clonedWhip.timer < 0f)
            {
                clonedWhip.Attack();
                clonedWhip.timer = clonedWhip.timeToAttack;  // ������ ������ timer�� �缳���մϴ�.
            }
        }
    }

    public void LevelUp(float damageValue, int countValue)
    {
        this.damage = (int)damageValue;
        this.count += countValue;

        if (id == 0)
        {
            whipDamage = this.damage;
        }

        timer = timeToAttack; // ������ ���� timer �ʱ�ȭ

        // ������ ������ �Ӽ����� ������Ʈ�մϴ�.
        if (clonedWhip != null)
        {
            clonedWhip.damage = this.damage;
            clonedWhip.whipDamage = this.whipDamage;
            clonedWhip.count = this.count;

            clonedWhip.timer = timeToAttack; // ������ ���� timer �ʱ�ȭ
        }

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    public void AcquireBossDropItem()
    {
        if (isBossDropItemAcquired == false)
        {
            isBossDropItemAcquired = true;

            // ���� WhipWeapon�� �������� �����մϴ�.
            clonedWhip = Instantiate(this, player.transform);

            // Y���� �������� 180�� ȸ���Ͽ� ���⸦ �������ϴ�.
            clonedWhip.transform.localRotation = Quaternion.Euler(180, 180, 0);

            // �������� timer�� timeToAttack ���� ������ ���� �����ϰ� ����
            clonedWhip.timer = this.timer;
            clonedWhip.timeToAttack = this.timeToAttack; // �� ���� �߰�
        }
    }

    private void Attack()
    {
        timer = timeToAttack;

        if (player.lastHorizontalVector > 0)  // ���������� ����
        {
            rightWhipObject.SetActive(true);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(rightWhipObject.transform.position, whipAttackSize, 0f);
            ApplyDamage(colliders);
        }
        else   // �������� ����
        {
            leftWhipObject.SetActive(true);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(leftWhipObject.transform.position, whipAttackSize, 0f);
            ApplyDamage(colliders);
        }

        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }
        //AudioManager.instance.PlaySfx(AudioManager.Sfx.Whip);
    }

    private void ApplyDamage(Collider2D[] colliders)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            IDamageable enemy = colliders[i].GetComponent<IDamageable>();
            if (enemy != null)
            {
                enemy.TakeDamage((int)damage);
            }
        }
    }

    public void Init(ItemData data, GameObject rightPrefab, GameObject leftPrefab)
    {
        name = "WhipWeapon" + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        id = data.itemId;
        damage = (int)data.baseDamage;
        count = data.baseCount;

        whipDamage = damage; // ���� ������ �ʱ�ȭ

        for (int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            if (data.projectile == GameManager.instance.pool.prefabs[index])
            {
                prefabId = index;
                break;
            }
        }

        rightWhipObject = Instantiate(rightPrefab, transform);
        rightWhipObject.transform.localPosition = new Vector3(2.28f, 0.01f, 0f); // rightWhipObject�� ��ġ ����

        leftWhipObject = Instantiate(leftPrefab, transform);
        leftWhipObject.transform.localPosition = new Vector3(-2.28f, 0.01f, 0f); // leftWhipObject�� ��ġ ����

        // ��Ÿ �ʱ�ȭ ����

        // ������ �ڵ忡�� rightWhipObject�� leftWhipObject�� ����� �� �ֵ��� ����
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }
}