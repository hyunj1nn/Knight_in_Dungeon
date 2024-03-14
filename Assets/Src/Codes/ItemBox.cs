using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour, IDamageable
{
    public Animator anim; 
    private bool isOpened = false; // ���ڰ� �̹� ���ȴ��� Ȯ��

    public float timeBeforeRespawn = 60f; // ��: 60��

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

        // ���� �������� ���� �ڷ�ƾ�� �ٽ� �����մϴ�.
        StartCoroutine(RespawnAfterDelay(timeBeforeRespawn));
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // �̹� ���Ȱų� ���� �±װ� �ƴϸ� return
        if (isOpened || (!collision.CompareTag("Sword") && !collision.CompareTag("Slash")))
            return;

        // ���ڰ� ����� �浹���� ���� ó��
        anim.SetTrigger("Hit"); // ���� ���, "Open"�̶�� Ʈ���Ű� ������ ������ Animator�� �ִٰ� �����մϴ�.
        isOpened = true; // ���ڰ� �������� ǥ��

        // AudioSource ������Ʈ�� �ְ� ����� Ŭ���� �����Ǿ� �ִ� ��� ���� ���
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }

        StartCoroutine(DestroyAfterDelay(0f)); // 1�� �ڿ� �ı�
    }

    public void TakeDamage(int damage)
    {
        // AudioSource ������Ʈ�� �ְ� ����� Ŭ���� �����Ǿ� �ִ� ��� ���� ���
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }

        anim.SetTrigger("Hit");
        // ���� ������ �ٷ� Destroy�ϴ� ��ſ� ���� ������ ��� �ð��� ���� Coroutine�� ȣ���մϴ�.
        StartCoroutine(DestroyAfterDelay(0.6f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
