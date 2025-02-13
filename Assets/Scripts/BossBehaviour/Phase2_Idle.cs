using UnityEngine;

public class Phase2_Idle : StateMachineBehaviour
{
    [SerializeField]
    private float timer;
    public float minTime;
    public float maxTime;

    private int nextBehavior;

    private Boss boss; // ü�������� ������ �ִ� ZEUS ������Ʈ�� Boss ������Ʈ�� �������� ���� ����


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        timer = Random.Range(minTime, maxTime); // idle ������Ʈ�� ���� ������ ���ð��� ������.
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (timer <= 0) //���ð��� �����ٸ�,
        {

            nextBehavior = Random.Range(0, 3);  // �������ݰ� ���� ������ ������ �������� �������� ����.
            //nextBehavior = 2;
            if (nextBehavior == 0)
            {
                animator.SetTrigger("JumpAttack");
            }
            else if (nextBehavior == 1)
            {
                animator.SetTrigger("LightningStrike");

            }
            else
                animator.SetTrigger("MultipleLightningStrike");



        }

        else
            timer -= Time.deltaTime;    //���ð��� ������ �ʾҴٸ�, ���ð��� ���ҽ�Ų��.

    }



}
