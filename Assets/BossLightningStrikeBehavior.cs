using UnityEditor.Tilemaps;
using UnityEngine;

public class BossLightningStrikeBehavior : StateMachineBehaviour
{

    public float timer;
    public float minTime;
    public float maxTime;
    public float LightningDelay;
    public GameObject lightningStrike;
    private GameObject lightningObject; //������Ʈ �ø��� ���� �������� ���� ������Ʈ ����
    [SerializeField]
    private float lightningHeight; // ������ �������� ����
    


    [SerializeField]
    private float lightningTimer; // ������ ������Ʈ���� ���� �����̺���, OnStateEnter���� lightningDelay�� �ʱ�ȭ�������
    private LineRenderer lineRenderer; // ���� ��� ���� ���� ������ ��������
    private Vector2 playerpos; // �÷��̾��� ��ġ
    private Vector2 originBossPos; // ������ ��ġ
    private Vector2 lightningStartPos; //������ �������� �����ϴ� ��ġ
    private Vector2 lightningEndPos;

    private bool canAttack;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = Random.Range(minTime, maxTime);
        lightningTimer = LightningDelay;// ���� ������ �ʱ�ȭ


        GameObject boss = animator.gameObject; // ������Ʈ�� �����ϱ� ���� ���� ������Ʈ ������ ��������
        lineRenderer = boss.GetComponent<LineRenderer>(); //���η����� �Ҵ��ϱ�

        originBossPos = animator.transform.position;
        playerpos = GameObject.FindGameObjectWithTag("Player").transform.position;  //�±׸� ���� �÷��̾��� ��ġ���� ���ϱ�
        lightningStartPos = new Vector2(playerpos.x, originBossPos.y + lightningHeight); // �÷��̾��� �Ӹ� �� ���� ������ �������� ���� ���
        lightningEndPos = new Vector2(lightningStartPos.x, lightningStartPos.y - lightningHeight);


        //ù ���� �ÿ� �ѹ� �������� �־��ְ� ��Ȱ��ȭ���ѳ��´�.
        if(lightningObject == null)
        {
            lightningObject = Instantiate(lightningStrike, lightningStartPos, Quaternion.Euler(90, 0, 0));
            lightningObject.SetActive(false);
        }


    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log(lightningTimer);
        //����Ʈ�� �����̰� �������� ���� �����ɽ�Ʈ�� ���.
        if(lightningTimer >= 0)
        {
            canAttack = true;

            //Physics2D.Raycast(LightningStartPos, Vector2.down * 10, Color.red);
            lineRenderer.SetPosition(0, lightningStartPos);
            lineRenderer.SetPosition(1, lightningEndPos);
            lineRenderer.enabled = true;

            lightningTimer -= Time.deltaTime;
        }

        else if (lightningTimer <= 0)
        {
            if(lineRenderer.enabled)
            {
                lineRenderer.enabled = false;
            }

            if(canAttack)
            {
                
                Instantiate(lightningStrike, lightningStartPos, Quaternion.Euler(90, 0, 0));
                canAttack = false;
            }
            timer -= Time.deltaTime;

        }



        if (timer <= 0)
        {
            animator.SetTrigger("Idle");
        }
    }


    void ActivateLightning()
    {
        if(lightningObject != null)
        {
            lightningObject.transform.position = lightningStartPos;
            lightningObject.SetActive(true);

        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}


}
