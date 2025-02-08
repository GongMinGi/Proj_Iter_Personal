using UnityEditor.Tilemaps;
using UnityEngine;

public class BossLightningStrikeBehavior : StateMachineBehaviour
{

    public float timer;
    public float minTime;
    public float maxTime;
    public float LightningDelay;
    public GameObject lightningStrike;

    [SerializeField]
    private float lightningHeight; // ������ �������� ����

    private Vector2 playerpos; // �÷��̾��� ��ġ
    private Vector2 originBossPos; // ������ ��ġ
    private Vector2 LightningStartPos; //������ �������� �����ϴ� ��ġ

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Lightning Strike");
        // �÷��̾��� ������ �Ʒ��� 1������ �����ɽ�Ʈ�� ��� 
        // �ð��� ������, ���������� ����
        GameObject boss = animator.gameObject;
        originBossPos = animator.transform.position;
        //�±׸� ���� �÷��̾��� ��ġ���� ���ϱ�
        playerpos = GameObject.FindGameObjectWithTag("Player").transform.position;
        LightningStartPos = new Vector2(playerpos.x, originBossPos.y + lightningHeight); // �÷��̾��� �Ӹ� �� ���� ������ �������� ���� ���

       
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log(LightningDelay);
        //����Ʈ�� �����̰� �������� ���� �����ɽ�Ʈ�� ���.
        if(LightningDelay >= 0)
        {
            
            //Physics2D.Raycast(LightningStartPos, Vector2.down * 10, Color.red);
            LightningDelay -= Time.deltaTime;
        }

        else if (LightningDelay <= 0)
        {
            timer -= Time.deltaTime;
            Instantiate(lightningStrike, LightningStartPos, Quaternion.identity);
        }
        
           

        if (timer <= 0)
        {
            animator.SetTrigger("Idle");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}


}
