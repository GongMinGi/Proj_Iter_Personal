using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    // 플레이어 상태 데이터
    public int maxHealth;
    public int health;
    public string nowScene;
    public Vector3 playerPos;

    void Awake()
    {
        // 싱글톤 패턴 구현
        if (instance == null)
        {

            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 유지

        }
        else
        {

            Destroy(gameObject); // 중복된 GameManager 제거

        }

    }




    // 플레이어 데이터 저장
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

    // 플레이어 데이터 로드
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