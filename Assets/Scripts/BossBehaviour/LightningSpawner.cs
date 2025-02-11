using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningSpawner : MonoBehaviour
{
    public GameObject lightningPrefab; // ������ ������ ���� ������Ʈ
    public int poolSize = 10; // ������ ������ ��
    private Queue<GameObject> lightningPool = new Queue<GameObject>(); //������Ʈ Ǯ���� ����� ť

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
        //�������� ��ŭ�� ������ ������Ʈȭ ���Ѽ� ť�� ����ִ� ����
        for (int i = 0; i < poolSize; i++)
        {
            GameObject lightning = Instantiate(lightningPrefab); // �������� �����Ѵ�.
            lightning.SetActive(false); //��Ȱ��ȭ��Ų��
            lightningPool.Enqueue(lightning); // ť�� ����ִ´�.
        }
    }


    //������ �ð� �������� spawnLightning �Լ� ����
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
                //(-5, 5) ���� ������ ������������ ���� ��ġ�� ���Ѵ�.
                Vector2 spawnPos = new Vector2(startPoint.x + (i * spacingX), startPoint.y - (j * spacingY));
                GameObject lightning = GetPooledObject(); // ���� ������Ʈ�� ������
                if (lightning != null) // Ȥ�ø𸣴� nullüũ
                {
                    lightning.transform.position = spawnPos; // ������ ������ ��ġ�� ���߰�
                    lightning.SetActive(true); // Ȱ��ȭ
                    StartCoroutine(DisableAfterTime(lightning, 1.5f)); // 1.5�� �� ���� ��Ȱ��ȭ
                }
            }
        }
    }

    //ť�� ���� ������Ʈ�� �����ִٸ� ������.
    GameObject GetPooledObject()
    {
        if (lightningPool.Count > 0)
        {
            return lightningPool.Dequeue();
        }
        return null;
    }


    //������ ������ �Ŀ� ����Ǵ� �ڷ�ƾ
    IEnumerator DisableAfterTime(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay); // �Ű������� ���� �����̸�ŭ ���
        obj.SetActive(false); // ���� ��Ȱ��ȭ
        lightningPool.Enqueue(obj); // ��Ȱ��ȭ�� ������ �ٽ� ť�� ����ִ´�.
    }
}



/*
 * - ���ڷ� ������ ���� �ƴϹǷ� x�ุ ����ϸ� �׸�,
 * - ���� ��ġ�� ��� ���������� ����.
 * - ������ �������� ������ ������ ��, �ƴϸ� ���� ä��´������� ������ �������� ����غ�����.
 */