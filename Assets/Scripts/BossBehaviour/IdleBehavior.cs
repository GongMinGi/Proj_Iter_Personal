using UnityEngine;

public class IdleBehavior : StateMachineBehaviour
{
    [SerializeField]
    private float timer;
    public float minTime;
    public float maxTime;

    private int nextBehavior;

    private Boss boss; // 체력정보를 가지고 있는 ZEUS 오브젝트의 Boss 컴포넌트를 가져오기 위한 변수


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.GetComponentInParent<Boss>();

        timer = Random.Range(minTime, maxTime); // idle 스테이트에 들어가면 임의의 대기시간을 가진다.
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (timer <= 0) //대기시간이 끝났다면,
        {
            if (boss.currentHealth <= boss.maxHealth /2) // 만약 idle이 끝나고 체력이 절반 이하라면 2페이즈로 이동
            {
                animator.SetTrigger("PhaseTwo");
            }

            else // 그렇지 않다면 계속해서 1페이즈 진행
            {
                nextBehavior = Random.Range(0, 2);  // 점프공격과 벼락 공격중 무엇을 실행할지 랜덤으로 고른다.

                if (nextBehavior == 0)
                {
                    animator.SetTrigger("JumpAttack");
                }
                else
                    animator.SetTrigger("LightningStrike");
            }


        }

        else
            timer -= Time.deltaTime;    //대기시간이 끝나지 않았다면, 대기시간을 감소시킨다.

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}


}
