using UnityEngine;

public class MouseInput : MonoBehaviour
{
    public Transform playerTransform; // 플레이어의 Transform
    public GameObject boomClone; // 폭탄 프리팹
    public float bombSpeed = 5f; // 폭탄의 속도

    private void OnDrawGizmos()
    {
        // 디버깅을 위해 마우스 위치를 시각화
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.2f);
    }

    [System.Obsolete]
    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // 마우스 우클릭
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // 2D 좌표에서 Z 축 제거

            // 폭탄 생성
            GameObject bomb = Instantiate(boomClone, playerTransform.position, Quaternion.identity);

            // 폭탄의 방향 설정
            Vector2 direction = (mousePosition - playerTransform.position).normalized;

            // 폭탄에 Rigidbody2D 추가 후 속도 설정
            Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * bombSpeed;
            }
        }
    }
}




/*using UnityEngine;

public class MouseInput : MonoBehaviour
{
    Vector3 MousePosition;
    public LayerMask whatisPlatform;
    public GameObject boomClone;
    private void OnDrawGizmos ()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere (MousePosition, 0.2f);
    }
    void Update()
    {
       if (Input.GetMouseButtonDown (1)) 
                    {
            MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Instantiate(boomClone, MousePosition, Quaternion.identity);

        }
    }
}
*/