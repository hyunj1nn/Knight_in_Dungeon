using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] int healAmount;
    [SerializeField] float magnetSpeed = 5.0f;  // �ڼ� ȿ���� �ӵ�
    private bool isAttracted = false;  // �÷��̾�� ������ �ִ����� ����
    private bool bounceEffect = false;  // ƨ���� ������ ȿ���� ����
    [SerializeField] float bounceForce = 2.0f;  // ƨ���� ������ ��
    //[SerializeField] float acceleration = 0.5f;  // ���ӵ� ��

    private float currentMagnetSpeed;  // ���� �ڼ� ȿ���� �ӵ�

    private Vector2 currentVelocity;  // SmoothDamp�� �ʿ��� ����
    private float smoothTime = 0.1f;  // �������� �ε巯�� ������ �����մϴ�.
    private AudioSource audioSource;

    private Transform playerTransform;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (bounceEffect)
        {
            // �ݴ� �������� ƨ���� �������� �մϴ�.
            Vector2 bounceDirection = (transform.position - playerTransform.position).normalized;
            transform.position += (Vector3)bounceDirection * bounceForce * Time.deltaTime;

            StartCoroutine(BounceTimer());  // ���� �ð� �� ƨ��� ȿ�� ����
        }

        else if (isAttracted)
        {
            transform.position = Vector2.SmoothDamp(transform.position, playerTransform.position, ref currentVelocity, smoothTime, Mathf.Infinity, Time.deltaTime);

            // �÷��̾�� ����� ������
            if (Vector2.Distance(transform.position, playerTransform.position) < 0.5f)
            {
                //AudioManager.instance.PlaySfx(AudioManager.Sfx.GemPickup, 0.5f); // ���� ���� (���⼭�� 0.5�� ����)

                Character c = playerTransform.GetComponent<Character>();
                if (c != null)
                {
                    IPickUpObject pickupObject = GetComponent<IPickUpObject>();
                    if (pickupObject != null)
                    {
                        pickupObject.OnPickUp(c);
                        Destroy(gameObject);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Character c = collision.GetComponent<Character>();
        if (c != null)
        {
            bounceEffect = true;  // ���� ƨ�ܳ����� ȿ���� Ȱ��ȭ�մϴ�.
            playerTransform = collision.transform;
            currentMagnetSpeed = magnetSpeed;  // �ڼ� ȿ���� ���۵� ���� �ʱ� �ӵ��� �����մϴ�.

            // ������ ���� �� 0.5�� �ڿ� �Ҹ� ���
            StartCoroutine(PlayPickupSoundAfterDelay(0.3f));
        }
    }

    IEnumerator BounceTimer()
    {
        yield return new WaitForSeconds(0.5f);  // ��: 0.5�� ���� ƨ��� ȿ���� �־����ϴ�.
        bounceEffect = false;
        isAttracted = true;
    }

    IEnumerator PlayPickupSoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }
    }
}
