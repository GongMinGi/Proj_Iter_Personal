using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{

    public Slider healthBar; // ������ ü�¹� (�����̴�UI)
    public int maxHealth = 30; //���� �ִ� ü��
    public int currentHealth; // ���� ���� ü��

    void Start()
    {
        currentHealth = maxHealth; // ���� ���� �� ������ ü���� �ִ밪���� ����
        healthBar.maxValue = maxHealth; // ü�¹��� �ִ밪 ����
        healthBar.value = currentHealth; // ü�¹� �ʱⰪ ����
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // ������ ������ ���� ü�°���
        healthBar.value = currentHealth;  // ü�¹� ui ������Ʈ

        if (currentHealth < 0)
        {
            Die(); // ü���� 0�� �Ǹ� ��� ó��
        }
    }

    
    private void Die() //���� ��� �� ����Ǵ� �Լ�
    {
        Debug.Log("Boss Defeated");
        Destroy(gameObject); // ���� ������Ʈ ����
    }


    void Update()
    {
        
    }
}
