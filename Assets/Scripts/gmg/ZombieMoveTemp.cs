using UnityEngine;

public class ZombieMoveTemp : BaseMonster
{
    public float speed; // �̵� �ӵ�
    public float attackRange; // ���� ����
    public float attackCooldown; // ���� ��Ÿ��
    public Rigidbody2D target; // ���ΰ� Ÿ��
    public float detectionRange = 3f;  //zombie�� Ÿ�� Ž�� ����
    private SpriteRenderer spriteRenderer; // ��������Ʈ ������: ���� �̹��� ���� �� ǥ��
    private float lastAttackTime = 0f; // ������ ���� �ð��� �����
    private bool isMovingToTarget = false; // ���ΰ����� �̵� ������ ����

    public Transform attackBoxPos;
    public Vector2 boxSize;

    private bool isAttack = false;

    protected override void Awake()
    {
        base.Awake(); // BaseMonster�� Awake �޼��� ȣ��
        spriteRenderer = GetComponent<SpriteRenderer>(); // SpriteRenderer ������Ʈ ��������
        speed = Random.Range(speed - 1f, speed + 1f); // �Ǽ� �������� ������
        knockbackDistance = Random.Range(knockbackDistance - 0.1f * speed, knockbackDistance + 0.1f * speed);
        knockbackSpeed= Random.Range(knockbackSpeed - 2f, knockbackSpeed + 2f);
    }

    protected override void FixedUpdate()
    {
        //base.FixedUpdate(); // �θ� Ŭ������ FixedUpdate ȣ��
        if ( isAttack)
        {
            return;
        }


        float distanceToTarget = Vector2.Distance(transform.position, target.position); // ���ΰ����� �Ÿ� ���

        if (distanceToTarget <= attackRange && Time.time >= lastAttackTime + attackCooldown && distanceToTarget <= detectionRange)
        {
            StartAttack(); // ���� ����

            AudioManager.instance.PlaySfx(AudioManager.Sfx.Zombie2);
        }
        else if (distanceToTarget > attackRange && !isAttack && distanceToTarget <= detectionRange)
        {
            MoveToTarget(); // ���ΰ��� ���� �̵�
        }
        else
        {
            Idle(); // Idle ���·� ��ȯ
        }
    }

    private void MoveToTarget()
    {
        Vector2 dirVec = target.position - (Vector2)transform.position; // ���ΰ����� ���� ���� ���
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime; // �̵��� ��ġ ��� (�ӵ� * �ð�)

        animator.SetTrigger("Walk");

        transform.position += (Vector3)nextVec; // ������ ��ġ ������Ʈ

        //spriteRenderer.flipX = dirVec.x > 0 ? true : false ; // ���ΰ� ���⿡ ���� ��������Ʈ ����
        transform.localScale = dirVec.x > 0 ?  new Vector3 ( 1, 1, 1) :  new Vector3 (-1, 1,1); // ���ΰ� ���⿡ ���� ��������Ʈ ����

    }


    public void StartAttack()
    {
        isAttack = true;
        // ���� ó��
        lastAttackTime = Time.time; // ������ ���� �ð� ����
        animator.SetTrigger("Attack"); // ���� �ִϸ��̼� ����
    }

    public void Attack()
    {

        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(attackBoxPos.position, boxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            Debug.Log(collider.tag);

            if (collider.CompareTag("Player")) //�±װ� Monster�� ���
            {
                //collider.GetComponent<EnemyHealth>().Damage(atk, collider.transform.position - transform.position);
                collider.GetComponentInChildren<PlayerHealth>().TakeDamage(1, transform.position);
            }
        }

    }

    public void EndAttack()
    {
        isAttack = false;
        Idle();
    }

    private void Idle()
    {
        // Idle ���� ó��
        animator.SetTrigger("Idle"); // Idle �ִϸ��̼� ����
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.gameObject.GetComponentInChildren<PlayerHealth>().TakeDamage(1, transform.position);


            Debug.Log("�÷��̾�� �浹");
            // �浹�� ��ü�� ���ΰ��� ��� (�ʿ� �� �߰� ���� ���� ����)
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackBoxPos.position, boxSize);
    }

    public override void TakeDamage(int damage, Vector2 damageSourcePosition)
    {

        base.TakeDamage(damage, damageSourcePosition);

        animator.SetTrigger("Damaged");

    }

}
