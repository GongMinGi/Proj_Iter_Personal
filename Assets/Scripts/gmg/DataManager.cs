using Unity.VisualScripting;
using UnityEngine;
using System.IO; //��򰡷� �ְų� �����ö� ���� input /output

/* ���� ���
 *  1. ������ �����Ͱ� ����
 *  2. �����и� ���̽����� ��ȯ
 *  3. ���̽��� �ܺο� ����
 *  
 * �ҷ�����
 *  1. �ܺο� ������ ���̽��� ������
 *  2. ���̽��� ������ ���·� ��ȯ
 *  3. �ҷ��� �����͸� ���
 */


public class PlayerData
{
    // �̸�, ����, ����, �������� ����
    public string nowScene;
    public string name;
    public int curHealth =15;   // ���� ���� �ʱⰪ�� ����
    public int maxHealth = 15;
    public Vector3 playerPos;

}


//������ ���� �� ����/�ҷ����� ����� ����ϴ� Monobehaviour Ŭ����
public class DataManager : MonoBehaviour
{
    //DataManager�� �̱��� �ν��Ͻ� (������Ʈ ������ �ϳ��� �����ϰ� ��)
    public static DataManager Instance;
    
    // ���� �÷��̾� �����͸� ������ ����( playerData Ÿ���� �ν��Ͻ�)
    public PlayerData nowPlayer = new PlayerData();

    public string path;                //���������� ��θ� ������ ����
    //string filename = "save";   //���������� �̸��� ���� (Ȯ���� ���� �ܼ� �̸�), ����� �Ʒ��� path������ save�� ���� ���ִ�
    public int nowSlot;         //���� �̸��� ���Ը��� ��������, nowSlot ������ �̿��ؼ� ���� ���忡 ����ϴ� ���� ��ȣ�� �Ű��ش�.

    // ��ũ��Ʈ �ν��Ͻ��� Ȱ��ȭ �� �� ȣ��
    private void Awake()
    {
        //���ø����̼��� ���� ������ ���� ��θ� ������ path ������ ����
        //persistentDataPath�� �÷����� �´� ��θ� ���� ( ����Ƽ�� ��� ������Ʈ ����)
        path = Application.persistentDataPath + "/save"; // + "/"�� �ٿ��� jsonutility�� ���� �����̸��� ������ �� �ִ�.


        //�̱��� �ν��Ͻ��� ���� �Ҵ���� �ʾѴٸ� ���� �ν��Ͻ� �Ҵ�
        if(Instance == null )
        {
            Instance = this;
        }
        //�̹� �ٸ� �ν��Ͻ��� �����Ѵٸ�, �ߺ��� �����ϱ� ���� ���� �ν��Ͻ��� ���� ������Ʈ�� �ı�
        else if(Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        DontDestroyOnLoad(Instance);
    }


    void Start()
    {

    }

    //���� �÷��̾��� ������ ���̽����� �ٲٰ� ����
    public void SaveData()
    {
        string data = JsonUtility.ToJson(nowPlayer); // json�� string�̱� ������ ���ڿ��� ���� �� ���� 
        File.WriteAllText(path + nowSlot.ToString(), data);   //�����ϰ��� �ϴ� ��ο� +�� �ٿ��� ������ �̸��� ������ �� �ִ�.
        // + nowSlot�� �ޱ� ������ �̸��� ���ٰ� �ϴ��� �ڿ� 0, 1, 2 �� �ٱ⿡ ������ �� �ִ�.
    }

    public void LoadData()
    {
        string data = File.ReadAllText(path + nowSlot.ToString()); // ��ο� �ҷ��� ������ �̸��� �Ű��� �����͸� �����´�.
        nowPlayer = JsonUtility.FromJson<PlayerData>(data);
    }


    public void DataClear()
    {
        nowSlot = -1; // ���� ��ȣ�� -�� �� �� �����Ƿ� -�� �ʱ�ȭ
        nowPlayer = new PlayerData(); //�÷��̾� ���� �ʱ�ȭ
    }
}
