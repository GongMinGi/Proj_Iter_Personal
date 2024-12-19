using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{

    public GameObject heart;
    public List<Image> hearts;

    private PlayerHealth playerHealth;

    void Start()
    {

        playerHealth = PlayerHealth.instance;
        playerHealth.DamageTaken += UpdateHearts;
        playerHealth.HealthUpgraded += AddHearts;

        InitializeHearts();     // 초기 하트 생성 및 UI 반영

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

            GameObject h = Instantiate(heart, this.transform);

            hearts.Add(h.GetComponent<Image>());

        }

        UpdateHearts();     // 현재 체력 반영

    }

    void UpdateHearts()
    {

        int heartFill = playerHealth.Health;

        foreach (Image i in hearts)
        {

            i.fillAmount = Mathf.Clamp01((float)heartFill);     // 0과 1 사이로 제한

            heartFill -= 1;

        }

    }

    void AddHearts()
    {

        InitializeHearts(); // 최대 체력 증가 시 하트 갱신

    }

    void OnDestroy()
    {

        if (playerHealth != null)
        {

            playerHealth.DamageTaken -= UpdateHearts;
            playerHealth.HealthUpgraded -= AddHearts;

        }

    }

}