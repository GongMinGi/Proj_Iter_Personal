using UnityEngine;

public class BossJumpAttackBehavior : StateMachineBehaviour
{

    public float timer;
    public float minTime;
    public float maxTime;

    private float speed; // ���� ����Ǵ� ���ǵ�
    
    public float jumpSpeed;  //�������� ���ǵ�
    public float stampingSpeed;  // ������� ���ǵ�

    private int remainSequence = 2; // ���� ������ ���� ����

    private Vector2 playerPos;
    private Vector2 originBossPos;
    Vector2 target;



    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        remainSequence = 2;


        playerPos = GameObject.FindGameObjectWithTag("Player").transform.position; // �÷��̾��� transform�� ���ϱ�
        originBossPos = animator.transform.position; // �ִϸ����Ϳ��� ���� ������ transform�� ���� ����.
        speed = jumpSpeed;

        target = playerPos;
        target.y += originBossPos.y + 3f; // ���� ������ y ��ġ���� 3��ŭ ���� ������ �����ϵ���
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        animator.transform.position = Vector2.MoveTowards(animator.transform.position, target, speed * Time.deltaTime);



        if (animator.transform.position.x == target.x &&
            animator.transform.position.y == target.y && 
            remainSequence == 2 ) // Ÿ�� ��ġ ���� ����������, �� ��ġ���� ������ ���� ���־��� y��ǥ���� �����ﵵ�� Ÿ�� ����.
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
