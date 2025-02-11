using System.Collections;
using UnityEditor.Tilemaps;
using UnityEngine;

public class BossLightningStrikeBehavior : StateMachineBehaviour
{

    public float timer;
    public float minTime;
    public float maxTime;
    public float LightningDelay;
    [SerializeField]
    private float lightningTimer; // ������ ������Ʈ���� ���� �����̺���, OnStateEnter���� lightningDelay�� �ʱ�ȭ�������


    private GameObject boss;
    public GameObject lightningStrike; // �������� ������Ʈ�� ������ ����
    private GameObject lightningObject; //������Ʈ �ø��� ���� �������� ���� ������Ʈ ����
    ParticleSystem ps;


    [SerializeField]
    private float lightningHeight; // ������ �������� ����
    



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


        boss = animator.gameObject; // ������Ʈ�� �����ϱ� ���� ���� ������Ʈ ������ ��������
        lineRenderer = boss.GetComponent<LineRenderer>(); //���η����� �Ҵ��ϱ�




        //�ʿ��� ��ġ �ʱ�ȭ
        originBossPos = animator.transform.position;
        playerpos = GameObject.FindGameObjectWithTag("Player").transform.position;  //�±׸� ���� �÷��̾��� ��ġ���� ���ϱ�
        lightningStartPos = new Vector2(playerpos.x, originBossPos.y + lightningHeight); // �÷��̾��� �Ӹ� �� ���� ������ �������� ���� ���
        lightningEndPos = new Vector2(lightningStartPos.x, lightningStartPos.y - lightningHeight);


        //ù ���� �ÿ� �ѹ� �������� �־��ְ� ��Ȱ��ȭ���ѳ��´�.
        if(lightningObject == null)
        {
            lightningObject = Instantiate(lightningStrike, lightningStartPos, Quaternion.Euler(90, 0, 0));
            ps = lightningObject.GetComponentInParent<ParticleSystem>();

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
                ActivateLightning();
                
                //Instantiate(lightningStrike, lightningStartPos, Quaternion.Euler(90, 0, 0));
                canAttack = false;
            }
            timer -= Time.deltaTime;

        }



        if (timer <= 0)
        {
            lightningObject.SetActive(false);

            
            animator.SetTrigger("Idle");
        }
    }


    void ActivateLightning()
    {

        // ������Ʈ�� ���� �������� ������
        if(lightningObject != null)
        {
            lightningObject.transform.position = lightningStartPos; //��ġ�� ���� ��ġ�� �ٲٰ�
            lightningObject.SetActive(true); // Ȱ��ȭ
            if(ps != null)
            {
                ps.Play();
            }

        }
    }



    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}


}
