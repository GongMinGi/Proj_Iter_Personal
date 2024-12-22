using UnityEngine;

public class WindZone : MonoBehaviour
{
    [SerializeField]
    private float windForce = 10f; // 상승 힘
    [SerializeField]
    private float upperHeight = 3f; // 윈드 존 기준 상단 높이
    [SerializeField]
    private float lowerHeight = 0.5f; // 윈드 존 기준 하단 높이 (발판 바로 위)
    [SerializeField]
    private float horizontalRange = 2f; // 발판 중심으로부터의 수평 범위

    private Transform playerTransform;
    private Rigidbody2D playerRb;
    private PlayerController playerController;

    [System.Obsolete]
    private void Update()
    {
        // 플레이어 오브젝트 찾기
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerRb = player.GetComponent<Rigidbody2D>();
                playerController = player.GetComponent<PlayerController>();
            }
        }

        // 플레이어가 없거나 컴포넌트가 누락된 경우, 실행 중단
        if (playerTransform == null || playerRb == null || playerController == null) return;

        // 발판 기준 높이와 거리 계산
        float playerHeight = playerTransform.position.y;
        float playerHorizontalDistance = Mathf.Abs(playerTransform.position.x - transform.position.x);
        float zoneBottom = transform.position.y + lowerHeight;
        float zoneTop = transform.position.y + upperHeight;

        // 조건 확인: 높이 범위, 수평 범위 내에 있고 글라이딩 중인지
        if (playerHeight >= zoneBottom && playerHeight <= zoneTop &&
            playerHorizontalDistance <= horizontalRange && playerController.IsGliding)
        {
            // 상승 효과 적용
            playerRb.velocity = new Vector2(playerRb.velocity.x, windForce);
        }
    }
}


/*
 * using UnityEngine;

public class WindZone : MonoBehaviour
{
    [SerializeField]
    private float windForce = 10f; // 상승 힘
    [SerializeField]
    private float upperHeight = 3f; // 윈드 존 기준 상단 높이
    [SerializeField]
    private float lowerHeight = 0.5f; // 윈드 존 기준 하단 높이 (발판 바로 위)

    private Transform playerTransform;
    private Rigidbody2D playerRb;
    private PlayerController playerController;

    private void Update()
    {
        // 플레이어 오브젝트 찾기
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerRb = player.GetComponent<Rigidbody2D>();
                playerController = player.GetComponent<PlayerController>();
            }
        }

        // 플레이어가 없거나 컴포넌트가 누락된 경우, 실행 중단
        if (playerTransform == null || playerRb == null || playerController == null) return;

        // 발판 기준 높이 계산
        float playerHeight = playerTransform.position.y;
        float zoneBottom = transform.position.y + lowerHeight;
        float zoneTop = transform.position.y + upperHeight;

        // 조건 확인: 높이 범위 내에 있고 글라이딩 중인지
        if (playerHeight >= zoneBottom && playerHeight <= zoneTop && playerController.IsGliding)
        {
            // 상승 효과 적용
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, windForce);
        }
    }
}

*/


/*using UnityEngine;

public class WindZone : MonoBehaviour
{
    [SerializeField]
    private float windForce = 10f; // 바람 상승 힘
    [SerializeField]
    private float upperHeight = 3f; // 윈드 존 기준 활성화 영역의 상한 높이

    private void Update()
    {
        // 디버그 라인으로 활성화 영역 시각화
        Debug.DrawLine(transform.position + Vector3.up * upperHeight, transform.position, Color.blue);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Player 태그 확인
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();

            if (playerController != null && playerRb != null)
            {
                // 글라이딩 중인지 확인
                if (playerController.IsGliding)
                {
                    float playerHeight = collision.transform.position.y;
                    float zoneTop = transform.position.y + upperHeight;
                    float zoneBottom = transform.position.y;

                    // 플레이어가 발판 기준 특정 높이와 아래 사이에 있는지 확인
                    if (playerHeight <= zoneTop && playerHeight >= zoneBottom)
                    {
                        // 상승 효과 적용
                        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, windForce);
                    }
                }
            }
        }
    }
}
*/