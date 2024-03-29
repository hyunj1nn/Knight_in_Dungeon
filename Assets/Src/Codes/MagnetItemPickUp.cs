using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetItemPickUp : MonoBehaviour, IPickUpObject
{
    public ItemWaypoint waypointController;

    [SerializeField] private float duration = 10.0f;  // 자석 효과가 지속되는 시간

    [SerializeField] float magnetSpeed = 5.0f;  // 자석 효과의 속도
    private bool isAttracted = false;  // 플레이어에게 끌리고 있는지의 상태
    private bool bounceEffect = false;  // 튕겨져 나가는 효과의 상태

    [SerializeField] float bounceForce = 2.0f;  // 튕겨져 나가는 힘
    private float currentMagnetSpeed;  // 현재 자석 효과의 속도
    private Vector2 currentVelocity;  // SmoothDamp에 필요한 변수
    private float smoothTime = 0.1f;  // 움직임의 부드러움 정도를 조정
    
    private Transform playerTransform;
    private AudioSource audioSource;

    public void OnPickUp(Character character)
    {

    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (waypointController != null)  // waypointController가 null이 아닐 때만 호출
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
                Character c = playerTransform.GetComponent<Character>();
                if (c != null)
                {
                    IPickUpObject pickupObject = GetComponent<IPickUpObject>();
                    if (pickupObject != null)
                    {
                        pickupObject.OnPickUp(c);
                        Destroy(gameObject); // 여기에 아이템 오브젝트 제거 로직을 추가
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
                magnetEffect.ActivateMagnet(duration);  // 자석 효과를 활성화

                if (audioSource != null && audioSource.clip != null)
                {
                    audioSource.PlayOneShot(audioSource.clip);
                }
            }

            Character c = collision.GetComponent<Character>();
            if (c != null)
            {
                bounceEffect = true;  // 먼저 튕겨나가는 효과를 활성화
                playerTransform = collision.transform;
                currentMagnetSpeed = magnetSpeed;  // 자석 효과가 시작될 때의 초기 속도를 설정
            }
        }
    }

    IEnumerator BounceTimer()
    {
        yield return new WaitForSeconds(0.5f); 
        bounceEffect = false;
        isAttracted = true;
    }
}
