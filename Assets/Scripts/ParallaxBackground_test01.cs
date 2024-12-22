using UnityEngine;

public class ParallaxBackground_test01 : MonoBehaviour
{

    [SerializeField]
    private Transform target;   // 현재 배경과 이어지는 배경
    [SerializeField]
    private float scrollAmount; // 이어지는 두 배경 사이의 거리
    [SerializeField]
    private float scrollSpeed; // 이동속도
    [SerializeField]
    private Vector3 moveDirection; // 이동 방향
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //배경이 moveDirection 방향으로 moveSpeed 속도로 이동
        transform.position += moveDirection * scrollSpeed * Time.deltaTime;

        //배경이 설정된 범위를 벗어나면 위치 재설정
        if(transform.position.y <= -scrollAmount)
        {
            transform.position = target.position - moveDirection * scrollAmount;
        }
        
    }
}
