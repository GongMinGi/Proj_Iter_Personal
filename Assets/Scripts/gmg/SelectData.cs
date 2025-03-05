using JetBrains.Annotations;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectData : MonoBehaviour
{
    //����ִ� ������ ������ �� �ߴ� â
    public GameObject createViewPort;
    public Text[] slotText;             //�����͸� ������ �� ������ ������ �ٲ��� �ϱ� ������ ������ �ؽ�Ʈ�� ������ ������ �־�� ��
    public Text newPlayerName;           // ���ο� ���� �����͸� ���� �� �Է¹��� �÷��̾� �̸�

    bool[] savefile = new bool[3];

    void Start()
    {
        // ���� ���� ����� �����Ͱ� �����ϴ��� �Ǵ�
        // ������ ��θ� Ȯ���ؾ� ��.
        for(int i =0; i< 3; i++)
        {
            //0~2���� ���鼭 ������ �����ϴ��� �ƴ��� bool������ ��ȯ�Ѵ�.
            if(File.Exists(DataManager.Instance.path + $"{i}"))
            {
                savefile[i] = true;
                DataManager.Instance.nowSlot = i;   // �����Ͱ� �����Ѵٸ�, ���� ���Կ� �ش� ������ ��ȣ�� �Է�
                DataManager.Instance.LoadData();    // �ش� ���Կ� ����� �����͸� �ҷ��� DataManager�� nowPlayer�� �Ҵ�
                slotText[i].text = DataManager.Instance.nowPlayer.name; // �ҷ��� �������� �÷��̾� �̸��� ������ �ؽ�Ʈ�� ������´�.
            }
            else // ������ �������� �ʴ´ٸ�,
            {
                slotText[i].text = "��� ����";
            }

        }
        //DataManager.Instance.DataClear(); // ������ �����͸� �ݿ��Ϸ��� ���� �ƴ� ���Կ� ǥ�ø� �Ϸ��� ���̱� ������, ���� �ҷ��� ������ �ʱ�ȭ���ش�.


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //���Կ� �� �Լ�
    //������ 3���ε� ��� �˸°� �ҷ����°�
    //���� ��ư Ŭ���� ȣ��Ǵ� �޼���, Ŭ���� ������ ��ȣ(number)�� �Ű������� ����
    public void Slot(int number)
    {
        DataManager.Instance.nowSlot = number; // �Լ��� ������ �� �Էµ� ���ڰ� ���콽�Կ� ��

        if (savefile[number])
        {
            // 2. ����� �����Ͱ� ���� �� => �ҷ����� �ؼ� ���Ӿ����� �Ѿ
            DataManager.Instance.LoadData();
            GoGame(); // ���̺굥���Ͱ� ���� ���� ���� ������ �ٷ� �Ѿ��.

        }
        else  // 1. ����� �����Ͱ� ���� ��
        {
            createNewPlayer(); // �� �÷��̾� ������ ���� UI â�� Ȱ��ȭ�ϴ� �޼��� (�� ���� Ŭ�� �� �� ������ ����)

        }


    }

    //�� ������ ������ ĳ���͸� ���� ������Ų��.
    public void createNewPlayer()
    {
        createViewPort.gameObject.SetActive(true);  //�÷��̾� ���� â�� Ȱ��ȭ�Ͽ� ȭ�鿡 ǥ��.
    }


    public void GoGame()
    {
        //���� ������ �̵��ϱ� ���� �Է��� ������ �̹� ����Ǿ��ִ� ��� �� ���� �Է��ϴ� ���̳� ���������̱� ������ �̸��� �������� ���װ� �����.
        if (!savefile[DataManager.Instance.nowSlot]) // ���� ���Կ� �����Ͱ� ����Ǿ� ���� �ʴٸ�,
        {
            DataManager.Instance.nowPlayer.name = newPlayerName.text; //������ ���� �� �Է��ߴ� �̸��� ���絥������ �̸��� ����Ѵ�.
            DataManager.Instance.SaveData();                            // �� �����͸� ���Ͽ� ����
            SceneManager.LoadScene(1); // �ε��� 1�� �ش��ϴ� ��(���� ��)���� ��ȯ
        }
        SceneManager.LoadScene(DataManager.Instance.nowPlayer.nowScene); // �����Ϳ� ����Ǿ��ִ� Scene���� �̵�

    }



}
