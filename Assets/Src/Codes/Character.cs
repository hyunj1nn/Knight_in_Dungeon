using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public static Character instance;

    public int maxHp;
    public int currentHp;

    public float armor = 0.0f;

    [SerializeField] StatusBar hpBar;
    [HideInInspector] public Player playerMovement;
    [HideInInspector] public LevelUp level;

    public GameObject summonedGuardian;

    public GameObject bloodEffectPrefab;  // �� ȿ�� ������ ����
    private float lastBloodEffectTime = 0f;  // ���������� �� ȿ���� ������ �ð�
    public float bloodEffectCooldown = 0.5f;   // �� ȿ�� ���� ��ٿ� 

    [SerializeField] private AudioClip[] hitSounds; // �������� ����� �ǰ� �����
    private AudioSource audioSource;
    private Animator animator;

    private float lastSoundPlayedTime = 0f;  // ���������� ���尡 ����� �ð�
    public float soundCooldown = 0.2f;      // ���� ��� ��ٿ� 


    public void Awake()
    {
        level = GetComponent<LevelUp>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<Player>();
    }

    private void Start()
    {
        hpBar.SetState(currentHp, maxHp);
    }

    private void PlayRandomHitSound()
    {
        if (hitSounds.Length == 0) return; // ���尡 ������ ��ȯ

        // ���� �ð��� ���������� ���尡 ����� �ð��� ���̰� ��ٿ�� ũ�ų� ������ ���带 ���
        if (Time.time - lastSoundPlayedTime >= soundCooldown)
        {
            int randomIndex = UnityEngine.Random.Range(0, hitSounds.Length); // ���� �ε��� ����
            audioSource.PlayOneShot(hitSounds[randomIndex]); // ������ ���� ���
            lastSoundPlayedTime = Time.time;  // ���������� ���尡 ����� �ð� ������Ʈ
        }
    }

    public void TakeDamage(int damage)
    {
        //���� ü���� 0���϶�� ������ ó���� ����
        if (currentHp <= 0) return;

        ApplyArmor(ref damage);

        currentHp -= damage;

        PlayRandomHitSound(); // ������ �ǰ� ���� ���

        ShowBloodEffect();  // �� ȿ�� ���

        if (currentHp <= 0)
        {
            animator.SetTrigger("Dead"); // Dead Ʈ���Ÿ� Ȱ��ȭ�Ͽ� �״� �ִϸ��̼� ����
            playerMovement.canMove = false; // ������ ��Ȱ��ȭ
            level.enabled = false;  // ������ ��ũ��Ʈ ��Ȱ��ȭ
            StartCoroutine(EndGameAfterDelay(2f)); // 2�� �� ���� ����
        }

        hpBar.SetState(currentHp, maxHp);
    }

    private IEnumerator EndGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.instance.EndGame();
    }

    private void ShowBloodEffect()
    {
        if (bloodEffectPrefab != null && Time.time - lastBloodEffectTime >= bloodEffectCooldown)
        {
            GameObject bloodEffect = Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);
            // �ִϸ��̼��� ����� �� �ڵ� ���� 
            Destroy(bloodEffect, 1f);  // n�� �� ����
            lastBloodEffectTime = Time.time;  // ���������� �� ȿ���� ������ �ð� ������Ʈ
        }
    }

    public void ApplyArmor(ref int damage)
    {
        float reducedDamage = damage - armor;
        damage = Mathf.RoundToInt(reducedDamage);
        if (damage < 0) { damage = 0; }
    }

    public void Heal(int amount)
    {
        if (currentHp <= 0) { return; }

        currentHp += amount;
        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }
        hpBar.SetState(currentHp, maxHp);
    }

    public void IncreaseArmor(float amount)
    {
        armor += amount;
    }

    public void ActivateSummon()
    {
        if (summonedGuardian != null)
        {
            summonedGuardian.SetActive(true); // ��ȯ�� ���� ������Ʈ Ȱ��ȭ
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GemPickUpObject gem = other.GetComponent<GemPickUpObject>();
        if (gem != null)
        {
            gem.OnPickUp(this);
        }
    }
}
