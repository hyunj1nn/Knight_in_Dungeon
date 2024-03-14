using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour, IDamageable
{
    public Animator anim; 
    private bool isOpened = false; // 상자가 이미 열렸는지 확인

    public float timeBeforeRespawn = 60f; // 예: 60초

    private RespawnManager respawnManager;
    private AudioSource audioSource;

    private void Start()
    {
        respawnManager = FindObjectOfType<RespawnManager>();
        StartCoroutine(RespawnAfterDelay(timeBeforeRespawn));

        audioSource = GetComponent<AudioSource>();
    }

    private IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 newPosition = respawnManager.GenerateRandomSpawnPosition();
        transform.position = newPosition;
        isOpened = false;

        // 다음 리스폰을 위해 코루틴을 다시 시작합니다.
        StartCoroutine(RespawnAfterDelay(timeBeforeRespawn));
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 이미 열렸거나 무기 태그가 아니면 return
        if (isOpened || (!collision.CompareTag("Sword") && !collision.CompareTag("Slash")))
            return;

        // 상자가 무기와 충돌했을 때의 처리
        anim.SetTrigger("Hit"); // 예를 들어, "Open"이라는 트리거가 아이템 상자의 Animator에 있다고 가정합니다.
        isOpened = true; // 상자가 열렸음을 표시

        // AudioSource 컴포넌트가 있고 오디오 클립이 설정되어 있는 경우 사운드 재생
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }

        StartCoroutine(DestroyAfterDelay(0f)); // 1초 뒤에 파괴
    }

    public void TakeDamage(int damage)
    {
        // AudioSource 컴포넌트가 있고 오디오 클립이 설정되어 있는 경우 사운드 재생
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }

        anim.SetTrigger("Hit");
        // 이전 로직을 바로 Destroy하는 대신에 위와 동일한 대기 시간을 가진 Coroutine을 호출합니다.
        StartCoroutine(DestroyAfterDelay(0.6f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
