using System.Collections;
using UnityEngine;

public class BaseMonster : MonoBehaviour
{
    [Header("Monster Settings")]
    [SerializeField] public int health = 5;  //체력
    [SerializeField] public float knockbackDistance = 3f;    //넉백 거리
    [SerializeField] public float knockbackSpeed = 20f;  //넉백 속도
    public float proximityThreshold = 0.1f; //넉백 위치 근접 판정

    protected Rigidbody2D rigid;        // Rigidbody2D 컴포넌트
    protected Animator animator;        // Animator 컴포넌트

    private Vector2 knockbackPosition;  // 넉백 목표 위치
    private bool isKnockback = false; 

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public virtual void TakeDamage(int damage, Vector2 damageSourcePosition)
    {
        //체력감소
        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Remaining health: {health}");

        //죽음 처리
        if (health <= 0 )
        {
            Die();
            return;
        }

        //넉백처리
        CalculateKnockback(damageSourcePosition);

    }


    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        Destroy(gameObject);
    }


    private void CalculateKnockback(Vector2 damageSourcePosition)
    {
        if (rigid == null) return;

        // 넉백 방향 계산 ( x축만)
        Vector2 dirVec = new Vector2(rigid.position.x - damageSourcePosition.x, 0).normalized;

        //넉백 목표 위치 계산
        knockbackPosition = rigid.position + dirVec * knockbackDistance;

        isKnockback = true;

        //넉백 처리 
        //StartCoroutine(KnockBackRoutine());

    }

    private void HandleKnockback()
    {
        rigid.MovePosition(Vector2.MoveTowards(rigid.position, knockbackPosition, knockbackSpeed * Time.fixedDeltaTime));

        // 목표 위치에 도달하면 넉백 중지
        if (Vector2.Distance(rigid.position, knockbackPosition) <= proximityThreshold)
        {
            isKnockback = false;
            OnKnockbackComplete();
        }

    }

    protected virtual void OnKnockbackComplete()
    {
        //넉백 완료 후 추가 행동( 상속받은 클래스에 확장 가능)
        Debug.Log($"{gameObject.name} completed knockback.");
    }


    //private IEnumerator KnockBackRoutine()
    //{
    //    while(isKnockback)
    //    {
    //        rigid.MovePosition(Vector2.MoveTowards(rigid.position, knockbackPosition, knockbackSpeed * Time.fixedDeltaTime));

    //        if (Vector2.Distance(rigid.position, knockbackPosition) <= 0.1f)
    //        {
    //            isKnockback = false;
    //        }

    //        yield return null;
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        
    }
}
