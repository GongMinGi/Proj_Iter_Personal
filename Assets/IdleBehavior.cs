using UnityEngine;

public class IdleBehavior : StateMachineBehaviour
{
    [SerializeField]
    private float timer;
    public float minTime;
    public float maxTime;

    private int nextBehavior;

    private Transform boss;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        timer = Random.Range(minTime, maxTime); // idle ������Ʈ�� ���� ������ ���ð��� ������.
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (timer <= 0) //���ð��� �����ٸ�,
        {
            nextBehavior = Random.Range(0, 2);  // �������ݰ� ���� ������ ������ �������� �������� ����.

            if (nextBehavior == 0)
            {
                animator.SetTrigger("JumpAttack");
            }
            else
                animator.SetTrigger("LightningStrike");
        }

        else
            timer -= Time.deltaTime;    //���ð��� ������ �ʾҴٸ�, ���ð��� ���ҽ�Ų��.

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}


}
