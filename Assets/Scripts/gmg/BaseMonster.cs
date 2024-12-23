using System.Collections;
using UnityEngine;

public class BaseMonster : MonoBehaviour
{
    [Header("Monster Settings")]
    [SerializeField] public int health = 5;  //체력
    [SerializeField] public float knockbackDistance = 3f;    //넉백 거리
    [SerializeField] public float knockbackSpeed = 20f;  //넉백 속도
    public float proximityThreshold = 0.2f; //넉백 위치 근접 판정

    protected Rigidbody2D rigid;        // Rigidbody2D 컴포넌트
    protected Animator animator;        // Animator 컴포넌트

    [SerializeField] protected Vector2 knockbackPosition;  // 넉백 목표 위치
    [SerializeField] protected bool isKnockback = false;
    private Vector2 attackSource;

    protected virtual void Awake()
    {
        Collider2D myCollider = GetComponent<Collider2D>();

        GameObject[] objectsWithTargetTag = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject obj in objectsWithTargetTag)
        {
            // 태그가 맞는 오브젝트의 Collider와 충돌 무시 설정
            Collider2D targetCollider = obj.GetComponent<Collider2D>();
            if (targetCollider != null && myCollider != null)
            {
                Physics2D.IgnoreCollision(myCollider, targetCollider);
            }
        }


        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    protected virtual void FixedUpdate()
    {
        if(isKnockback)
        {
            
            Knockback(attackSource);

        }
    }

    public virtual void TakeDamage(int damage, Vector2 damageSourcePosition)
    {
        attackSource = damageSourcePosition;
        //체력감소
        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Remaining health: {health}");

        //죽음 처리
        if (health <= 0 )
        {
            Die();
            return;
        }



        // 넉백 방향 계산 ( x축만)
        Vector2 knockbackDir = new Vector2(rigid.position.x - damageSourcePosition.x, 0).normalized;

        Debug.Log(knockbackDir);

        //넉백 목표 위치 계산
        knockbackPosition = rigid.position + knockbackDir * knockbackDistance;
        Debug.Log(knockbackPosition);
        //isKnockback = true;


        isKnockback = true;
    }


    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        Destroy(gameObject);
    }


    protected virtual void Knockback(Vector2 damageSourcePosition)
    {
        if (!isKnockback) return;



        rigid.MovePosition(Vector2.MoveTowards(rigid.position, knockbackPosition, knockbackSpeed * Time.fixedDeltaTime));

        // 목표 위치에 도달하면 넉백 중지
        if (Vector2.Distance(rigid.position, knockbackPosition) <= proximityThreshold)
        {
            isKnockback = false;

        }

    }





    // Update is called once per frame
    void Update()
    {
        
    }
}
