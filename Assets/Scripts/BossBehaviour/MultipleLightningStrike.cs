using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MultipleLightningStrike :StateMachineBehaviour
{
    public float timer;
    public float minTime;
    public float maxTime;

    public float lightningDelay; //번개가 떨어지는 지연 시간
    public float lightningTimer; // 위의 딜레이를 받아서 실제로 할당할 타이머값.

    GameObject boss; // 부모인 보스를 가져와야함
    LineRenderer lineRenderer; // 거기서 라인랜더러 가져오기
    ParticleSystem ps;
    BoxCollider2D bossRoom; // 보스룸 콜라이더 가져오기
    public GameObject lightningPrefab; // 번개 프리팹을 저장해놓을 변수

    [SerializeField]
    private float spacingX; //번개를 출력할 간격
    private float minSpacing; // 출력 최소간격 ( 번개의 최대개수를 미리 풀링해놓고 쓰고싶은 만큼만 꺼내쓰면 됀다.
    public float lightningHeight; //번개 출력 시작 높이
    public int maxLightningCount; //번개의 최대 생성 개수
    public int currentLightningCount;  // 현재 번개의 생성 개수
    private float lightningStartX;
    Vector2 lightningStartPos; // 번개의 생성 위치
    Vector2 lightningEndPos;

    //보스룸의 시작과 끝. 이걸기준으로 간격을 나눠 번개를 동시생성
    [SerializeField]
    private float roomMinX;
    [SerializeField]
    private float roomMaxX;

    List<GameObject> lightningPool; //컬링을 위해 비활성화된 번개를 담아놓을 큐

    bool canAttack = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        lightningTimer = lightningDelay;
        timer = Random.Range(minTime, maxTime); // idle 스테이트에 들어가면 임의의 대기시간을 가진다.

        minSpacing = 4;
        spacingX = Random.Range((int)minSpacing, 7);

        boss = animator.gameObject;
        bossRoom = boss.GetComponent<Boss>().bossRoomRange;

        canAttack = true;
        roomMinX = bossRoom.bounds.min.x;
        roomMaxX = bossRoom.bounds.max.x;

        maxLightningCount = Mathf.CeilToInt((roomMaxX - roomMinX) / minSpacing); // 생성할 번개의 개수
        currentLightningCount = Mathf.CeilToInt((roomMaxX - roomMinX) / spacingX); // 생성할 번개의 개수


        if (lightningPool == null)
        {
            lightningPool = new List<GameObject>();

            //번개의 개수만큼 오브젝트를 만들어서 비활성화 시킨 뒤 큐에 집어 넣는다.
            for (int i = 0; i < maxLightningCount; i++)
            {
                //위치는 어짜피 활성화될 때 바뀔것이므로 임의로 설정한다.
                GameObject lightning = Instantiate(lightningPrefab, boss.transform.position, Quaternion.Euler(90, 0, 0));
                lightning.SetActive(false);
                lightningPool.Add(lightning);
                Debug.Log("리스트에 프리팹 추가함");


            }
        }

    }


    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (lightningTimer >= 0)
        {

            for (int i = 0; i < currentLightningCount; i++)
            {
                //GameObject lineObj = GetPooledObject(lineRendererPool);
                lightningStartX = roomMinX + (spacingX * i);
                lightningStartPos = new Vector2(lightningStartX, bossRoom.bounds.max.y);
                lightningEndPos = new Vector2(lightningStartX, bossRoom.bounds.min.y);

                lightningPool[i].transform.position = lightningStartPos;
                lightningPool[i].SetActive(true);


                lineRenderer = lightningPool[i].GetComponent<LineRenderer>();
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, lightningStartPos);
                lineRenderer.SetPosition(1, lightningEndPos);

                Debug.Log("라인랜더러 활성화시킴");

            }

            lightningTimer -= Time.deltaTime;

        }

        else if (lightningTimer <= 0)
        {

            if (canAttack)
            {
                // 라인 랜더러를 끄고 번개 파티클 출력
                for (int i = 0; i < currentLightningCount; i++)
                {

                    lineRenderer = lightningPool[i].GetComponent<LineRenderer>();
                    if (lineRenderer.enabled)
                    {
                        lineRenderer.enabled = false;
                    }

                    lightningPool[i].SetActive(true);

                    ps = lightningPool[i].GetComponent<ParticleSystem>();

                    ActivatingLightning(ps);
                }
            }
            canAttack = false;

            timer -= Time.deltaTime;

        }

        if (timer <= 0)
        {
            for(int i =0; i < currentLightningCount;i++)
            {
                lightningPool[i].SetActive(false);
            }

            animator.SetTrigger("Idle");
        }




    }

    void ActivatingLightning(ParticleSystem ps)
    {

        if (ps != null)
        {
            Debug.Log("파티클이 잇긴함");
            ps.Stop();
            Debug.Log("재생중인거 멈춤");
            ps.Clear();
            Debug.Log("파티클 초기화");
            ps.Play();
            Debug.Log("파티클 재생");
        }
    }



}


// line renderer를 풀링하는 pool을 하나 더만들것이냐 
// 아니면 리스트에 넣어서 관리할 것이냐.