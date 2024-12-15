using Unity.Mathematics;
using UnityEngine;

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
    Animator playerAnimator;

    void Start()
    {
        playerRigid = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        playerDirection = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Jump();
        
    }

    void FixedUpdate()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        playerRigid.linearVelocity = new Vector2(moveInput * moveSpeed, playerRigid.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerRigid.linearVelocity = new Vector2(playerRigid.linearVelocity.y,jumpForce);
        }
    }

    void Jump()
    {

    }
}
