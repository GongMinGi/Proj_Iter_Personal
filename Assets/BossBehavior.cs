using UnityEngine;

public class BossBehavior : StateMachineBehaviour
{

    private int rand;
    private Animator animator;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.animator = animator; // �ִϸ����� ������Ʈ�� ����
        rand = Random.Range(0, 3);// 0~2 �� ���� �� ���� 

        if(rand == 0 )
        {
            animator.SetTrigger("Idle");
        }

        else if( rand == 1 )
        {
            animator.SetTrigger("JumpAttack");
        }

        else
        {
            animator.SetTrigger("LightningStrike");
        }

    }


    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

}
