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
    Animator anim;
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


    /*
     * out 파라미터
     *  - 출력 매개변수, 빈 변수를 매개변수에 넣어주면 함수에서 그 변수에 값을 넣어준다.
     */
    private void Start()
    {
        //anim = GetComponentInParent<Animator>();
        anim = GetComponent<Animator>();

        if (GameManager.instance != null)
        {

            GameManager.instance.LoadPlayerData(out health, out maxHealth);
            transform.position = GameManager.instance.playerPos; // 씬이 시작할 때 플레이어의 위치를 저장데이터의 위치로 변경한다.

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
        
        //GetComponentInParent<PlayerController>().OnDamaged(enemyDirection);
        GetComponent<PlayerController>().OnDamaged(enemyDirection);


        DamageTaken?.Invoke();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Iamattattacked);
        if (health <= 0)
        {

            Die();

        }
    }

    //private void OnDamaged(Vector2 enemyPos)
    //{

    //    transform.parent.gameObject.layer = 7;

    //    GetComponentInParent<SpriteRenderer>().color = new Color(1, 1, 1, 0.4f);


    //    float dirc = enemyPos.x - transform.parent.transform.position.x > 0 ? -1 : 1;
    //    GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(dirc, 0.2f) * playerKnockbackForce, ForceMode2D.Impulse);


    //    Invoke("OffDamaged", 3);
    //}


    //private void OffDamaged()
    //{
    //    //Animator anim = GetComponentInParent<Animator>();
    //    anim.SetBool("onDamaged", false);

    //    transform.parent.gameObject.layer = 0;
    //    GetComponentInParent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        


    //}

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

            GameManager.instance.SavePlayerData(health, maxHealth, this.transform.position);
            //GameManager.instance.SavePlayerData(health, maxHealth);
        }

    }

    public void SaveData()
    {
        GameManager.instance.SavePlayerData(health, maxHealth, this.transform.position);

    }

}
