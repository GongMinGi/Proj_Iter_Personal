using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{

    public static PlayerHealth instance;

    public int maxHealth;
    private int health;

    public event Action DamageTaken;
    public event Action HealthUpgraded;
    public event Action PlayerDied;

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

        }
        else
        {

            Destroy(gameObject);

        }

    }

    private void Start()
    {

        if (GameManager.instance != null)
        {

            GameManager.instance.LoadPlayerData(out health, out maxHealth);

        }
        else
        {

            health = maxHealth; 

        }

    }

    public void TakeDamage(int damage, Vector2 enemyDirection)
    {

        if (health <= 0)
            return;

        health -= damage;

        OnDamaged(enemyDirection);

        DamageTaken?.Invoke(); 

        if (health <= 0)
        {

            Die();

        }
    }

    private void OnDamaged(Vector2 enemyPos)
    {
        transform.parent.gameObject.layer = 7;

        GetComponentInParent<SpriteRenderer>().color = new Color(1, 1, 1, 0.4f);

        int dirc = transform.parent.transform.position.x - enemyPos.x >0 ? 1 : -1;
        GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);


        Invoke("OffDamaged", 3);
    }


    private void OffDamaged()
    {
        transform.parent.gameObject.layer = 0;
        GetComponentInParent<SpriteRenderer>().color = new Color(1, 1, 1, 1);


    }

    public void Heal(int healAmount)
    {

        if (health >= maxHealth)
            return;

        health += healAmount;

        if (health > maxHealth)
        {

            health = maxHealth;

        }

        DamageTaken?.Invoke();

    }

    public void UpgradeHealth()
    {

        maxHealth++;
        health = maxHealth;

        HealthUpgraded?.Invoke(); // Trigger HealthUpgraded event

    }

    private void Die()
    {

        PlayerDied?.Invoke(); // Trigger PlayerDied event

    }

    private void OnDestroy()
    {
        
        if (GameManager.instance != null)
        {

            GameManager.instance.SavePlayerData(health, maxHealth);

        }

    }

}
