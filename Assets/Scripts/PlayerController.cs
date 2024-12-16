using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.Composites;

// made by mingi

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    float moveSpeed = 1f;
    [SerializeField]
    float jumpForce = 3f;
    bool isGround = true;
    Rigidbody2D playerRigid;
    Transform playerDirection;
    Animator playerAnim;
    SpriteRenderer spriteRenderer;


    void Awake()
    {
        playerRigid = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        playerDirection = GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Jump();

        //sprite를 뒤집는 코드. GetAxisRaw(horizontal)은 a를 누를땐 -1, d를 누를 땐 1이므로 
        //a를 누르면 -1과 같아져 true, d를 누르면 -1과 달라져서 false를 반환하게 됨.
        // 또한 getbuttondown은 버튼이 처음으로 눌렸을대만 true를 반환한다.
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        
    }

    void FixedUpdate()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        playerRigid.linearVelocity = new Vector2(moveInput * moveSpeed, playerRigid.linearVelocity.y);


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
}
