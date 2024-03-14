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
    private WhipWeapon clonedWhip;  // 복제된 무기 참조
    public bool isBossDropItemAcquired = false;

    public int id;  // 무기 ID
    public int prefabId;  // 프리펩 ID
    public int damage;  // 무기 데미지
    public int count;  // 무기 갯수
    public float speed;  // 무기 속도

    private void Awake()
    {
        player = GameManager.instance.player;
    }

    void Start()
    {
        if (isBossDropItemAcquired)
        {
            this.enabled = false;  // 복제된 무기의 Update를 비활성화합니다.
        }
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        timer -= Time.deltaTime;

        // 원래의 무기(whipweapon) 로직
        if (timer < 0f)
        {
            Attack();
        }

        // clonedWhip 로직 (복제된 무기)
        if (clonedWhip != null)
        {
            clonedWhip.timer -= Time.deltaTime;

            if (clonedWhip.timer < 0f)
            {
                clonedWhip.Attack();
                clonedWhip.timer = clonedWhip.timeToAttack;  // 복제된 무기의 timer를 재설정합니다.
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

        timer = timeToAttack; // 기존의 무기 timer 초기화

        // 복제된 무기의 속성값도 업데이트합니다.
        if (clonedWhip != null)
        {
            clonedWhip.damage = this.damage;
            clonedWhip.whipDamage = this.whipDamage;
            clonedWhip.count = this.count;

            clonedWhip.timer = timeToAttack; // 복제된 무기 timer 초기화
        }

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    public void AcquireBossDropItem()
    {
        if (isBossDropItemAcquired == false)
        {
            isBossDropItemAcquired = true;

            // 현재 WhipWeapon의 복제본을 생성합니다.
            clonedWhip = Instantiate(this, player.transform);

            // Y축을 기준으로 180도 회전하여 무기를 뒤집습니다.
            clonedWhip.transform.localRotation = Quaternion.Euler(180, 180, 0);

            // 복제본의 timer와 timeToAttack 값을 원본의 값과 동일하게 설정
            clonedWhip.timer = this.timer;
            clonedWhip.timeToAttack = this.timeToAttack; // 이 줄을 추가
        }
    }

    private void Attack()
    {
        timer = timeToAttack;

        if (player.lastHorizontalVector > 0)  // 오른쪽으로 공격
        {
            rightWhipObject.SetActive(true);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(rightWhipObject.transform.position, whipAttackSize, 0f);
            ApplyDamage(colliders);
        }
        else   // 왼쪽으로 공격
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

        whipDamage = damage; // 무기 데미지 초기화

        for (int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            if (data.projectile == GameManager.instance.pool.prefabs[index])
            {
                prefabId = index;
                break;
            }
        }

        rightWhipObject = Instantiate(rightPrefab, transform);
        rightWhipObject.transform.localPosition = new Vector3(2.28f, 0.01f, 0f); // rightWhipObject의 위치 설정

        leftWhipObject = Instantiate(leftPrefab, transform);
        leftWhipObject.transform.localPosition = new Vector3(-2.28f, 0.01f, 0f); // leftWhipObject의 위치 설정

        // 기타 초기화 로직

        // 이후의 코드에서 rightWhipObject와 leftWhipObject를 사용할 수 있도록 설정
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }
}