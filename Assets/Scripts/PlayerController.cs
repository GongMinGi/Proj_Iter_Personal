using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.Tilemaps;

// made by mingi

public class PlayerController : MonoBehaviour
{

    // 기본변수들
    bool isGround = true;
    Rigidbody2D playerRigid;
    Transform playerTransform;
    Vector2 playerDirection;
    Animator playerAnim;
    SpriteRenderer spriteRenderer;

    //이동 관련 변수들
    [Header("이동 세팅")]
    [SerializeField]
    float moveSpeed = 1f; // 이동시에 곱해주는 스피드 
    [SerializeField]
    float jumpForce = 3f;


    //대쉬 관련 변수들
    [Header("대시 세팅")]
    [SerializeField] float dashVelocity = 20f;  // 대시 속도
    [SerializeField] float dashTime = 0.2f;     // 대쉬 지속시간
    [SerializeField] float dashCoolDown = 1f;   // 대시 쿨타임

    private Vector2 dashDirection;      // 대시 방향
    private bool isDashing;             // 대시 중인지 여부
    private bool canDash = true;        // 대시 가능 여부

    private TrailRenderer trailRenderer;


    //공격 관련 변수들
    [Header("공격 세팅")]
    [SerializeField] int atk = 1;   //공격력
    [SerializeField] float attackForce = 20f;
    [SerializeField]
    float attackCurTime = 0f;   // 지속적으로 감소
    [SerializeField]
    float attackCoolTime = 0.5f; // 공격시 attackCUrTime에 넣어준다.
    public Transform attackBoxPos;
    public Vector2 boxSize;

    //차지 공격 관련 변수와 상태
    [SerializeField] float chargeStartTime = 0.5f; //차지공격 준비에 필요한 시간
    [SerializeField] float chargeIsReady = 2f;

    private float chargeCounter = 0f; //마우스 버튼을 누른 시가 
    private bool isCharging = false; //현재 차지 공격 상태 여부
    private bool movementDisabled = false; // 차지공격 상태일 때 이동 제한

    [SerializeField] private GameObject energyBallPrefab;
    [SerializeField] private GameObject chargeBeamPrefab;
    [SerializeField] private Transform weaponTip; // 무기의 끝, 에너미볼 생성 위치
    
    private GameObject currentEnergyBall;
    private GameObject currentEnergyBeam;



    // 피격 관련 변수
    public float playerKnockbackForce;


    // 글라이딩 관련 변수들
    [Header("글라이딩 세팅")]
    [SerializeField] private float glideGravityScale = 0.5f; // 글라이딩 중 중력 값
    public bool isGliding = false;// 글라이딩 상태 플래그
    [SerializeField] private float velocityLimit;
    private bool isGlideSoundPlaying = false;

