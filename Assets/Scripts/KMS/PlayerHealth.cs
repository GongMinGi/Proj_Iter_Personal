using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;

    public int maxHealth;
    private int health;

    public event Action DamageTaken;
    public event Action HealthUpgraded;
    public event Action PlayerDied; // 사망 이벤트 추가

    public int Health
    {
        get
        {
            return health;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 객체 유지
        }
        else
        {
            Destroy(gameObject); // 중복 방지
        }
    }

    private void Start()
    {
        LoadHealth(); // 저장된 체력 데이터 복원

        if (health == 0) // 데이터가 없으면 초기화
        {
            health = maxHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        if (health <= 0)
            return;

        health -= damage;

        DamageTaken?.Invoke(); // DamageTaken 이벤트 호출

        if (health <= 0)
        {
            Debug.Log("Die 호출");
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        if (health >= maxHealth)
            return;

        health += healAmount;

        if (health > maxHealth)
        {
            health = maxHealth; // 최대 체력을 초과하지 않도록
        }

        DamageTaken?.Invoke(); // DamageTaken 이벤트 호출
    }

    public void UpgradeHealth()
    {
        maxHealth++;
        health = maxHealth;

        HealthUpgraded?.Invoke(); // HealthUpgraded 이벤트 호출
    }

    public void SaveHealth()
    {
        PlayerPrefs.SetInt("CurrentHealth", health);
        PlayerPrefs.SetInt("MaxHealth", maxHealth);
        PlayerPrefs.Save();
    }

    public void LoadHealth()
    {
        if (PlayerPrefs.HasKey("CurrentHealth"))
        {
            health = PlayerPrefs.GetInt("CurrentHealth");
            maxHealth = PlayerPrefs.GetInt("MaxHealth");
        }
    }

    private void Die()
    {
        Debug.Log("Player is dead");
        PlayerDied?.Invoke(); // 사망 이벤트 호출
        // 향후 추가 사망 처리 로직은 여기서 구현
    }
}
