using UnityEditor.Tilemaps;
using UnityEngine;

public class BossLightningStrikeBehavior : StateMachineBehaviour
{

    public float timer;
    public float minTime;
    public float maxTime;
    public float LightningDelay;
    public GameObject lightningStrike;
    private GameObject lightningObject; //오브젝트 컬링을 위해 프리팹을 담을 오브젝트 변수
    [SerializeField]
    private float lightningHeight; // 번개가 떨어지는 높이
    


    [SerializeField]
    private float lightningTimer; // 실제로 업데이트에서 쓰일 딜레이변수, OnStateEnter에서 lightningDelay로 초기화해줘야함
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


        GameObject boss = animator.gameObject; // 컴포넌트에 접속하기 위해 보스 오브젝트 변수로 가져오기
        lineRenderer = boss.GetComponent<LineRenderer>(); //라인랜더러 할당하기

        originBossPos = animator.transform.position;
        playerpos = GameObject.FindGameObjectWithTag("Player").transform.position;  //태그를 통해 플레이어의 위치벡터 구하기
        lightningStartPos = new Vector2(playerpos.x, originBossPos.y + lightningHeight); // 플레이어의 머리 위 조금 떨어진 지점에서 번개 출력
        lightningEndPos = new Vector2(lightningStartPos.x, lightningStartPos.y - lightningHeight);


        //첫 실행 시에 한번 프리팹을 넣어주고 비활성화시켜놓는다.
        if(lightningObject == null)
        {
            lightningObject = Instantiate(lightningStrike, lightningStartPos, Quaternion.Euler(90, 0, 0));
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
