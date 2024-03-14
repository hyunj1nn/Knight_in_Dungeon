using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("���� ���� ����")]

    [SerializeField] public float timeToAttack = 1f;
    [SerializeField] float attackRadius = 2f;
    [SerializeField] public GameObject attackObject;
    [SerializeField] float scaleIncreasePerLevel = 0.1f; // ������ �� ������ ������ �������� ��
    [SerializeField] float rangeIncreasePerLevel = 0.2f; // ������ �� ������ ������ ������ ��

    [Header("���� ����")]

    public int id;  // ���� ID
    public int prefabId;  // ������ ID
    public int damage;  // ���� ������
    public int count;  // ���� ����
    public float speed;  // ���� �ӵ�
    public float bulletspeed;

    float timer;
    Player player;

    void Awake()
    {
        player = GameManager.instance.player;

        // position ���� �����Ͽ� ���ϴ� ��ġ�� ����
        //transform.position = new Vector3(0f, -1.6f, 0f);
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        timer -= Time.deltaTime;
        //Debug.Log("Timer: " + timer);  // �� �����Ӹ��� timer ���� ����մϴ�.
        if (timer < 0f)
        {
            Attack();
        }

        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            default:
                timer += Time.deltaTime;
                //Debug.Log("Current timer: " + timer + ", speed: " + speed);  // timer�� speed ���� ����մϴ�.

                if (timer > speed)
                {
                    timer = 0f;
                   // Fire();
                }
                break;
        }

        // �׽�Ʈ
        //if (Input.GetButtonDown("Jump"))
        //{
        //LevelUp(20, 2);
        //}
    }

    public void Init(ItemData data)
    {
        name = "Weapon" + data.itemId;
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
            case 0:
                speed = 150;
                bulletspeed = 10f;
                Batch();
                break; 
        }
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }
    public void LevelUp(float damage, int count)
    {
        this.damage = (int)damage;
        this.count += count;

        attackRadius += rangeIncreasePerLevel; // ������ �������� ���� ������Ŵ
        transform.localScale += Vector3.one * scaleIncreasePerLevel; // �������� �������� ���� ������Ŵ

        if (id == 0)
            Batch();

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }


    void Batch()  // �÷��̾� ������ ���� ����
    {
        for (int index = 0; index < count; index++)
        {
            Transform Sword;

            if (index < transform.childCount)
            {
                Sword = transform.GetChild(index);
            }
            else
            {
                Sword = GameManager.instance.pool.Get(prefabId).transform;
                Sword.parent = transform;

            }

            Sword.localPosition = Vector3.zero;
            Sword.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            Sword.Rotate(rotVec);
            Sword.Translate(Sword.up * 0f, Space.World); // �÷��̾� �������� ���� �Ÿ� ��ġ
            Sword.GetComponent<Sword>().Init(damage, -1, Vector3.zero,bulletspeed); // -1 �� �������� �����ϴ� ��������         
        }
    }

    void Attack()  // �÷��̾� ���� ���� ����
    {
        timer = timeToAttack;

        Vector3 attackPosition = player.transform.position;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPosition, attackRadius);
        ApplyDamage(colliders);

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

