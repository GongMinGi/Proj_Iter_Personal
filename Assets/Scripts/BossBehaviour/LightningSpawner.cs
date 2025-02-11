using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningSpawner : MonoBehaviour
{
    public GameObject lightningPrefab; // 번개를 저장할 게임 오브젝트
    public int poolSize = 10; // 생성할 번개의 수
    private Queue<GameObject> lightningPool = new Queue<GameObject>(); //오브젝트 풀링에 사용할 큐

    public int columns = 5;
    public int rows = 3;
    public float spacingX = 2f;
    public float spacingY = 2f;
    public Vector2 startPoint = new Vector2(-5, 5);
    public float interval = 2f;

    void Start()
    {
        InitializePool();
        StartCoroutine(SpawnLightningRoutine());
    }

    void InitializePool()
    {
        //만들고싶은 만큼의 번개를 오브젝트화 시켜서 큐에 집어넣는 과정
        for (int i = 0; i < poolSize; i++)
        {
            GameObject lightning = Instantiate(lightningPrefab); // 프리팹을 저장한다.
            lightning.SetActive(false); //비활성화시킨다
            lightningPool.Enqueue(lightning); // 큐에 집어넣는다.
        }
    }


    //일정한 시간 간격으로 spawnLightning 함수 실행
    IEnumerator SpawnLightningRoutine()
    {
        while (true)
        {
            SpawnLightning();
            yield return new WaitForSeconds(interval);
        }
    }

    void SpawnLightning()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                //(-5, 5) 왼쪽 위부터 일정간격으로 생성 위치를 정한다.
                Vector2 spawnPos = new Vector2(startPoint.x + (i * spacingX), startPoint.y - (j * spacingY));
                GameObject lightning = GetPooledObject(); // 번개 오브젝트를 꺼내옴
                if (lightning != null) // 혹시모르는 null체크
                {
                    lightning.transform.position = spawnPos; // 꺼내온 번개의 위치를 맞추고
                    lightning.SetActive(true); // 활성화
                    StartCoroutine(DisableAfterTime(lightning, 1.5f)); // 1.5초 후 번개 비활성화
                }
            }
        }
    }

    //큐에 번개 오브젝트가 남아있다면 꺼낸다.
    GameObject GetPooledObject()
    {
        if (lightningPool.Count > 0)
        {
            return lightningPool.Dequeue();
        }
        return null;
    }


    //번개가 생성된 후에 실행되는 코루틴
    IEnumerator DisableAfterTime(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay); // 매개변수로 받은 딜레이만큼 대기
        obj.SetActive(false); // 번개 비활성화
        lightningPool.Enqueue(obj); // 비활성화된 번개를 다시 큐에 집어넣는다.
    }
}



/*
 * - 격자로 생성할 것이 아니므로 x축만 고려하면 그만,
 * - 시작 위치를 어떻게 생성할지가 관건.
 * - 보스를 기준으로 번개를 생성할 지, 아니면 맵을 채우는느낌으로 번개를 생성할지 고민해봐야함.
 */