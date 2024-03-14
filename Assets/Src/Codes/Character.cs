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

    public GameObject bloodEffectPrefab;  // 피 효과 프리팹 참조
    private float lastBloodEffectTime = 0f;  // 마지막으로 피 효과가 생성된 시간
    public float bloodEffectCooldown = 0.5f;   // 피 효과 생성 쿨다운 

    [SerializeField] private AudioClip[] hitSounds; // 랜덤으로 재생될 피격 사운드들
    private AudioSource audioSource;
    private Animator animator;

    private float lastSoundPlayedTime = 0f;  // 마지막으로 사운드가 재생된 시간
    public float soundCooldown = 0.2f;      // 사운드 재생 쿨다운 


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
        if (hitSounds.Length == 0) return; // 사운드가 없으면 반환

        // 현재 시간과 마지막으로 사운드가 재생된 시간의 차이가 쿨다운보다 크거나 같으면 사운드를 재생
        if (Time.time - lastSoundPlayedTime >= soundCooldown)
        {
            int randomIndex = UnityEngine.Random.Range(0, hitSounds.Length); // 랜덤 인덱스 선택
            audioSource.PlayOneShot(hitSounds[randomIndex]); // 랜덤한 사운드 재생
            lastSoundPlayedTime = Time.time;  // 마지막으로 사운드가 재생된 시간 업데이트
        }
    }

    public void TakeDamage(int damage)
    {
        //현재 체력이 0이하라면 데미지 처리를 중지
        if (currentHp <= 0) return;

        ApplyArmor(ref damage);

        currentHp -= damage;

        PlayRandomHitSound(); // 랜덤한 피격 사운드 재생

        ShowBloodEffect();  // 피 효과 출력

        if (currentHp <= 0)
        {
            animator.SetTrigger("Dead"); // Dead 트리거를 활성화하여 죽는 애니메이션 실행
            playerMovement.canMove = false; // 움직임 비활성화
            level.enabled = false;  // 레벨업 스크립트 비활성화
            StartCoroutine(EndGameAfterDelay(2f)); // 2초 후 게임 종료
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
            // 애니메이션이 종료된 후 자동 삭제 
            Destroy(bloodEffect, 1f);  // n초 후 삭제
            lastBloodEffectTime = Time.time;  // 마지막으로 피 효과가 생성된 시간 업데이트
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
            summonedGuardian.SetActive(true); // 소환수 게임 오브젝트 활성화
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
