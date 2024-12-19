using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.Composites;

// made by mingi

public class PlayerController : MonoBehaviour
{

    // 기본변수들
    bool isGround = true;
    Rigidbody2D playerRigid;
    Transform playerDirection;
    Animator playerAnim;
    SpriteRenderer spriteRenderer;

    //이동 관련 변수들
    [SerializeField]
    float walkSpeed = 1f;
    float moveSpeed = 1f; // 이동시에 곱해주는 스피드 
    [SerializeField]
    float jumpForce = 3f;
    [SerializeField]
    float dashSpeed = 1f;
    float dashTimeRemain = 0f; // 현재 대쉬 중이면 양수
    float dashTime = 0.5f; // 해당값으로 초기화
    float dashCoolRemain = 0f;
    float dashCool = 1f;
    float dashDirection = 0f;
    bool isDashing = false;

    // 공격 관련 변수들
    [SerializeField]
    float attackCurTime = 0f;
    [SerializeField]
    float attackCoolTime = 0.5f;
    public Transform attackBoxPos;
    public Vector2 boxSize;

    void Awake()
    {
        playerRigid = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        playerDirection = GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        moveSpeed = walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        Jump();
        Attack();


        dashTimeRemain -= Time.deltaTime; // 계속해서 대쉬 지속지간과 쿨타임을 감소시켜준다.
        dashCoolRemain -= Time.deltaTime;

        //대쉬 중도 아니고 쿨타임도 안남았고 대쉬타임도 아닌 상태에서 쉬프트를 누르면 대쉬 진행
        if (!isDashing && dashCoolRemain < 0f && dashTimeRemain < 0f && Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dash();
        }


        //sprite를 뒤집는 코드. GetAxisRaw(horizontal)은 a를 누를땐 -1, d를 누를 땐 1이므로 
        //a를 누르면 -1과 같아져 true, d를 누르면 -1과 달라져서 false를 반환하게 됨.
        // 또한 getbuttondown은 버튼이 처음으로 눌렸을대만 true를 반환한다.
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        
    }

    void FixedUpdate()
    {
        Move();
        


        //landing platform
        if(playerRigid.linearVelocity.y < 0 ) //추락할때만 레이를 아래로 쏜다.
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
                }
            }
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


    void Attack()
    {
        if(attackCurTime <= 0f)
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {

                Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(attackBoxPos.position, boxSize, 0);
                foreach (Collider2D collider in collider2Ds)
                {
                    Debug.Log(collider.tag);
                }
                playerAnim.SetTrigger("Attack");
                attackCurTime = attackCoolTime;
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

    void Dash()
    {
        isDashing = true;
        dashTimeRemain = dashTime;
        dashCoolRemain = dashCool;

        dashDirection = spriteRenderer.flipX ? 1 : -1;

        playerRigid.linearVelocity = new Vector2( dashDirection * dashSpeed, playerRigid.linearVelocity.y);
        isDashing = false;
    }
    
}
