using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class PlayerControllerbyBae : MonoBehaviour
{
    // 기본 변수들
    bool isGround = true; // 땅에 있는지 여부
    Rigidbody2D playerRigid; // 플레이어 Rigidbody2D
    Transform playerTransform; // 플레이어 Transform
    Animator playerAnim; // 플레이어 Animator
    SpriteRenderer spriteRenderer; // SpriteRenderer

    // 이동 관련 변수들
    [Header("이동 세팅")]
    [SerializeField] float moveSpeed = 1f; // 이동 속도
    [SerializeField] float jumpForce = 3f; // 점프 힘

    // 글라이딩 관련 변수들
    [Header("글라이딩 세팅")]
    [SerializeField] private float glideGravityScale = 0.5f; // 글라이딩 중 중력 값
    public bool IsGliding { get; private set; } // 글라이딩 상태 플래그

    // 대쉬 관련 변수들
    [Header("대시 세팅")]
    [SerializeField] float dashVelocity = 20f;  // 대시 속도
    [SerializeField] float dashTime = 0.2f;     // 대쉬 지속시간
    [SerializeField] float dashCoolDown = 1f;   // 대시 쿨타임

    private Vector2 dashDirection;      // 대시 방향
    private bool isDashing;             // 대시 중인지 여부
    private bool canDash = true;        // 대시 가능 여부

    private TrailRenderer trailRenderer;

    // 공격 관련 변수들
    [Header("공격 세팅")]
    [SerializeField] int atk = 1;   // 공격력
    [SerializeField] float attackCoolTime = 0.5f; // 일반 공격 쿨타임
    [SerializeField] float chargeTime = 0.5f; // 차지 공격 준비 시간
    private float attackCurTime = 0f;
    private float chargeCounter = 0f; // 버튼 누름 시간
    private bool isCharging = false;
    private bool movementDisabled = false; // 이동 제한 플래그

    public Transform attackBoxPos; // 공격 박스 위치
    public Vector2 boxSize; // 공격 박스 크기

    void Awake()
    {
        playerRigid = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        playerTransform = GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    [System.Obsolete]
    void Update()
    {
        if (!movementDisabled) // 차지 상태가 아닐 때만 입력 처리
        {
            Jump();
            AudioManager.instance.PlaySfx(AudioManager.Sfx.jump1);
        }

        HandleAttack();

        if (Input.GetMouseButton(1)) // 마우스 우측 버튼 눌림
        {
            StartGliding();
        }
        else if (IsGliding)
        {
            StopGliding();
        }

        if (!movementDisabled && Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartDash();
            AudioManager.instance.PlaySfx(AudioManager.Sfx.dash1);
        }

        if (!movementDisabled && Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        if (Mathf.Abs(playerRigid.velocity.normalized.x) >= 0.1)
        {
            playerAnim.SetBool("isMoving", true);
        }
        else
        {
            playerAnim.SetBool("isMoving", false);
        }

        UpdateAttackBoxPosition();
    }

    [System.Obsolete]
    void FixedUpdate()
    {
        if (!movementDisabled) // 차지 상태가 아닐 때만 이동 처리
        {
            Move();
        }

        if (playerRigid.velocity.y < 0 && !isGround)
        {
            playerAnim.SetBool("isFalling", true);
        }
        else
        {
            playerAnim.SetBool("isFalling", false);
        }

        if (playerRigid.velocity.y < 0)
        {
            Debug.DrawRay(playerRigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(playerRigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 1f)
                {
                    playerAnim.SetBool("isJumping", false);
                    isGround = true;
                }
            }
            isGround = false;
        }
    }

    [System.Obsolete]
    void HandleAttack()
    {
        if (Input.GetMouseButton(0)) // 마우스 왼쪽 버튼 누름
        {
            chargeCounter += Time.deltaTime; // 버튼 누른 시간 증가
            playerAnim.SetFloat("chargeCounter", chargeCounter); // Animator에 값 전달

            if (chargeCounter >= chargeTime && !isCharging) // 차지 상태 조건
            {
                isCharging = true;
                playerAnim.SetBool("IsCharging", true); // 차지 애니메이션 시작
                movementDisabled = true; // 이동 제한
                StartCoroutine(HandleChargeState()); // 차지 상태 유지
            }
        }

        if (Input.GetMouseButtonUp(0)) // 마우스 왼쪽 버튼 뗌
        {
            if (!isCharging && chargeCounter < chargeTime) // 일반 공격 조건
            {
                PerformNormalAttack(); // 일반 공격 실행
                AudioManager.instance.PlaySfx(AudioManager.Sfx.attack3);
            }

            // 상태 초기화
            chargeCounter = 0f;
            playerAnim.SetFloat("chargeCounter", chargeCounter); // Animator 값 초기화
            isCharging = false;
            movementDisabled = false;
            playerAnim.SetBool("IsCharging", false); // 차지 상태 해제
        }
    }

    private IEnumerator HandleChargeState()
    {
        yield return new WaitForSeconds(2.5f); // 차지 유지 시간

        if (isCharging)
        {
            PerformChargeAttack(); // 차지 공격 실행
        }

        // 차지 상태 종료
        isCharging = false;
        playerAnim.SetBool("IsCharging", false);
        movementDisabled = false; // 이동 가능
    }

    void UpdateAttackBoxPosition()
    {
        if (spriteRenderer.flipX)
        {
            attackBoxPos.localPosition = new Vector2(-Mathf.Abs(attackBoxPos.localPosition.x), attackBoxPos.localPosition.y);
        }
        else
        {
            attackBoxPos.localPosition = new Vector2(Mathf.Abs(attackBoxPos.localPosition.x), attackBoxPos.localPosition.y);
        }
    }

    private void PerformNormalAttack()
    {
        Debug.Log("일반 공격 실행!");
        playerAnim.SetTrigger("Attack"); // 일반 공격 애니메이션
        attackCurTime = attackCoolTime; // 쿨타임 초기화

        // 공격 판정
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(attackBoxPos.position, boxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.CompareTag("Monster"))
            {
                collider.GetComponent<BaseMonster>().TakeDamage(atk, attackBoxPos.position);
            }
        }
    }

    private void PerformChargeAttack()
    {
        Debug.Log("차지 공격 실행!");
        playerAnim.SetTrigger("ChargeAttack"); // 차지 공격 애니메이션

        // 차지 공격 효과 추가
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(attackBoxPos.position, boxSize * 1.5f, 0); // 더 넓은 범위
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.CompareTag("Monster"))
            {
                collider.GetComponent<BaseMonster>().TakeDamage(atk * 2, attackBoxPos.position); // 차지 공격은 2배 데미지
            }
        }
    }

    [System.Obsolete]
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !playerAnim.GetBool("isJumping"))
        {
            playerRigid.velocity = new Vector2(playerRigid.velocity.x, jumpForce);
            playerAnim.SetBool("isJumping", true);
        }
    }

    [System.Obsolete]
    void StartGliding()
    {
        if (playerRigid.velocity.y < 0) // 아래로 떨어질 때만 글라이딩 가능
        {
            IsGliding = true;
            playerRigid.gravityScale = glideGravityScale;
            playerAnim.SetBool("IsGliding", true);
        }
    }

    void StopGliding()
    {
        IsGliding = false;
        playerRigid.gravityScale = 1f;
        playerAnim.SetBool("IsGliding", false);
    }

    [System.Obsolete]
    void Move()
    {
        if (!isDashing)
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            playerRigid.velocity = new Vector2(moveInput * moveSpeed, playerRigid.velocity.y);
        }
    }

    [System.Obsolete]
    void StartDash()
    {
        dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), 0).normalized;

        if (dashDirection == Vector2.zero)
        {
            return;
        }

        isDashing = true;
        playerAnim.SetBool("isDashing", true);
        canDash = false;

        if (trailRenderer != null)
        {
            trailRenderer.emitting = true;
        }

        StartCoroutine(PerformDash());
    }

    [System.Obsolete]
    private IEnumerator PerformDash()
    {
        float startTime = Time.time;

        while (Time.time < startTime + dashTime)
        {
            playerRigid.velocity = dashDirection * dashVelocity;
            yield return null;
        }

        playerRigid.velocity = Vector2.zero;
        isDashing = false;

        playerAnim.SetBool("isDashing", false);

        if (playerAnim.GetBool("isJumping"))
        {
            playerAnim.SetBool("isFalling", true);
        }
        else
        {
            playerAnim.SetBool("isFalling", false);
            playerAnim.SetBool("isMoving", false);
        }

        if (trailRenderer != null)
        {
            trailRenderer.emitting = false;
        }

        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;
    }
}