    // 점프 관련 변수들
    private bool IsJumpSoundPlaying = false;
    void Awake()
    {
        weaponTip.SetParent(transform);
        playerRigid = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        playerTransform = GetComponent<Transform>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(playerAnim.GetCurrentAnimatorStateInfo(0).IsName("PlayerCharge"));

        Jump();
        HandleAttack();

        if (Input.GetMouseButton(1) && !isGround) // 마우스 우측 버튼 눌림
        {
            StartGliding();

            if (!isGlideSoundPlaying)
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.glide);
                isGlideSoundPlaying = true; // 재생 상태 설정
            }
        }
        else if (isGliding)
        {
            StopGliding();
            isGlideSoundPlaying = false; // 소리 재생 가능 상태로 초기화
        }
        /*AudioManager.instance.PlaySfx(AudioManager.Sfx.glide);
    }
    else if (isGliding)
    {
        StopGliding();
    }
        */

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartDash();

            AudioManager.instance.PlaySfx(AudioManager.Sfx.dash1);
        }


        //sprite를 뒤집는 코드. GetAxisRaw(horizontal)은 a를 누를땐 -1, d를 누를 땐 1이므로 
        //a를 누르면 -1과 같아져 true, d를 누르면 -1과 달라져서 false를 반환하게 됨.
        // 또한 getbuttondown은 버튼이 처음으로 눌렸을대만 true를 반환한다.
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;


        //if (Mathf.Abs(playerRigid.linearVelocity.normalized.x )>= 0.1 && !isDashing)
        if (Mathf.Abs(playerRigid.linearVelocity.normalized.x) >= 0.1)
        {
            playerAnim.SetBool("isMoving", true);

        }
        else
        {

            playerAnim.SetBool("isMoving", false);
        }



        UpdateAttackBoxPosition();
    }

    void FixedUpdate()
    {
        Move();

        VelocityLimit();



        if (playerRigid.linearVelocity.y < 0 && !isGround)
        {
            playerAnim.SetBool("isFalling", true);
        }
        else
        {
            playerAnim.SetBool("isFalling", false);
        }



        //landing platform
        if (playerRigid.linearVelocity.y < 0) //추락할때만 레이를 아래로 쏜다.
        {
            Debug.DrawRay(playerRigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(playerRigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                Debug.Log(rayHit.collider.name);

                if (rayHit.distance < 1f)
                {
                    Debug.Log(rayHit.collider.name);
                    playerAnim.SetBool("isJumping", false);
                    isGround = true;
                }
            }
            else
            {
                isGround = false;
            }
        }

    }

    // y축으로 플레이어의 속도가 제한값을 벗어날 경우 최대치로 다시 초기화
    void VelocityLimit()
    {
        if (playerRigid.linearVelocity.y > 0 && Mathf.Abs(playerRigid.linearVelocity.y) > velocityLimit)
        {
            playerRigid.linearVelocity = new Vector2(playerRigid.linearVelocity.x, velocityLimit);
        }
        else if (playerRigid.linearVelocity.y < 0 && Mathf.Abs(playerRigid.linearVelocity.y) > velocityLimit)
        {
            Debug.Log("FUck");
            playerRigid.linearVelocity = new Vector2(playerRigid.linearVelocity.x, -velocityLimit);
        }
    }


    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !playerAnim.GetBool("isJumping") && !playerAnim.GetBool("isCharging"))
        {
            playerRigid.linearVelocity = new Vector2(playerRigid.linearVelocity.x, jumpForce);
            playerAnim.SetBool("isJumping", true);

            // 점프 소리 재생 조건 추가
            if (!IsJumpSoundPlaying)
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.jump1); // 점프 효과음
                IsJumpSoundPlaying = true; // 소리 재생 상태 설정
            }
        }

        // 점프 종료 시 초기화
        if (playerAnim.GetBool("isJumping") == false && IsJumpSoundPlaying)
        {
            IsJumpSoundPlaying = false; // 점프 소리 재생 가능 상태로 초기화
        }
    }

    //void Jump 소리 중복으로 인한 수정

    void UpdateAttackBoxPosition()
    {
        if (spriteRenderer.flipX)
        {
            attackBoxPos.localPosition = new Vector2(-Mathf.Abs(attackBoxPos.localPosition.x), attackBoxPos.localPosition.y);
            weaponTip.localPosition = new Vector2(-Mathf.Abs(weaponTip.localPosition.x), weaponTip.localPosition.y);

        }
        else
        {
            attackBoxPos.localPosition = new Vector2(Mathf.Abs(attackBoxPos.localPosition.x), attackBoxPos.localPosition.y);
            weaponTip.localPosition = new Vector2(Mathf.Abs(weaponTip.localPosition.x), weaponTip.localPosition.y);

        }

    }

    //차지 시작시 구체 생성 
    private void EnergyBallStart()
    {
        // 이미 구체가 있다면 중복생성 방지
        if (currentEnergyBall != null) return;

        // 구체 프리팹 생성 
        currentEnergyBall = Instantiate(energyBallPrefab, weaponTip.position, quaternion.identity);

        // 구체를 weaponTip의 자식으로 둬서 위치 고정
        currentEnergyBall.transform.SetParent(weaponTip);
    }

    //차지가 끝나면 구체 해제 
    private void EndCharge()
    {
        if (currentEnergyBall != null)
        {
            Destroy(currentEnergyBall);
        }
    }


    void HandleAttack()
    {
        // (1) 마우스 왼쪽 버튼을 누르고 있는 동안 시간 카운트 
        if (Input.GetMouseButtonDown(0))
        {
            //공격 쿨타임이 남아잇다면 리턴
            if (attackCurTime > 0f) return;

            //새로 누르기 시작했으니 타이머 초기화
            chargeCounter = 0f;
            isCharging = false;
        }

        if (Input.GetMouseButton(0))
        {
            chargeCounter += Time.deltaTime; //버튼 누른 시간 증가

            //chargeStarttime이 지나면 차지 모션 시작 ( 단 한번만)
            if (!isCharging && chargeCounter >= chargeStartTime)
            {
                //AudioManager.instance.PlaySfx(AudioManager.Sfx.ChargeattackCharging);
                isCharging = true;
                playerAnim.SetBool("isCharging", true); // 차지 애니메이션 시작
                EnergyBallStart();
                movementDisabled = true; //이동제한
            }
        }

        //(2) 마우스를 때는 순간 분기
        if (Input.GetMouseButtonUp(0)) //마우스 왼쪽 버튼을 땔 때
        {
            // (2-1) 충분히 길게 누른 상태라면 => 차지공격
            if (isCharging && chargeCounter >= chargeIsReady)
            {
                //차지 공격 실행
                PerformChargeAttack();
                AudioManager.instance.PlaySfx(AudioManager.Sfx.ChargeAttackRelease1_1);
            }
            else  // (2-1) 차지 중이 아니거나, 차지시간이 모자라다면 => 일반공격
            {
                PerformNormalAttack(); // 일반공격
                AudioManager.instance.PlaySfx(AudioManager.Sfx.attack3);
            }

            //공격 후 상태 초기화
            chargeCounter = 0; //누른 시간 초기화
            isCharging = false;//  차지상태 해제 
            movementDisabled = false; // 이동가능
            playerAnim.SetBool("isCharging", false); //차지 애니메이션 해제
            EndCharge();
        }


        //(3) 공격 쿨타임 감소
        if (attackCurTime > 0f)
        {
            attackCurTime -= Time.deltaTime;
        }
    }

    public void StartChargeLoop()
    {

        // 현재 애니메이션 상태를 반복 재생
        playerAnim.Play("PlayerCharge", 0, 0.15f);
    }


    private void PerformNormalAttack()
    {
        //일반 공격 애니메이션
        if(isDashing || playerAnim.GetBool("isFalling"))
        {
            return;
        }
        playerAnim.SetTrigger("Attack");
        //쿨타임 리셋
        attackCurTime = attackCoolTime;

        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(attackBoxPos.position, boxSize, 0f);
        foreach (Collider2D col in hitColliders)
        {
            if (col.CompareTag("Monster"))
            {
                BaseMonster monster = col.GetComponent<BaseMonster>();
                if (monster != null)
                {
                    monster.TakeDamage(1, transform.position);
                }
            }
        }
    }

    private void PerformChargeAttack()
    {
        Debug.Log("차지공격실행");


        //차지 공격 애니메이션
        playerAnim.SetTrigger("chargeAttack");

        //GetComponent<RaycastBeamShooter>().ShootBeam();
        //빔 발사 프리팹 생성
        GameObject beamObj = Instantiate(chargeBeamPrefab, weaponTip.position, quaternion.identity);


        if(spriteRenderer.flipX)
        {
            beamObj.transform.localScale = new Vector3(-1, 1, 1);
        }

        
        beamObj.transform.SetParent(weaponTip);

        // 빔 일정시간 이후 파괴
        Destroy(beamObj, 1f);

        //쿨타임 리셋
        attackCurTime = attackCoolTime;

        //// 실제 데미지 판정 (차지 데미지를 더 높게 설정하는 예시)
        //Collider2D[] hitColliders = Physics2D.OverlapBoxAll(attackBoxPos.position, boxSize, 0f);
        //foreach (Collider2D col in hitColliders)
        //{
        //    if (col.CompareTag("Monster"))
        //    {
        //        BaseMonster monster = col.GetComponent<BaseMonster>();
        //        if (monster != null)
        //        {
        //            monster.TakeDamage(2, transform.position);
        //        }
        //    }
        //}

    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackBoxPos.position, boxSize);
    }


    void Move()
    {

        if (!isDashing && !movementDisabled) //차지 혹은 대시 중에는 이동 부락
        {

            float moveInput = Input.GetAxisRaw("Horizontal");

            playerRigid.linearVelocity = new Vector2(moveInput * moveSpeed, playerRigid.linearVelocity.y);
        }


    }

    void StartDash()
    {


        //플레이어 입력에서 대시 방향 가져오기 

        //전방향 대시
        //dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        //x축만 대시 가능 
        dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), 0).normalized;


        //입력이 없을 경우 대시 불가
        if (dashDirection == Vector2.zero)
        {
            return;
            //dashDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        }

        if(playerAnim.GetBool("isGliding"))
        {
            return;
        }

        isDashing = true;   // 대시 시작 설정
        playerAnim.SetBool("isDashing", true);

        canDash = false; // 대시 가능 상태 비활성화

        if (trailRenderer != null)
        {
            trailRenderer.emitting = true;
        }

        // 대시 코루틴 실행
        StartCoroutine(PerformDash());
    }


    private IEnumerator PerformDash()
    {
        float startTime = Time.time; // 대시 시작 시간 저장


        //대시 지속 시간 동안 속도 적용
        while (Time.time < startTime + dashTime)
        {
            playerRigid.linearVelocity = dashDirection * dashVelocity;
            yield return null; // 다음 프레임까지 대기
        }

        //대시 중지 설정
        playerRigid.linearVelocity = Vector2.zero; // 속도 초기화
        isDashing = false;

        playerAnim.SetBool("isDashing", false);


        //낙하 상태 확인
        if (playerAnim.GetBool("isJumping"))
        {
            playerAnim.SetBool("isFalling", true);

        }
        else
        {
            playerAnim.SetBool("isFalling", false);
            playerAnim.SetBool("isMoving", false); //Idle로 전환
        }



        //Trail Renderer 비활성화
        if (trailRenderer != null)
        {
            trailRenderer.emitting = false;
        }


        //대시 쿨다운 적용
        yield return new WaitForSeconds(dashCoolDown); //쿨다운 시간 설정 
        canDash = true;


    }

    public void OnDamaged(Vector2 enemyPos)
    {
        playerAnim.SetTrigger("onDamaged");

        gameObject.layer = 7;

        spriteRenderer.color = new Color(1, 1, 1, 0.4f);


        float dirc = enemyPos.x - transform.position.x > 0 ? -1 : 1;
        playerRigid.AddForce(new Vector2(dirc, 0.2f) * playerKnockbackForce, ForceMode2D.Impulse);


        Invoke("OffDamaged", 1.5f);
    }


    private void OffDamaged()
    {
        //Animator anim = GetComponentInParent<Animator>();
        //playerAnim.SetBool("onDamaged", false);

        gameObject.layer = 0;
        spriteRenderer.color = new Color(1, 1, 1, 1);



    }


    public void StartGliding()
    {
        if (playerRigid.linearVelocity.y < 0) // 아래로 떨어질 때만 글라이딩 가능
        {
            isGliding = true;
            velocityLimit = 2;
            playerRigid.gravityScale = glideGravityScale;
            playerAnim.SetBool("isGliding", true);
        }
    }

    public void StopGliding()
    {
        isGliding = false;
        velocityLimit = 100;
        playerRigid.gravityScale = 1f;
        playerAnim.SetBool("isGliding", false);
    }

    public void StartGlideLoop()
    {
        // 현재 애니메이션 상태를 반복 재생
        playerAnim.Play("PlayerGliding", 0, 0.6f); // 50%에서 시작
    }
}
