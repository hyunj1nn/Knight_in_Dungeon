using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("UI Settings")]
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private Vector3 damageTextOffset;

    [Header("����ġ ��")]
    [SerializeField] private GameObject dropItemPrefab;
    [SerializeField] private GameObject redExpJamPrefab; // ���� ����ġ�� ������
    [SerializeField] private GameObject purpleExpJamPrefab; // ����� ����ġ�� ������
    [SerializeField] [Range(0f, 1f)] private float redExpJamDropChance = 0.8f;  // �⺻ 80% Ȯ��

    [Header("�� �ɷ�ġ")]
    public float speed;
    public float originalSpeed;
    public float health = 100f;
    public float maxHealth = 100f;
    public StatusBar healthBar; // Inspector���� ü�¹� ������Ʈ ����
    public int damage;
    public bool isBoss = false;
    private int experience_reward;
    public bool isFrozen = false;

    [SerializeField] private AudioClip[] hitSounds; // �������� ����� �ǰ� �����
    private AudioSource audioSource;


    Character targetCharacter;
    GameObject targetGameobject;
    public float knockbackIntensity = 3.0f;
    public Rigidbody2D target; // ���������� �÷��̾ ���󰡴� ����
    public RuntimeAnimatorController[] animCon;
    bool isLive; // �÷��̾��� ���� ����

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
        if (healthBar != null) // ü�¹ٰ� �����Ǿ� �ִ��� Ȯ��
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
            if (!isLive) // ������� �������� ����
                return;
        }
        else
        {
            if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) // ������� �������� ����
                return;
        }

        Vector2 dirVec = target.position - rigid.position;  // ���Ϳ� �÷��̾� ������ �Ÿ� = Ÿ�� ��ġ - �ڽ� ��ġ
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime; // ���� �Ÿ� ����ȭ
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero; // ���� ���˽� ƨ���� ���� ����
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (!isLive) // ������� �������� ����
            return;

        spriter.flipX = target.position.x < rigid.position.x; // �÷��̾ ���� ������ ���� ��ȯ
    }

    void OnEnable() // ��ũ��Ʈ�� Ȱ��ȭ �� �� ȣ��Ǵ� �Լ�
    {
        // ������ ���� ������Ʈ�� Ÿ�� ���� �Ұ�, ���Ͱ� Ÿ���� ����
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true; // rigidbody�� ������ Ȱ��ȭ�� simulated = true
        spriter.sortingOrder = 5;  // ���� ��������� ���̾�
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

    private void OnCollisionStay2D(Collision2D collision)   // �÷��̾� ����
    {
        if (collision.gameObject.GetComponent<Character>())
        {
            Attack();
        }
    }

    protected virtual void Attack()
    {
        if (speed == 0) return; // ����� ���� ��� �������� ���� ����

        if (targetCharacter == null)
        {
            targetCharacter = target.GetComponent<Character>();
        }

        targetCharacter.TakeDamage(damage);
    }

    private void PlayRandomHitSound()
    {
        if (hitSounds.Length == 0) return; // ���尡 ������ ��ȯ

        int randomIndex = UnityEngine.Random.Range(0, hitSounds.Length); // ���� �ε��� ����
        audioSource.PlayOneShot(hitSounds[randomIndex]); // ������ ���� ���
    }

    void OnTriggerEnter2D(Collider2D collision) // ���Ϳ� ���� �浹 ����  // void OnTriggerEnter2D
    {
        if ((!collision.CompareTag("Sword") && !collision.CompareTag("Slash")) || !isLive)  // !isLive ����ؼ� ����������� �Ʒ� �ڵ� ����
            return;

        health -= collision.GetComponent<Sword>().damage;

        PlayRandomHitSound(); // ������ �ǰ� ���� ���


        StartCoroutine(KnockBack());

        if (health > 0) // ü���� 0�̻� �϶� �ǰ�
        {
            anim.SetTrigger("Hit");
        }
        else // ���� ���
        {
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false; // rigidbody�� ������ ��Ȱ��ȭ�� simulated = false
            spriter.sortingOrder = 4;  // ���Ͱ� �׾� �÷��̾�� ���͸� ������ �ʱ����� ���̾� �� �ܰ� ����
            anim.SetBool("Dead", true);
            GameManager.instance.kill++;
            //GameManager.instance.GetExp();
        }

    }

    IEnumerator KnockBack()
    {
        yield return new WaitForEndOfFrame(); // �������� ������ ���

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
        float dropChance = UnityEngine.Random.Range(0f, 1f); // 0.0�� 1.0 ������ ������ ���� ����ϴ�.

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

        // ������ �ؽ�Ʈ �ν��Ͻ�ȭ
        GameObject damageTextObject = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
        DamageText damageText = damageTextObject.GetComponent<DamageText>();
        damageText.ShowDamage(whipDamage);
   
        PlayRandomHitSound(); // ������ �ǰ� ���� ���

        if (healthBar != null) // ü�¹ٰ� �����Ǿ� �ִ��� Ȯ��
        {
            healthBar.SetState(health, maxHealth); // ü�¹� ������Ʈ
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

        // ���� �ð� �Ŀ� ������ �ؽ�Ʈ ���� �Ǵ� ��Ȱ��ȭ
        Destroy(damageTextObject, 2f); // ����: 2�� �� ����
    }
}
