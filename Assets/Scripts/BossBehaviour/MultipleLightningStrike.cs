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
    BoxCollider2D bossRoom; // 보스룸 콜라이더 가져오기
    public GameObject lightningPrefab; // 번개 프리팹을 저장해놓을 변수

    private float spacingX; //번개를 출력할 간격
    public float lightningHeight; //번개 출력 시작 높이
    int lightningCount; //생성할 번개의 개수
    private float lightningStartX;
    Vector2 lightningStartPos; // 번개의 생성 위치
    Vector2 lightningEndPos;

    //보스룸의 시작과 끝. 이걸기준으로 간격을 나눠 번개를 동시생성
    private float roomMinX; 
    private float roomMaxX;

    Queue<GameObject> lightningPool; //컬링을 위해 비활성화된 번개를 담아놓을 큐
    Queue<GameObject> lineRendererPool;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        lightningTimer = lightningDelay;
        timer = Random.Range(minTime, maxTime); // idle 스테이트에 들어가면 임의의 대기시간을 가진다.


        boss = animator.gameObject;
        lineRenderer = boss.GetComponent<LineRenderer>();
        bossRoom = boss.GetComponent<BoxCollider2D>();
        lightningPool = new Queue<GameObject>();
        lineRendererPool = new Queue<GameObject>();

        roomMinX = bossRoom.bounds.min.x;
        roomMaxX = bossRoom.bounds.max.x;

        lightningCount = Mathf.FloorToInt((roomMaxX - roomMinX) / spacingX); // 생성할 번개의 개수

        //번개의 개수만큼 오브젝트를 만들어서 비활성화 시킨 뒤 큐에 집어 넣는다.
        for (int i = 0; i < lightningCount; i++)
        {
            //위치는 어짜피 활성화될 때 바뀔것이므로 임의로 설정한다.
            GameObject lightning = Instantiate(lightningPrefab, boss.transform.position , Quaternion.Euler(90,0,0));
            lightning.SetActive(false);
            lightningPool.Enqueue(lightning);

            //보스 오브젝트에 있는 라인 랜더러를 뜯어서 게임 오브젝트화
            // 라인 랜더러만 뜯어오면 활성 비활성이불가능하다.
            // 랜더러가 붙어있는 오브젝트를 뜯어와야 한다.그러면...
            // 라이트닝 프리팹의 랜더러를 뜯어오면 안됌..
            // 빈 오브젝트 만들어서 addcomponent로 보스에 잇는 라인랜더러 넣고 오브젝트 째로 큐에 넣자

            LineRenderer lineTemp = lineRenderer; // 필요한 라인 랜더러를 뜯어온다
            GameObject lineObj = new GameObject(); // 빈 오브젝트를 만든다
            lineObj = lineTemp.gameObject; // 여기에 넣는다.
            lineObj.SetActive(false); // 비활성화
            lineRendererPool.Enqueue(lineObj); // 큐에 넣기
        }
    }


    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(lightningTimer >= 0)
        {

            for (int i = 0; i < lightningCount; i++)
            {
                GameObject lineObj = GetPooledObject(lineRendererPool);
                lightningStartX = roomMinX + (spacingX * i);
                lightningStartPos = new Vector2(lightningStartX, bossRoom.bounds.max.y);
                lightningEndPos = new Vector2(lightningStartX, bossRoom.bounds.max.y - lightningHeight);

                lineObj.transform.position = lightningStartPos;
                lineObj.SetActive(true);

                lineObj.GetComponent<LineRenderer>().SetPosition(0, lightningStartPos);
                lineObj.GetComponent<LineRenderer>().SetPosition(1, lightningEndPos);
                UnableObject(lineObj, lineRendererPool, lightningDelay);
            }

            lightningTimer -= Time.deltaTime;

        }

        else if(lightningTimer <= 0)
        {
            // 라인 랜더러를 끄고 번개 파티클 출력
            for (int i = 0; i < lightningCount; i++)
            {

                GameObject lightning = GetPooledObject(lightningPool);
                lightningStartX = roomMinX + (spacingX * i);
                lightningStartPos = new Vector2(lightningStartX, bossRoom.bounds.max.y);
                lightningEndPos = new Vector2(lightningStartX, bossRoom.bounds.max.y - lightningHeight);


                lightning.transform.position = lightningStartPos;
                lightning.SetActive(true);

                lightning.GetComponent<ParticleSystem>().Play();
                UnableObject(lightning, lightningPool, timer);

            }

            timer -= Time.deltaTime;

        }

        if (timer <= 0)
        {
            animator.SetTrigger("Idle");
        }




    }


    GameObject GetPooledObject(Queue<GameObject> pool)
    {
        if (lightningPool.Count > 0)
        {
            return lightningPool.Dequeue();
        }
        else return null;
    }

    IEnumerator UnableObject(GameObject obj, Queue<GameObject> pool,  float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        pool.Enqueue(obj);


    }
}


// line renderer를 풀링하는 pool을 하나 더만들것이냐 
// 아니면 리스트에 넣어서 관리할 것이냐.