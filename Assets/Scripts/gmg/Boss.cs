using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{

    public Slider healthBar; // ������ ü�¹� (�����̴�UI)
    public int maxHealth = 30; //���� �ִ� ü��
    public int currentHealth; // ���� ���� ü��
    [SerializeField]
    public BoxCollider2D bossRoomRange;
    Animator anim;
    AnimatorStateInfo stateInfo;

    void Start()
    {
        currentHealth = maxHealth; // ���� ���� �� ������ ü���� �ִ밪���� ����
        healthBar.maxValue = maxHealth; // ü�¹��� �ִ밪 ����
        healthBar.value = currentHealth; // ü�¹� �ʱⰪ ����
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // ���� ���°� 2������� �Ѿ�� ���¶�� ���� ���� �ο�
        if(stateInfo.IsName("Phase2")) 
        {
            return;
        }


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
        //if(currentHealth == maxHealth/2 )
        //{
        //    anim.SetTrigger("PhaseTwo");
        //}
    }
}
