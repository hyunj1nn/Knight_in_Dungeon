using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetItemPickUp : MonoBehaviour, IPickUpObject
{
    public ItemWaypoint waypointController;

    [SerializeField] private float duration = 10.0f;  // �ڼ� ȿ���� ���ӵǴ� �ð�

    [SerializeField] float magnetSpeed = 5.0f;  // �ڼ� ȿ���� �ӵ�
    private bool isAttracted = false;  // �÷��̾�� ������ �ִ����� ����
    private bool bounceEffect = false;  // ƨ���� ������ ȿ���� ����

    [SerializeField] float bounceForce = 2.0f;  // ƨ���� ������ ��
    private float currentMagnetSpeed;  // ���� �ڼ� ȿ���� �ӵ�
    private Vector2 currentVelocity;  // SmoothDamp�� �ʿ��� ����
    private float smoothTime = 0.1f;  // �������� �ε巯�� ������ �����մϴ�.
    
    private Transform playerTransform;
    private AudioSource audioSource;

    public void OnPickUp(Character character)
    {

    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (waypointController != null)  // waypointController�� null�� �ƴ� ���� ȣ��
        {
            waypointController.CreateWaypoint(transform.position);
        }
        else
        {
            Debug.LogWarning("WaypointController is not assigned in BossDropItem.");
        }
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
            if (Vector2.Distance(transform.position, playerTransform.position) < 0.5f)  // 0.5�� ����� �Ÿ�, �ʿ信 ���� �����ϼ���.
            {
                Character c = playerTransform.GetComponent<Character>();
                if (c != null)
                {
                    IPickUpObject pickupObject = GetComponent<IPickUpObject>();
                    if (pickupObject != null)
                    {
                        pickupObject.OnPickUp(c);
                        Destroy(gameObject); // ���⿡ ������ ������Ʈ ���� ������ �߰��մϴ�.
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            MagnetItem magnetEffect = collision.GetComponent<MagnetItem>();
            if (magnetEffect != null)
            {
                magnetEffect.ActivateMagnet(duration);  // �ڼ� ȿ���� Ȱ��ȭ�մϴ�.

                if (audioSource != null && audioSource.clip != null)
                {
                    audioSource.PlayOneShot(audioSource.clip);
                }
                //Destroy(gameObject);  // ������ ������ ���� (�ʿ��ϸ� �ּ��� �����ϼ���)
            }

            // �߰��� ����
            Character c = collision.GetComponent<Character>();
            if (c != null)
            {
                bounceEffect = true;  // ���� ƨ�ܳ����� ȿ���� Ȱ��ȭ�մϴ�.
                playerTransform = collision.transform;
                currentMagnetSpeed = magnetSpeed;  // �ڼ� ȿ���� ���۵� ���� �ʱ� �ӵ��� �����մϴ�.
            }
        }
    }

    IEnumerator BounceTimer()
    {
        yield return new WaitForSeconds(0.5f);  // ��: 0.5�� ���� ƨ��� ȿ���� �־����ϴ�.
        bounceEffect = false;
        isAttracted = true;
    }
}
