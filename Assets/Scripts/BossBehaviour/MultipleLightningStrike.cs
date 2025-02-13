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
    ParticleSystem ps;
    BoxCollider2D bossRoom; // ������ �ݶ��̴� ��������
    public GameObject lightningPrefab; // ���� �������� �����س��� ����

    [SerializeField]
    private float spacingX; //������ ����� ����
    private float minSpacing; // ��� �ּҰ��� ( ������ �ִ밳���� �̸� Ǯ���س��� ������� ��ŭ�� �������� �´�.
    public float lightningHeight; //���� ��� ���� ����
    public int maxLightningCount; //������ �ִ� ���� ����
    public int currentLightningCount;  // ���� ������ ���� ����
    private float lightningStartX;
    Vector2 lightningStartPos; // ������ ���� ��ġ
    Vector2 lightningEndPos;

    //�������� ���۰� ��. �̰ɱ������� ������ ���� ������ ���û���
    [SerializeField]
    private float roomMinX;
    [SerializeField]
    private float roomMaxX;

    List<GameObject> lightningPool; //�ø��� ���� ��Ȱ��ȭ�� ������ ��Ƴ��� ť

    bool canAttack = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        lightningTimer = lightningDelay;
        timer = Random.Range(minTime, maxTime); // idle ������Ʈ�� ���� ������ ���ð��� ������.

        minSpacing = 4;
        spacingX = Random.Range((int)minSpacing, 7);

        boss = animator.gameObject;
        bossRoom = boss.GetComponent<Boss>().bossRoomRange;

        canAttack = true;
        roomMinX = bossRoom.bounds.min.x;
        roomMaxX = bossRoom.bounds.max.x;

        maxLightningCount = Mathf.CeilToInt((roomMaxX - roomMinX) / minSpacing); // ������ ������ ����
        currentLightningCount = Mathf.CeilToInt((roomMaxX - roomMinX) / spacingX); // ������ ������ ����


        if (lightningPool == null)
        {
            lightningPool = new List<GameObject>();

            //������ ������ŭ ������Ʈ�� ���� ��Ȱ��ȭ ��Ų �� ť�� ���� �ִ´�.
            for (int i = 0; i < maxLightningCount; i++)
            {
                //��ġ�� ��¥�� Ȱ��ȭ�� �� �ٲ���̹Ƿ� ���Ƿ� �����Ѵ�.
                GameObject lightning = Instantiate(lightningPrefab, boss.transform.position, Quaternion.Euler(90, 0, 0));
                lightning.SetActive(false);
                lightningPool.Add(lightning);
                Debug.Log("����Ʈ�� ������ �߰���");


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

                Debug.Log("���η����� Ȱ��ȭ��Ŵ");

            }

            lightningTimer -= Time.deltaTime;

        }

        else if (lightningTimer <= 0)
        {

            if (canAttack)
            {
                // ���� �������� ���� ���� ��ƼŬ ���
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
            Debug.Log("��ƼŬ�� �ձ���");
            ps.Stop();
            Debug.Log("������ΰ� ����");
            ps.Clear();
            Debug.Log("��ƼŬ �ʱ�ȭ");
            ps.Play();
            Debug.Log("��ƼŬ ���");
        }
    }



}


// line renderer�� Ǯ���ϴ� pool�� �ϳ� ��������̳� 
// �ƴϸ� ����Ʈ�� �־ ������ ���̳�.