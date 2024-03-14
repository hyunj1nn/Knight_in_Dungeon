using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] int healAmount;
    [SerializeField] float magnetSpeed = 5.0f;  // 자석 효과의 속도
    private bool isAttracted = false;  // 플레이어에게 끌리고 있는지의 상태
    private bool bounceEffect = false;  // 튕겨져 나가는 효과의 상태
    [SerializeField] float bounceForce = 2.0f;  // 튕겨져 나가는 힘
    //[SerializeField] float acceleration = 0.5f;  // 가속도 값

    private float currentMagnetSpeed;  // 현재 자석 효과의 속도

    private Vector2 currentVelocity;  // SmoothDamp에 필요한 변수
    private float smoothTime = 0.1f;  // 움직임의 부드러움 정도를 조정합니다.
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
            // 반대 방향으로 튕겨져 나가도록 합니다.
            Vector2 bounceDirection = (transform.position - playerTransform.position).normalized;
            transform.position += (Vector3)bounceDirection * bounceForce * Time.deltaTime;

            StartCoroutine(BounceTimer());  // 일정 시간 후 튕기는 효과 종료
        }

        else if (isAttracted)
        {
            transform.position = Vector2.SmoothDamp(transform.position, playerTransform.position, ref currentVelocity, smoothTime, Mathf.Infinity, Time.deltaTime);

            // 플레이어와 충분히 가까우면
            if (Vector2.Distance(transform.position, playerTransform.position) < 0.5f)
            {
                //AudioManager.instance.PlaySfx(AudioManager.Sfx.GemPickup, 0.5f); // 볼륨 설정 (여기서는 0.5로 설정)

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
            bounceEffect = true;  // 먼저 튕겨나가는 효과를 활성화합니다.
            playerTransform = collision.transform;
            currentMagnetSpeed = magnetSpeed;  // 자석 효과가 시작될 때의 초기 속도를 설정합니다.

            // 아이템 먹은 후 0.5초 뒤에 소리 재생
            StartCoroutine(PlayPickupSoundAfterDelay(0.3f));
        }
    }

    IEnumerator BounceTimer()
    {
        yield return new WaitForSeconds(0.5f);  // 예: 0.5초 동안 튕기는 효과를 주었습니다.
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
