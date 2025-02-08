using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{

    public Slider healthBar; // 보스의 체력바 (슬라이더UI)
    public int maxHealth = 30; //보스 최대 체력
    public int currentHealth; // 보스 현재 체력

    void Start()
    {
        currentHealth = maxHealth; // 게임 시작 시 보스의 체력을 최대값으로 설정
        healthBar.maxValue = maxHealth; // 체력바의 최대값 설정
        healthBar.value = currentHealth; // 체력바 초기값 설정
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // 공격을 받으면 보스 체력감소
        healthBar.value = currentHealth;  // 체력바 ui 업데이트

        if (currentHealth < 0)
        {
            Die(); // 체력이 0이 되면 사망 처리
        }
    }

    
    private void Die() //보스 사망 시 실행되는 함수
    {
        Debug.Log("Boss Defeated");
        Destroy(gameObject); // 보스 오브젝트 제거
    }


    void Update()
    {
        
    }
}
