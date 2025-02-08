using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.Tilemaps;

// made by mingi

public class PlayerController : MonoBehaviour
{

    // �⺻������
    bool isGround = true;
    Rigidbody2D playerRigid;
    Transform playerTransform;
    Vector2 playerDirection;
    Animator playerAnim;
    SpriteRenderer spriteRenderer;

    //�̵� ���� ������
    [Header("�̵� ����")]
    [SerializeField]
    float moveSpeed = 1f; // �̵��ÿ� �����ִ� ���ǵ� 
    [SerializeField]
    float jumpForce = 3f;


    //�뽬 ���� ������
    [Header("��� ����")]
    [SerializeField] float dashVelocity = 20f;  // ��� �ӵ�
    [SerializeField] float dashTime = 0.2f;     // �뽬 ���ӽð�
    [SerializeField] float dashCoolDown = 1f;   // ��� ��Ÿ��

    private Vector2 dashDirection;      // ��� ����
    private bool isDashing;             // ��� ������ ����
    private bool canDash = true;        // ��� ���� ����

    private TrailRenderer trailRenderer;


    //���� ���� ������
    [Header("���� ����")]
    [SerializeField] int atk = 1;   //���ݷ�
    [SerializeField] float attackForce = 20f;
    [SerializeField]
    float attackCurTime = 0f;   // ���������� ����
    [SerializeField]
    float attackCoolTime = 0.5f; // ���ݽ� attackCUrTime�� �־��ش�.
    public Transform attackBoxPos;
    public Vector2 boxSize;

    //���� ���� ���� ������ ����
    [SerializeField] float chargeStartTime = 0.5f; //�������� �غ� �ʿ��� �ð�
    [SerializeField] float chargeIsReady = 2f;

    private float chargeCounter = 0f; //���콺 ��ư�� ���� �ð� 
    private bool isCharging = false; //���� ���� ���� ���� ����
    private bool movementDisabled = false; // �������� ������ �� �̵� ����

    [SerializeField] private GameObject energyBallPrefab;
    [SerializeField] private GameObject chargeBeamPrefab;
    [SerializeField] private Transform weaponTip; // ������ ��, �������� ���� ��ġ
    
    
    private GameObject currentEnergyBall;
    private GameObject currentEnergyBeam;



    // �ǰ� ���� ����
    public float playerKnockbackForce;


    // �۶��̵� ���� ������
    [Header("�۶��̵� ����")]
    [SerializeField] private float glideGravityScale = 0.5f; // �۶��̵� �� �߷� ��
    public bool isGliding = false;// �۶��̵� ���� �÷���
    [SerializeField] private float velocityLimit;
    private bool isGlideSoundPlaying = false;

    // ���� ���� ������
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

