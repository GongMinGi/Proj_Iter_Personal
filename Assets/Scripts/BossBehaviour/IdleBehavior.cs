using UnityEngine;

public class IdleBehavior : StateMachineBehaviour
{
    [SerializeField]
    private float timer;
    public float minTime;
    public float maxTime;

    private int nextBehavior;

    private Boss boss; // ü�������� ������ �ִ� ZEUS ������Ʈ�� Boss ������Ʈ�� �������� ���� ����


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.GetComponentInParent<Boss>();

        timer = Random.Range(minTime, maxTime); // idle ������Ʈ�� ���� ������ ���ð��� ������.
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (timer <= 0) //���ð��� �����ٸ�,
        {
            if (boss.currentHealth <= boss.maxHealth /2) // ���� idle�� ������ ü���� ���� ���϶�� 2������� �̵�
            {
                animator.SetTrigger("PhaseTwo");
            }

            else // �׷��� �ʴٸ� ����ؼ� 1������ ����
            {
                nextBehavior = Random.Range(0, 2);  // �������ݰ� ���� ������ ������ �������� �������� ����.

                if (nextBehavior == 0)
                {
                    animator.SetTrigger("JumpAttack");
                }
                else
                    animator.SetTrigger("LightningStrike");
            }


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
