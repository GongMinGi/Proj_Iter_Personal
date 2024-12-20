using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator anim;          // Animator 컴포넌트 참조
    private int hashAttackCnt;      // Animator의 AttackCount 변수의 해시 값
    private int attackCount = 0;    // 현재 AttackCount 값
    private bool isAttacking = false;

    void Awake()
    {
        anim = GetComponent<Animator>();

        //Animator의 attackCount 변수 해시화
        hashAttackCnt = Animator.StringToHash("attackCount");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) )
        {
            PerformAttack();
        }
    }

    void PerformAttack()
    {
        if(isAttacking)
            return;
        attackCount++;

        if (attackCount >2)
        {
            attackCount = 1;
        }

        //Animator에 AttackCount 값 설정
        anim.SetInteger(hashAttackCnt, attackCount);


        //Trigger 설정으로 공격 시작
        anim.SetTrigger("Attack");
    }

    public void ResetAttackCount()
    {
        //공격이 끝난 후 AttackCount 초기화 
        //attackCount=0;
        //anim.SetInteger(hashAttackCnt, attackCount);
        isAttacking =false;
    }


}
