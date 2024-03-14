using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("범위 무기 스탯")]

    [SerializeField] public float timeToAttack = 1f;
    [SerializeField] float attackRadius = 2f;
    [SerializeField] public GameObject attackObject;
    [SerializeField] float scaleIncreasePerLevel = 0.1f; // 레벨업 할 때마다 증가할 스케일의 값
    [SerializeField] float rangeIncreasePerLevel = 0.2f; // 레벨업 할 때마다 증가할 범위의 값

    [Header("무기 스탯")]

    public int id;  // 무기 ID
    public int prefabId;  // 프리펩 ID
    public int damage;  // 무기 데미지
    public int count;  // 무기 갯수
    public float speed;  // 무기 속도
    public float bulletspeed;

    float timer;
    Player player;

    void Awake()
    {
        player = GameManager.instance.player;

        // position 값을 고정하여 원하는 위치로 설정
        //transform.position = new Vector3(0f, -1.6f, 0f);
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        timer -= Time.deltaTime;
        //Debug.Log("Timer: " + timer);  // 매 프레임마다 timer 값을 출력합니다.
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
                //Debug.Log("Current timer: " + timer + ", speed: " + speed);  // timer와 speed 값을 출력합니다.

                if (timer > speed)
                {
                    timer = 0f;
                   // Fire();
                }
                break;
        }

        // 테스트
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

        attackRadius += rangeIncreasePerLevel; // 범위를 레벨업에 따라 증가시킴
        transform.localScale += Vector3.one * scaleIncreasePerLevel; // 스케일을 레벨업에 따라 증가시킴

        if (id == 0)
            Batch();

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }


    void Batch()  // 플레이어 주위를 도는 무기
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
            Sword.Translate(Sword.up * 0f, Space.World); // 플레이어 기준으로 무기 거리 배치
            Sword.GetComponent<Sword>().Init(damage, -1, Vector3.zero,bulletspeed); // -1 은 무한으로 관통하는 근접공격         
        }
    }

    void Attack()  // 플레이어 주위 범위 무기
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

