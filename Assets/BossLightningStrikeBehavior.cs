using UnityEngine;

public class BossLightningStrikeBehavior : StateMachineBehaviour
{

    public float timer;
    public float minTime;
    public float maxTime;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Lightning Strike");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(timer <= 0)
        {
            animator.SetTrigger("Idle");
        }
        else
            timer -= Time.deltaTime; 
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}


}
