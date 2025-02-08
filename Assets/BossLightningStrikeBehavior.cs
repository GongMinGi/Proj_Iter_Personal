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
    private float lightningHeight; // 번개가 떨어지는 높이

    private Vector2 playerpos; // 플레이어의 위치
    private Vector2 originBossPos; // 보스의 위치
    private Vector2 LightningStartPos; //번개가 떨어지기 시작하는 위치

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Lightning Strike");
        // 플레이어의 위에서 아래로 1초정도 레이케스트로 경고 
        // 시간이 지나면, 번개프리팹 생성
        GameObject boss = animator.gameObject;
        originBossPos = animator.transform.position;
        //태그를 통해 플레이어의 위치벡터 구하기
        playerpos = GameObject.FindGameObjectWithTag("Player").transform.position;
        LightningStartPos = new Vector2(playerpos.x, originBossPos.y + lightningHeight); // 플레이어의 머리 위 조금 떨어진 지점에서 번개 출력

       
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log(LightningDelay);
        //라이트닝 딜레이가 남아있을 때만 레이케스트를 쏜다.
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
