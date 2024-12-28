using UnityEngine;

public class MouseInput : MonoBehaviour
{
    public Transform playerTransform; // 플레이어의 Transform
    public Transform firePoint; // 발사체가 생성될 FirePoint 위치
    public GameObject boomClone; // 폭탄 프리팹
    public float bombSpeed = 50f; // 폭탄의 속도
    public float chargeTime = 1f; // 발사에 필요한 최소 충전 시간

    private float chargeCounter = 0f; // 마우스 버튼 누른 시간
    private SpriteRenderer spriteRenderer; // 플레이어의 SpriteRenderer

    private void Awake()
    {
        // SpriteRenderer 가져오기
        spriteRenderer = playerTransform.GetComponent<SpriteRenderer>();
    }

    private void OnDrawGizmos()
    {
        // FirePoint 위치 시각화
        if (firePoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(firePoint.position, 0.1f);
        }
    }

    [System.Obsolete]
    private void Update()
    {
        // FirePoint 위치 동기화
        UpdateFirePointPosition();

        // 마우스 버튼이 눌린 상태라면 시간 누적
        if (Input.GetMouseButton(0)) // 마우스 우클릭
        {
            chargeCounter += Time.deltaTime; // 누른 시간 누적
        }

        // 마우스 버튼을 뗄 때 발사 조건 확인
        if (Input.GetMouseButtonUp(0)) // 마우스 버튼에서 손을 뗌
        {
            if (chargeCounter >= chargeTime) // 충전 시간이 조건 충족
            {
                FireProjectile();
            }
            chargeCounter = 0f; // 충전 시간 초기화
        }
    }

    private void UpdateFirePointPosition()
    {
        if (spriteRenderer.flipX)
        {
            firePoint.localPosition = new Vector3(-Mathf.Abs(firePoint.localPosition.x), firePoint.localPosition.y, firePoint.localPosition.z);
        }
        else
        {
            firePoint.localPosition = new Vector3(Mathf.Abs(firePoint.localPosition.x), firePoint.localPosition.y, firePoint.localPosition.z);
        }
    }

    [System.Obsolete]
    private void FireProjectile()
    {
        // 발사 전에 FirePoint 위치 동기화
        UpdateFirePointPosition();

        // 플레이어가 보고 있는 방향 확인
        bool isFacingRight = !spriteRenderer.flipX;
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;

        // 폭탄 생성
        GameObject bomb = Instantiate(boomClone, firePoint.position, Quaternion.identity);

        // Rigidbody2D에 속도 설정
        Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * bombSpeed;
        }
    }
}



