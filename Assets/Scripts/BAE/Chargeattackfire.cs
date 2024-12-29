using UnityEngine;

public class Chargeattackfire : MonoBehaviour
{
    public Animator animator; // Animator 컴포넌트 참조

    void Update()
    {
        // 마우스 버튼을 놓았는지 확인
        if (Input.GetMouseButtonUp(0)) // 왼쪽 마우스 버튼
        {
            // Animator 파라미터를 Trigger로 설정
            animator.SetTrigger("Chargeattackfire");
        }
    }
}
