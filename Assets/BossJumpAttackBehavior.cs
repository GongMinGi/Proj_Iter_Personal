using UnityEngine;

public class BossJumpAttackBehavior : StateMachineBehaviour
{

    public float timer;
    public float minTime;
    public float maxTime;

    private float speed; // 실제 적용되는 스피드
    
    public float jumpSpeed;  //떠오르는 스피드
    public float stampingSpeed;  // 내려찍는 스피드

    private int remainSequence = 2; // 점프 공격의 남은 절차

    private Vector2 playerPos;
    private Vector2 originBossPos;
    Vector2 target;



    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        remainSequence = 2;


        playerPos = GameObject.FindGameObjectWithTag("Player").transform.position; // 플레이어의 transform을 구하기
        originBossPos = animator.transform.position; // 애니메이터에서 현재 보스의 transform에 접근 가능.
        speed = jumpSpeed;

        target = playerPos;
        target.y += originBossPos.y + 3f; // 현재 보스의 y 위치에서 3만큼 높은 곳까지 점프하도록
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        animator.transform.position = Vector2.MoveTowards(animator.transform.position, target, speed * Time.deltaTime);



        if (animator.transform.position.x == target.x &&
            animator.transform.position.y == target.y && 
            remainSequence == 2 ) // 타겟 위치 위에 도달햇으면, 그 위치에서 보스가 원래 서있었던 y좌표까지 내려찍도록 타겟 수정.
        {
            remainSequence--;
            target = new Vector2(playerPos.x, originBossPos.y) ;
            speed = stampingSpeed;
        }

        if (animator.transform.position.y == target.y &&
            animator.transform.position.x == target.x &&
            remainSequence == 1)
        {
            remainSequence--;

            animator.SetTrigger("Idle");

        }




    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}


}
