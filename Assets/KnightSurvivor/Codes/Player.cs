using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Scanner scanner;
    public Vector2 inputVec; // �Է� ���� ����

    public float speed; // �ӵ� ���� ���� 
    public float dashspeed; //  �뽬 �ӵ� 
    public float dashduration; // �뽬 ���� �ð�
    public float dashCooldown; // ��Ÿ�� �߰�

    private bool isDashing; // �뽬 ������ ����
    private bool canDash; // ��Ÿ���� �������� Ȯ���ϴ� ���� �߰�
    private float dashTimer; // �뽬 Ÿ�̸� 
    private float cooldownTimer; // ��Ÿ�� Ÿ�̸� �߰�

    public StatusBar dashCooldownBar; // ��Ÿ�� �� �߰�

    public float lastHorizontalVector; // ���� �̵�
    public float lastVerticalVector; // ���� �̵�

    public bool canMove = true;

    Rigidbody2D rigid; // ���� �Է� ���� ���� 
    public SpriteRenderer spriter;
    Animator anim; // �ִϸ��̼� �Է� ����

    void Awake()
    {
        // ���� ������ �ʱ�ȭ
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
    }

    private void Start()
    {
        lastHorizontalVector = 1f;
        lastVerticalVector = 1f;
        canDash = true; // �ʱ⿡�� �뽬 ����
    }

    void Update()
    {
        if (!canMove || !GameManager.instance.isLive)
            return;

        if (!GameManager.instance.isLive)
            return;
        inputVec.x = Input.GetAxisRaw("Horizontal"); // x�� ���� �̵�
        inputVec.y = Input.GetAxisRaw("Vertical"); // y�� ���� �̵�

        // �����̽��� �Է� ���� �� �뽬 ���� (��Ÿ���� ���� ��쿡��)
        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            isDashing = true;
            dashTimer = dashduration;
            canDash = false; // �뽬�� ����ϸ� ��Ÿ�� ���� �뽬 �Ұ���
            cooldownTimer = 0f; // ��Ÿ�� Ÿ�̸Ӹ� 0���� �ʱ�ȭ

            // ��Ÿ�� �� ����
            dashCooldownBar.SetState(cooldownTimer, dashCooldown);
        }
        // ��Ÿ�� ���� �� �뽬 ���� ���� ����
        if (!canDash)
        {
            cooldownTimer += Time.deltaTime; // ��Ÿ���� �þ���� ����

            // ��Ÿ�� �� ����
            dashCooldownBar.SetState(cooldownTimer, dashCooldown);

            if (cooldownTimer >= dashCooldown) // ��Ÿ���� �� á�� �� �뽬 �����ϵ��� ����
            {
                canDash = true;
                cooldownTimer = dashCooldown; // ��Ÿ�� Ÿ�̸Ӹ� �ִ� ��Ÿ������ ����
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

        // �뽬 ���� ���� �ƴ� �� ó�� ����� �ٸ�
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

        anim.SetFloat("Speed", inputVec.magnitude); // magnitude = ���Ϳ� ������ ũ���� ���� �Է�


        if (inputVec.x != 0) // inputVec.x �� 0�� �ƴҶ� ���� = ���� ��ȯ
        {
            spriter.flipX = inputVec.x < 0;
        }
    }
}

