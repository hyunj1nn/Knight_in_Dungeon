using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("UI Settings")]
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private Vector3 damageTextOffset;

    [Header("경험치 잼")]
    [SerializeField] private GameObject dropItemPrefab;
    [SerializeField] private GameObject redExpJamPrefab; // 빨간 경험치잼 프리팹
    [SerializeField] private GameObject purpleExpJamPrefab; // 보라색 경험치잼 프리팹
    [SerializeField] [Range(0f, 1f)] private float redExpJamDropChance = 0.8f;  // 기본 80% 확률

    [Header("적 능력치")]
    public float speed;
    public float originalSpeed;
    public float health = 100f;
    public float maxHealth = 100f;
    public StatusBar healthBar; // Inspector에서 체력바 오브젝트 연결
    public int damage;
    public bool isBoss = false;
    private int experience_reward;
    public bool isFrozen = false;

    [SerializeField] private AudioClip[] hitSounds; // 랜덤으로 재생될 피격 사운드들
    private AudioSource audioSource;


    Character targetCharacter;
    GameObject targetGameobject;
    public float knockbackIntensity = 3.0f;
    public Rigidbody2D target; // 물리적으로 플레이어를 따라가는 변수
    public RuntimeAnimatorController[] animCon;
    bool isLive; // 플레이어의 생존 여부

    private Rigidbody2D rigid;
    private Collider2D coll;
    private Animator anim;
    private SpriteRenderer spriter;
    private WaitForFixedUpdate wait;


    void Awake()
    {
        originalSpeed = speed;

        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        wait = new WaitForFixedUpdate();
        audioSource = GetComponent<AudioSource>();

    }

    private void Start()
    {
        health = maxHealth;
        if (healthBar != null) // 체력바가 설정되어 있는지 확인
        {
            healthBar.SetState(health, maxHealth);
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (isBoss)
        {
            if (!isLive) // 살아있지 않을때는 종료
                return;
        }
        else
        {
            if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) // 살아있지 않을때는 종료
                return;
        }

        Vector2 dirVec = target.position - rigid.position;  // 몬스터와 플레이어 사이의 거리 = 타겟 위치 - 자신 위치
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime; // 사이 거리 정규화
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero; // 몬스터 접촉시 튕겨져 나감 방지
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (!isLive) // 살아있지 않을때는 종료
            return;

        spriter.flipX = target.position.x < rigid.position.x; // 플레이어를 상대로 몬스터의 방향 전환
    }

    void OnEnable() // 스크립트가 활성화 될 때 호출되는 함수
    {
        // 프리팹 사용시 오브젝트로 타겟 설정 불가, 몬스터가 타겟을 지정
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true; // rigidbody의 물리적 활성화는 simulated = true
        spriter.sortingOrder = 5;  // 몬스터 살아있을때 레이어
        anim.SetBool("Dead", false);
        health = maxHealth;
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
        damage = data.damage;
    }

    private void OnCollisionStay2D(Collision2D collision)   // 플레이어 공격
    {
        if (collision.gameObject.GetComponent<Character>())
        {
            Attack();
        }
    }

    protected virtual void Attack()
    {
        if (speed == 0) return; // 얼려져 있을 경우 데미지를 주지 않음

        if (targetCharacter == null)
        {
            targetCharacter = target.GetComponent<Character>();
        }

        targetCharacter.TakeDamage(damage);
    }

    private void PlayRandomHitSound()
    {
        if (hitSounds.Length == 0) return; // 사운드가 없으면 반환

        int randomIndex = UnityEngine.Random.Range(0, hitSounds.Length); // 랜덤 인덱스 선택
        audioSource.PlayOneShot(hitSounds[randomIndex]); // 랜덤한 사운드 재생
    }

    void OnTriggerEnter2D(Collider2D collision) // 몬스터와 무기 충돌 로직  // void OnTriggerEnter2D
    {
        if ((!collision.CompareTag("Sword") && !collision.CompareTag("Slash")) || !isLive)  // !isLive 사용해서 살아있을때만 아래 코드 실행
            return;

        health -= collision.GetComponent<Sword>().damage;

        PlayRandomHitSound(); // 랜덤한 피격 사운드 재생


        StartCoroutine(KnockBack());

        if (health > 0) // 체력이 0이상 일때 피격
        {
            anim.SetTrigger("Hit");
        }
        else // 몬스터 사망
        {
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false; // rigidbody의 물리적 비활성화는 simulated = false
            spriter.sortingOrder = 4;  // 몬스터가 죽어 플레이어와 몬스터를 가리지 않기위해 레이어 한 단계 감소
            anim.SetBool("Dead", true);
            GameManager.instance.kill++;
            //GameManager.instance.GetExp();
        }

    }

    IEnumerator KnockBack()
    {
        yield return new WaitForEndOfFrame(); // 프레임의 끝까지 대기

        Vector3 PlayerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - PlayerPos;

        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
    }

    void Dead()
    {

        if (dropItemPrefab != null)
        {
            //if (dropItemPrefab != null)
            {
                GameObject dropItem = Instantiate(dropItemPrefab, transform.position, Quaternion.identity);
                dropItem.transform.SetParent(DropItemManager.Instance.dropItemParent.transform);
                dropItem.SetActive(true);
            }
            gameObject.SetActive(false);
        }

        DropItemBasedOnChance();

        gameObject.SetActive(false);
    }
    
    void DropItemBasedOnChance()
    {
        float dropChance = UnityEngine.Random.Range(0f, 1f); // 0.0과 1.0 사이의 랜덤한 수를 얻습니다.

        GameObject dropPrefab = null;

        if (dropChance <= redExpJamDropChance)
        {
            dropPrefab = redExpJamPrefab;
        }
        else
        {
            dropPrefab = purpleExpJamPrefab;
        }

        if (dropPrefab != null)
        {
            GameObject dropItem = Instantiate(dropPrefab, transform.position, Quaternion.identity);
            dropItem.transform.SetParent(DropItemManager.Instance.dropItemParent.transform);
            dropItem.SetActive(true);
        }
    }


    public void TakeDamage(int whipDamage)
    {
        health -= whipDamage;

        // 데미지 텍스트 인스턴스화
        GameObject damageTextObject = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
        DamageText damageText = damageTextObject.GetComponent<DamageText>();
        damageText.ShowDamage(whipDamage);
   
        PlayRandomHitSound(); // 랜덤한 피격 사운드 재생

        if (healthBar != null) // 체력바가 설정되어 있는지 확인
        {
            healthBar.SetState(health, maxHealth); // 체력바 업데이트
        }

        if (health <= 0)
        {
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 4;
            anim.SetBool("Dead", true);
            GameManager.instance.kill++;
            //GameManager.instance.GetExp();
            //GameManager.instance.player.GetComponent<LevelUp>().AddExperience(experience_reward);
        }
        else
        {
            anim.SetTrigger("Hit");
        }

        // 일정 시간 후에 데미지 텍스트 삭제 또는 비활성화
        Destroy(damageTextObject, 2f); // 예시: 2초 후 삭제
    }
}
