using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Scanner scanner;
    public Vector2 inputVec; // 입력 변수 생성

    public float speed; // 속도 관리 변수 
    public float dashspeed; //  대쉬 속도 
    public float dashduration; // 대쉬 지속 시간
    public float dashCooldown; // 쿨타임 추가

    private bool isDashing; // 대쉬 중인지 여부
    private bool canDash; // 쿨타임이 끝났는지 확인하는 변수 추가
    private float dashTimer; // 대쉬 타이머 
    private float cooldownTimer; // 쿨타임 타이머 추가

    public StatusBar dashCooldownBar; // 쿨타임 바 추가

    public float lastHorizontalVector; // 수평 이동
    public float lastVerticalVector; // 수직 이동

    public bool canMove = true;

    Rigidbody2D rigid; // 물리 입력 저장 변수 
    public SpriteRenderer spriter;
    Animator anim; // 애니메이션 입력 변수

    void Awake()
    {
        // 변수 생성후 초기화
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
    }

    private void Start()
    {
        lastHorizontalVector = 1f;
        lastVerticalVector = 1f;
        canDash = true; // 초기에는 대쉬 가능
    }

    void Update()
    {
        if (!canMove || !GameManager.instance.isLive)
            return;

        if (!GameManager.instance.isLive)
            return;
        inputVec.x = Input.GetAxisRaw("Horizontal"); // x축 평행 이동
        inputVec.y = Input.GetAxisRaw("Vertical"); // y축 수직 이동

        // 스페이스바 입력 감지 및 대쉬 시작 (쿨타임이 끝난 경우에만)
        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            isDashing = true;
            dashTimer = dashduration;
            canDash = false; // 대쉬를 사용하면 쿨타임 동안 대쉬 불가능
            cooldownTimer = 0f; // 쿨타임 타이머를 0으로 초기화

            // 쿨타임 바 갱신
            dashCooldownBar.SetState(cooldownTimer, dashCooldown);
        }
        // 쿨타임 감소 및 대쉬 가능 상태 갱신
        if (!canDash)
        {
            cooldownTimer += Time.deltaTime; // 쿨타임이 늘어나도록 수정

            // 쿨타임 바 갱신
            dashCooldownBar.SetState(cooldownTimer, dashCooldown);

            if (cooldownTimer >= dashCooldown) // 쿨타임이 다 찼을 때 대쉬 가능하도록 수정
            {
                canDash = true;
                cooldownTimer = dashCooldown; // 쿨타임 타이머를 최대 쿨타임으로 설정
            }
        }

        if (isDashing)
        {
            lastHorizontalVector = inputVec.x;
            lastVerticalVector = inputVec.y;
        }
        else
        {
            if (inputVec.x != 0)
            {
                lastHorizontalVector = inputVec.x;
            }
            else if (inputVec.y != 0)
            {
                lastVerticalVector = inputVec.y;
            }
        }
    }

    void FixedUpdate()
    {
        if (!canMove || !GameManager.instance.isLive)
            return;

        if (!GameManager.instance.isLive)
            return;

        Vector2 nextVec;

        // 대쉬 중일 때와 아닐 때 처리 방식이 다름
        if (isDashing)
        {
            nextVec = new Vector2(lastHorizontalVector, lastVerticalVector).normalized * dashspeed * Time.fixedDeltaTime;
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0)
            {
                isDashing = false;
            }
        }
        else
        {
            nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        }

        rigid.MovePosition(rigid.position + nextVec);
    }

    void LateUpdate()
    {
        if (!canMove || !GameManager.instance.isLive)
            return;

        if (!GameManager.instance.isLive)
            return;

        anim.SetFloat("Speed", inputVec.magnitude); // magnitude = 벡터에 순수히 크기의 값을 입력


        if (inputVec.x != 0) // inputVec.x 가 0이 아닐때 실행 = 방향 전환
        {
            spriter.flipX = inputVec.x < 0;
        }
    }
}

