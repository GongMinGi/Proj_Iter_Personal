using UnityEngine;

public class ResetAttackState : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");

        var attack = animator.GetComponent<PlayerAttack>();
        if (attack != null)
        {
            attack.ResetAttackCount();
        }
    }
}
