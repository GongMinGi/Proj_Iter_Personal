using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class UIManager : MonoBehaviour
{

    public GameObject heart;
    public List<Image> hearts;

    private PlayerHealth playerHealth;

    public GameObject gameOverPanel;    // 게임 오버 화면

    void Start()
    {

        gameOverPanel.SetActive(false);

        playerHealth = PlayerHealth.instance;

        playerHealth.DamageTaken += UpdateHearts;
        playerHealth.HealthUpgraded += AddHearts;
//        playerHealth.PlayerDied += ShowGameOverScreen;  // 사망 이벤트 구독

        StartCoroutine(DelayedInitialize());    // 초기 하트 생성 및 UI 반영
    }

    IEnumerator DelayedInitialize()
    {

        yield return null;  // 한 프레임 대기하여 PlayerHealth 초기화 보장

        InitializeHearts();

    }

    void InitializeHearts()
    {

        foreach (Image i in hearts)
        {

            Destroy(i.gameObject);

        }

        hearts.Clear();

        for (int i = 0; i < playerHealth.maxHealth; i++)
        {

            GameObject h = Instantiate(heart, transform);

            hearts.Add(h.GetComponent<Image>());

        }

        UpdateHearts(); // 현재 체력 반영

    }

    void UpdateHearts()
    {

        int heartFill = playerHealth.Health;

        foreach (Image i in hearts)
        {

            i.fillAmount = Mathf.Clamp01((float)heartFill); // 0과 1 사이로 제한
            heartFill -= 1;

        }

    }

    void AddHearts()
    {

        InitializeHearts(); // 최대 체력 증가 시 하트 갱신

    }

    //void ShowGameOverScreen()
    //{

    //    Debug.Log("Game Over!");

    //    if (gameOverPanel != null)
    //    {

    //        gameOverPanel.SetActive(true); // 게임 오버 패널 활성화
    //    }

    //    Time.timeScale = 0f;

    //}

    void OnDestroy()
    {

        if (playerHealth != null)
        {

            playerHealth.DamageTaken -= UpdateHearts;
            playerHealth.HealthUpgraded -= AddHearts;
 //           playerHealth.PlayerDied -= ShowGameOverScreen; // 이벤트 해제

        }

    }

}