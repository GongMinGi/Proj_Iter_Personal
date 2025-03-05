using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    // �÷��̾� ���� ������
    public int maxHealth;
    public int health;
    public string nowScene;
    public Vector3 playerPos;

    void Awake()
    {
        // �̱��� ���� ����
        if (instance == null)
        {

            instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� ����

        }
        else
        {

            Destroy(gameObject); // �ߺ��� GameManager ����

        }

    }




    // �÷��̾� ������ ����
    public void SavePlayerData(int currentHealth, int maxHealth, Vector3 position)
    {

        this.health = currentHealth;
        this.maxHealth = maxHealth;
        this.nowScene = SceneManager.GetActiveScene().name;
        this.playerPos = position;

        DataManager.Instance.nowPlayer.curHealth = this.health;
        DataManager.Instance.nowPlayer.maxHealth = this.maxHealth;
        DataManager.Instance.nowPlayer.playerPos = this.playerPos;
        DataManager.Instance.nowPlayer.nowScene = this.nowScene;

        DataManager.Instance.SaveData();

    }

    // �÷��̾� ������ �ε�
    public void LoadPlayerData(out int currentHealth, out int maxHealth)
    {

        //currentHealth = this.health;
        //maxHealth = this.maxHealth;

        currentHealth = DataManager.Instance.nowPlayer.curHealth;
        maxHealth = DataManager.Instance.nowPlayer.maxHealth ;
        if(DataManager.Instance.nowPlayer.playerPos != null)
        {
            playerPos = DataManager.Instance.nowPlayer.playerPos;
        }

    }

}