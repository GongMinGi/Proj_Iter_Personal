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
    private float lightningTimer; // 실제로 업데이트에서 쓰일 딜레이변수, OnStateEnter에서 lightningDelay로 초기화해줘야함


    private GameObject boss;
    public GameObject lightningStrike; // 프리팹을 컴포넌트로 가져올 변수
    private GameObject lightningObject; //오브젝트 컬링을 위해 프리팹을 담을 오브젝트 변수
    ParticleSystem ps;


    [SerializeField]
    private float lightningHeight; // 번개가 떨어지는 높이
    



    private LineRenderer lineRenderer; // 번개 경고를 위한 라인 랜더러 가져오기


    private Vector2 playerpos; // 플레이어의 위치
    private Vector2 originBossPos; // 보스의 위치
    private Vector2 lightningStartPos; //번개가 떨어지기 시작하는 위치
    private Vector2 lightningEndPos;

    private bool canAttack;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = Random.Range(minTime, maxTime);
        lightningTimer = LightningDelay;// 공격 딜레이 초기화


        boss = animator.gameObject; // 컴포넌트에 접속하기 위해 보스 오브젝트 변수로 가져오기
        lineRenderer = boss.GetComponent<LineRenderer>(); //라인랜더러 할당하기




        //필요한 위치 초기화
        originBossPos = animator.transform.position;
        playerpos = GameObject.FindGameObjectWithTag("Player").transform.position;  //태그를 통해 플레이어의 위치벡터 구하기
        lightningStartPos = new Vector2(playerpos.x, originBossPos.y + lightningHeight); // 플레이어의 머리 위 조금 떨어진 지점에서 번개 출력
        lightningEndPos = new Vector2(lightningStartPos.x, lightningStartPos.y - lightningHeight);


        //첫 실행 시에 한번 프리팹을 넣어주고 비활성화시켜놓는다.
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
        //라이트닝 딜레이가 남아있을 때만 레이케스트를 쏜다.
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

        // 오브젝트에 번개 프리팹이 있으면
        if(lightningObject != null)
        {
            lightningObject.transform.position = lightningStartPos; //위치를 시작 위치로 바꾸고
            lightningObject.SetActive(true); // 활성화
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
