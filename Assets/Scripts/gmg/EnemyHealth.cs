using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    //오브젝트가 데미지를 받을 수 있는지 여부
    [SerializeField] private bool damageable = true;
    
    // 오브젝트의 최대 체력
    [SerializeField] private int maxHealth;

    //데미지를 받은 후 무적시간
    [SerializeField] private float invulnerabilityTime = 0.2f;

    //오브젝트가 공격되었는지 여부
    private bool hit = false;

    //현재 체력
    private int currentHealth;
    [SerializeField] private float knockbackForce = 5f; //밀려나는 힘

    private Rigidbody2D rb;
    [SerializeField]

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;      //게임 시작 시 최대체력으로 설정
    }


    //데미지 처리 및 체력 감소
    public void Damage(int amount, Vector2 attackDirection )
    {
        if (!damageable || hit || currentHealth <= 0) return;

        hit = true;
        currentHealth -= amount;

        ApplyKnockback(attackDirection);
    

        if (currentHealth <= 0)
        {
            // 체력이 0 이하일때 오브젝트 비활성화
            gameObject.SetActive(false);
        }
        else
        {
            //무적 시간을 지나면 다시 데미지 받을 수 있게 설정
            StartCoroutine(TurnOffHit());
        }
    }

    private void ApplyKnockback(Vector2 direction)
    {
        direction.y = 0;
        if(rb != null)
        {
            rb.linearVelocity = Vector2.zero; //기존 속도 초기화
            rb.AddForce(direction.normalized * knockbackForce, ForceMode2D.Impulse);
            
        }
    }


    private IEnumerator TurnOffHit()
    {
        yield return new WaitForSeconds(invulnerabilityTime);
        hit = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
