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
    float attackCurTime = 0f;
    [SerializeField]
    float attackCoolTime = 0.5f;
    public Transform attackBoxPos;
    public Vector2 boxSize;

    // 피격 관련 변수
    public float playerKnockbackForce;


    // 글라이딩 관련 변수들
    [Header("글라이딩 세팅")]
    [SerializeField] private float glideGravityScale = 0.5f; // 글라이딩 중 중력 값
    public bool isGliding = false;// 글라이딩 상태 플래그
    [SerializeField] private float velocityLimit;

    void Awake()
    {
        playerRigid = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        playerTransform = GetComponent<Transform>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Jump();
        Attack();

        if (Input.GetMouseButton(1) && !isGround) // 마우스 우측 버튼 눌림
        {
            StartGliding();
        }
        else if (isGliding)
        {
            StopGliding();
        }


        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartDash();
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
        if (playerRigid.linearVelocity.y < 0 ) //추락할때만 레이를 아래로 쏜다.
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
        if(playerRigid.linearVelocity.y > 0 && Mathf.Abs(playerRigid.linearVelocity.y) > velocityLimit )
        {
            playerRigid.linearVelocity = new Vector2(playerRigid.linearVelocity.x, velocityLimit);
        }
        else if (playerRigid.linearVelocity.y < 0 &&  Mathf.Abs(playerRigid.linearVelocity.y) > velocityLimit)
        {
            Debug.Log("FUck");
            playerRigid.linearVelocity = new Vector2(playerRigid.linearVelocity.x, -velocityLimit);
        }
    }


    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !playerAnim.GetBool("isJumping"))
        {
            playerRigid.linearVelocity = new Vector2(playerRigid.linearVelocity.y, jumpForce);
            playerAnim.SetBool("isJumping", true);
        }
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

    void Attack()
    {

        if (attackCurTime <= 0f)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                playerAnim.SetTrigger("Attack");
                attackCurTime = attackCoolTime;
                Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(attackBoxPos.position , boxSize, 0);
                foreach (Collider2D collider in collider2Ds)
                {
                    Debug.Log(collider.tag);

                    if(collider.CompareTag("Monster")) //태그가 Monster인 경우
                    {
                        //collider.GetComponent<EnemyHealth>().Damage(atk, collider.transform.position - transform.position);
                        collider.GetComponent<BaseMonster>().TakeDamage(1, transform.position);
                    }
                }
                
                
            }
        }
        else
        {
            attackCurTime -= Time.deltaTime;
        }
    }







    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackBoxPos.position, boxSize);
    }


    void Move()
    {

        if (!isDashing)
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
        if ( dashDirection == Vector2.zero)
        {
            return;
            //dashDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        }

        isDashing = true;   // 대시 시작 설정
        playerAnim.SetBool("isDashing", true);

        canDash = false; // 대시 가능 상태 비활성화

        if ( trailRenderer != null)
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
        while ( Time.time < startTime + dashTime)
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
            playerAnim.SetBool("isMovinng", false); //Idle로 전환
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


    void StartGliding()
    {
        if (playerRigid.linearVelocity.y < 0) // 아래로 떨어질 때만 글라이딩 가능
        {
            isGliding = true;
            velocityLimit = 2;
            playerRigid.gravityScale = glideGravityScale;
            playerAnim.SetBool("isGliding", true);
        }
    }

    void StopGliding()
    {
        isGliding = false;
        velocityLimit = 100;
        playerRigid.gravityScale = 1f;
        playerAnim.SetBool("isGliding", false);
    }

    public void StartGlideLoop()
    {
        // 현재 애니메이션 상태를 반복 재생
        playerAnim.Play("PlayerGliding", 0, 0.5f); // 50%에서 시작
    }

}
