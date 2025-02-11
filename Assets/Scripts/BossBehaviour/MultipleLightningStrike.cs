using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MultipleLightningStrike :StateMachineBehaviour
{
    public float timer;
    public float minTime;
    public float maxTime;

    public float lightningDelay; //������ �������� ���� �ð�
    public float lightningTimer; // ���� �����̸� �޾Ƽ� ������ �Ҵ��� Ÿ�̸Ӱ�.

    GameObject boss; // �θ��� ������ �����;���
    LineRenderer lineRenderer; // �ű⼭ ���η����� ��������
    BoxCollider2D bossRoom; // ������ �ݶ��̴� ��������
    public GameObject lightningPrefab; // ���� �������� �����س��� ����

    private float spacingX; //������ ����� ����
    public float lightningHeight; //���� ��� ���� ����
    int lightningCount; //������ ������ ����
    private float lightningStartX;
    Vector2 lightningStartPos; // ������ ���� ��ġ
    Vector2 lightningEndPos;

    //�������� ���۰� ��. �̰ɱ������� ������ ���� ������ ���û���
    private float roomMinX; 
    private float roomMaxX;

    Queue<GameObject> lightningPool; //�ø��� ���� ��Ȱ��ȭ�� ������ ��Ƴ��� ť
    Queue<GameObject> lineRendererPool;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        lightningTimer = lightningDelay;
        timer = Random.Range(minTime, maxTime); // idle ������Ʈ�� ���� ������ ���ð��� ������.


        boss = animator.gameObject;
        lineRenderer = boss.GetComponent<LineRenderer>();
        bossRoom = boss.GetComponent<BoxCollider2D>();
        lightningPool = new Queue<GameObject>();
        lineRendererPool = new Queue<GameObject>();

        roomMinX = bossRoom.bounds.min.x;
        roomMaxX = bossRoom.bounds.max.x;

        lightningCount = Mathf.FloorToInt((roomMaxX - roomMinX) / spacingX); // ������ ������ ����

        //������ ������ŭ ������Ʈ�� ���� ��Ȱ��ȭ ��Ų �� ť�� ���� �ִ´�.
        for (int i = 0; i < lightningCount; i++)
        {
            //��ġ�� ��¥�� Ȱ��ȭ�� �� �ٲ���̹Ƿ� ���Ƿ� �����Ѵ�.
            GameObject lightning = Instantiate(lightningPrefab, boss.transform.position , Quaternion.Euler(90,0,0));
            lightning.SetActive(false);
            lightningPool.Enqueue(lightning);

            //���� ������Ʈ�� �ִ� ���� �������� �� ���� ������Ʈȭ
            // ���� �������� ������ Ȱ�� ��Ȱ���̺Ұ����ϴ�.
            // �������� �پ��ִ� ������Ʈ�� ���;� �Ѵ�.�׷���...
            // ����Ʈ�� �������� �������� ������ �ȉ�..
            // �� ������Ʈ ���� addcomponent�� ������ �մ� ���η����� �ְ� ������Ʈ °�� ť�� ����

            LineRenderer lineTemp = lineRenderer; // �ʿ��� ���� �������� ���´�
            GameObject lineObj = new GameObject(); // �� ������Ʈ�� �����
            lineObj = lineTemp.gameObject; // ���⿡ �ִ´�.
            lineObj.SetActive(false); // ��Ȱ��ȭ
            lineRendererPool.Enqueue(lineObj); // ť�� �ֱ�
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
            // ���� �������� ���� ���� ��ƼŬ ���
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


// line renderer�� Ǯ���ϴ� pool�� �ϳ� ��������̳� 
// �ƴϸ� ����Ʈ�� �־ ������ ���̳�.