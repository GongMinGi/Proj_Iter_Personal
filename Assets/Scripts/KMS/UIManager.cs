using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{

    public GameObject heart;

    public List<Image> hearts;

    PlayerHealth playerHealth;

    void Start()
    {

        playerHealth = PlayerHealth.instance;
        playerHealth.DamageTaken += UpdateHearts;
        playerHealth.HealthUpgraded += AddHearts;

        for (int i = 0; i < playerHealth.maxHealth; i++)
        {

            GameObject h = Instantiate(heart, this.transform);
            hearts.Add(h.GetComponent<Image>());

        }

    }

    void UpdateHearts()
    {

        int heartFill = playerHealth.Health;

        foreach (Image i in hearts)
        {

            i.fillAmount = heartFill;
            heartFill -= 1;

        }

    }

    void AddHearts()
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

    }

}