        if (Input.GetMouseButton(1) && !isGround) // ���콺 ���� ��ư ����
        {
            StartGliding();

            if (!isGlideSoundPlaying)
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.glide);
                isGlideSoundPlaying = true; // ��� ���� ����
            }
        }
        else if (isGliding)
        {
            StopGliding();
            isGlideSoundPlaying = false; // �Ҹ� ��� ���� ���·� �ʱ�ȭ
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


        //sprite�� ������ �ڵ�. GetAxisRaw(horizontal)�� a�� ������ -1, d�� ���� �� 1�̹Ƿ� 
        //a�� ������ -1�� ������ true, d�� ������ -1�� �޶����� false�� ��ȯ�ϰ� ��.
        // ���� getbuttondown�� ��ư�� ó������ �������븸 true�� ��ȯ�Ѵ�.
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
        if (playerRigid.linearVelocity.y < 0) //�߶��Ҷ��� ���̸� �Ʒ��� ���.
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

    // y������ �÷��̾��� �ӵ��� ���Ѱ��� ��� ��� �ִ�ġ�� �ٽ� �ʱ�ȭ
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

            // ���� �Ҹ� ��� ���� �߰�
            if (!IsJumpSoundPlaying)
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.jump1); // ���� ȿ����
                IsJumpSoundPlaying = true; // �Ҹ� ��� ���� ����
            }
        }

        // ���� ���� �� �ʱ�ȭ
        if (playerAnim.GetBool("isJumping") == false && IsJumpSoundPlaying)
        {
            IsJumpSoundPlaying = false; // ���� �Ҹ� ��� ���� ���·� �ʱ�ȭ
        }
    }

    //void Jump �Ҹ� �ߺ����� ���� ����

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

    //���� ���۽� ��ü ���� 
    private void EnergyBallStart()
    {
        // �̹� ��ü�� �ִٸ� �ߺ����� ����
        if (currentEnergyBall != null) return;

        // ��ü ������ ���� 
        currentEnergyBall = Instantiate(energyBallPrefab, weaponTip.position, quaternion.identity);

        // ��ü�� weaponTip�� �ڽ����� �ּ� ��ġ ����
        currentEnergyBall.transform.SetParent(weaponTip);
    }

    //������ ������ ��ü ���� 
    private void EndCharge()
    {
        if (currentEnergyBall != null)
        {
            Destroy(currentEnergyBall);
        }
    }


    void HandleAttack()
    {
        // (1) ���콺 ���� ��ư�� ������ �ִ� ���� �ð� ī��Ʈ 
        if (Input.GetMouseButtonDown(0))
        {
            //���� ��Ÿ���� �����մٸ� ����
            if (attackCurTime > 0f) return;

            //���� ������ ���������� Ÿ�̸� �ʱ�ȭ
            chargeCounter = 0f;
            isCharging = false;
        }

        if (Input.GetMouseButton(0))
        {
            chargeCounter += Time.deltaTime; //��ư ���� �ð� ����

            //chargeStarttime�� ������ ���� ��� ���� ( �� �ѹ���)
            if (!isCharging && chargeCounter >= chargeStartTime)
            {
                //AudioManager.instance.PlaySfx(AudioManager.Sfx.ChargeattackCharging);
                isCharging = true;
                playerAnim.SetBool("isCharging", true); // ���� �ִϸ��̼� ����
                EnergyBallStart();
                movementDisabled = true; //�̵�����
            }
        }

        //(2) ���콺�� ���� ���� �б�
        if (Input.GetMouseButtonUp(0)) //���콺 ���� ��ư�� �� ��
        {
            // (2-1) ����� ��� ���� ���¶�� => ��������
            if (isCharging && chargeCounter >= chargeIsReady)
            {
                //���� ���� ����
                PerformChargeAttack();
                AudioManager.instance.PlaySfx(AudioManager.Sfx.ChargeAttackRelease1_1);
            }
            else  // (2-1) ���� ���� �ƴϰų�, �����ð��� ���ڶ�ٸ� => �Ϲݰ���
            {
                PerformNormalAttack(); // �Ϲݰ���
                AudioManager.instance.PlaySfx(AudioManager.Sfx.attack3);
            }

            //���� �� ���� �ʱ�ȭ
            chargeCounter = 0; //���� �ð� �ʱ�ȭ
            isCharging = false;//  �������� ���� 
            movementDisabled = false; // �̵�����
            playerAnim.SetBool("isCharging", false); //���� �ִϸ��̼� ����
            EndCharge();
        }


        //(3) ���� ��Ÿ�� ����
        if (attackCurTime > 0f)
        {
            attackCurTime -= Time.deltaTime;
        }
    }

    public void StartChargeLoop()
    {

        // ���� �ִϸ��̼� ���¸� �ݺ� ���
        playerAnim.Play("PlayerCharge", 0, 0.15f);
    }


    private void PerformNormalAttack()
    {
        //�Ϲ� ���� �ִϸ��̼�
        if(isDashing || playerAnim.GetBool("isFalling"))
        {
            return;
        }
        playerAnim.SetTrigger("Attack");
        //��Ÿ�� ����
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

            else if ( col.CompareTag("Boss"))
            {
                col.GetComponent<Boss>().TakeDamage(1);
            }
        }
    }

    private void PerformChargeAttack()
    {
        Debug.Log("�������ݽ���");


        //���� ���� �ִϸ��̼�
        playerAnim.SetTrigger("chargeAttack");

        //GetComponent<RaycastBeamShooter>().ShootBeam();


        //�� �߻� ������ ����
        GameObject beamObj = Instantiate(chargeBeamPrefab, weaponTip.position, quaternion.identity);


        if (spriteRenderer.flipX)
        {
            beamObj.transform.localScale = new Vector3(-1, 1, 1);
        }

        //���� transform �� �θ� weapontip���� �����ϸ� �÷��̾��� ������ �ٲ� �� ���� ����ٰ� �ȴ�. ��������
        //beamObj.transform.SetParent(weaponTip);

        // �� �����ð� ���� �ı�
        Destroy(beamObj, 1f);

        //��Ÿ�� ����
        attackCurTime = attackCoolTime;

        //������ ������ beam �����տ� ���� ��ũ��Ʈ�� ó��
        //// ���� ������ ���� (���� �������� �� ���� �����ϴ� ����)
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

        if (!isDashing && !movementDisabled) //���� Ȥ�� ��� �߿��� �̵� �Ұ�
        {

            float moveInput = Input.GetAxisRaw("Horizontal");

            playerRigid.linearVelocity = new Vector2(moveInput * moveSpeed, playerRigid.linearVelocity.y);
        }


    }

    void StartDash()
    {


        //�÷��̾� �Է¿��� ��� ���� �������� 

        //������ ���
        //dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        //x�ุ ��� ���� 
        dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), 0).normalized;


        //�Է��� ���� ��� ��� �Ұ�
        if (dashDirection == Vector2.zero)
        {
            return;
            //dashDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        }

        if(playerAnim.GetBool("isGliding"))
        {
            return;
        }

        isDashing = true;   // ��� ���� ����
        playerAnim.SetBool("isDashing", true);

        canDash = false; // ��� ���� ���� ��Ȱ��ȭ

        if (trailRenderer != null)
        {
            trailRenderer.emitting = true;
        }

        // ��� �ڷ�ƾ ����
        StartCoroutine(PerformDash());
    }


    private IEnumerator PerformDash()
    {
        float startTime = Time.time; // ��� ���� �ð� ����


        //��� ���� �ð� ���� �ӵ� ����
        while (Time.time < startTime + dashTime)
        {
            playerRigid.linearVelocity = dashDirection * dashVelocity;
            yield return null; // ���� �����ӱ��� ���
        }

        //��� ���� ����
        playerRigid.linearVelocity = Vector2.zero; // �ӵ� �ʱ�ȭ
        isDashing = false;

        playerAnim.SetBool("isDashing", false);


        //���� ���� Ȯ��
        if (playerAnim.GetBool("isJumping"))
        {
            playerAnim.SetBool("isFalling", true);

        }
        else
        {
            playerAnim.SetBool("isFalling", false);
            playerAnim.SetBool("isMoving", false); //Idle�� ��ȯ
        }



        //Trail Renderer ��Ȱ��ȭ
        if (trailRenderer != null)
        {
            trailRenderer.emitting = false;
        }


        //��� ��ٿ� ����
        yield return new WaitForSeconds(dashCoolDown); //��ٿ� �ð� ���� 
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
        if (playerRigid.linearVelocity.y < 0) // �Ʒ��� ������ ���� �۶��̵� ����
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
        // ���� �ִϸ��̼� ���¸� �ݺ� ���
        playerAnim.Play("PlayerGliding", 0, 0.6f); // 50%���� ����
    }
}
