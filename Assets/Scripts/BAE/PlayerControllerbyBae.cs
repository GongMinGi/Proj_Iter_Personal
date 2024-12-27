
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
        }

        HandleAttack();

        if (!movementDisabled && Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartDash();
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
            chargeCounter += Time.deltaTime;

            if (chargeCounter >= chargeTime && !isCharging)
            {
                isCharging = true;
                playerAnim.SetBool("isCharging", true); // 차지 애니메이션 시작
                playerRigid.velocity = Vector2.zero; // 속도 정지
                movementDisabled = true; // 이동 중지
                StartCoroutine(HandleChargeState());
            }
        }

        if (Input.GetMouseButtonUp(0)) // 마우스 왼쪽 버튼 뗌
        {
            if (!isCharging && chargeCounter < chargeTime && attackCurTime <= 0f)
            {
                PerformNormalAttack(); // 일반 공격 실행
            }

            chargeCounter = 0f;
            isCharging = false;
            playerAnim.SetBool("isCharging", false); // 차지 애니메이션 종료
            movementDisabled = false; // 이동 복구
        }

        attackCurTime -= Time.deltaTime; // 일반 공격 쿨타임 감소
    }

    private IEnumerator HandleChargeState()
    {
        yield return new WaitForSeconds(2.5f); // 2.5초 대기

        if (isCharging)
        {
            PerformChargeAttack(); // 차지 공격 실행
        }

        movementDisabled = false; // 이동 복구
        isCharging = false;
        playerAnim.SetBool("isCharging", false); // 차지 애니메이션 종료
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

        if (Input.GetKey(KeyCode.Space) && playerRigid.velocity.y < 0)
        {
            IsGliding = true;
            playerRigid.gravityScale = glideGravityScale;
            playerAnim.SetBool("IsGliding", true);
        }
        else
        {
            IsGliding = false;
            playerRigid.gravityScale = 1f;
            playerAnim.SetBool("IsGliding", false);
        }
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

/*using System.Collections;
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
    [SerializeField] float chargeTime = 2f; // 차지 공격에 필요한 최소 시간
    private float attackCurTime = 0f;
    private float chargeCounter = 0f; // 버튼 누름 시간
    private bool isCharging = false;

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
        Jump();
        HandleAttack();

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartDash();
        }

        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

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
        Move();

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

    void HandleAttack()
    {
        if (Input.GetMouseButton(0)) // 마우스 왼쪽 버튼 누름
        {
            chargeCounter += Time.deltaTime;

            if (chargeCounter >= chargeTime && !isCharging)
            {
                isCharging = true;
                playerAnim.SetBool("isCharging", true); // 차지 애니메이션 시작
            }
        }

        if (Input.GetMouseButtonUp(0)) // 마우스 왼쪽 버튼 뗌
        {
            if (isCharging)
            {
                PerformChargeAttack(); // 차지 공격 실행
            }
            else if (chargeCounter < chargeTime && attackCurTime <= 0f)
            {
                PerformNormalAttack(); // 일반 공격 실행
            }

            chargeCounter = 0f;
            isCharging = false;
            playerAnim.SetBool("isCharging", false); // 차지 애니메이션 종료
        }

        attackCurTime -= Time.deltaTime;
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

        // 글라이딩 추가
        if (Input.GetKey(KeyCode.Space) && playerRigid.velocity.y < 0) // 추락 중 글라이딩 활성화
        {
            IsGliding = true;
            playerRigid.gravityScale = glideGravityScale; // 글라이딩 중 중력 감소
            playerAnim.SetBool("IsGliding", true);
        }
        else
        {
            IsGliding = false;
            playerRigid.gravityScale = 1f; // 중력 원래 값 복구
            playerAnim.SetBool("IsGliding", false);
        }
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


*/