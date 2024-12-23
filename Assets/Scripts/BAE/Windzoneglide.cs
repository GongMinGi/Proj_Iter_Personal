using UnityEngine;

public class WindZoneglide : MonoBehaviour
{
    [SerializeField]
    private float windForce = 5f; // 바람의 지속적인 힘
    [SerializeField]
    private float upperHeight = 3f; // WindZone 기준 상단 높이
    [SerializeField]
    private float lowerHeight = 0.5f; // WindZone 기준 하단 높이
    [SerializeField]
    private float horizontalRange = 2f; // WindZone 중심으로부터의 수평 범위
    [SerializeField]
    private float defaultGravityScale = 1f; // 플레이어의 기본 중력 값

    private Transform playerTransform;
    private Rigidbody2D playerRb;
    private PlayerControllerbyBae playerController;
    private bool playerInZone = false; // 플레이어가 WindZone 안에 있는지 여부

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerTransform = collision.transform;
            playerRb = collision.GetComponent<Rigidbody2D>();
            playerController = collision.GetComponent<PlayerControllerbyBae>();

            if (playerRb != null)
            {
                defaultGravityScale = playerRb.gravityScale; // 초기 중력값 저장
            }
            playerInZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = false;
            if (playerRb != null)
            {
                // 중력을 복원
                playerRb.gravityScale = defaultGravityScale;
            }
        }
    }

    [System.Obsolete]
    private void FixedUpdate()
    {
        if (!playerInZone || playerTransform == null || playerRb == null || playerController == null) return;

        // 플레이어의 위치 및 WindZone 범위 계산
        float playerHeight = playerTransform.position.y;
        float playerHorizontalDistance = Mathf.Abs(playerTransform.position.x - transform.position.x);
        float zoneBottom = transform.position.y + lowerHeight;
        float zoneTop = transform.position.y + upperHeight;

        // 글라이딩 중이고, WindZone 범위 내에 있는 경우
        if (playerController.IsGliding &&
            playerHeight >= zoneBottom &&
            playerHeight <= zoneTop &&
            playerHorizontalDistance <= horizontalRange)
        {
            // 중력을 제거
            playerRb.gravityScale = 0;

            // 지속적인 바람의 힘 적용
            Vector2 windDirection = Vector2.up; // 항상 위쪽으로 힘을 가함
            playerRb.velocity = new Vector2(playerRb.velocity.x, windForce);
        }
        else
        {
            // WindZone을 벗어나거나 글라이딩을 중지하면 중력을 복원
            playerRb.gravityScale = defaultGravityScale;
        }
    }
}




/*using UnityEngine;

public class WindZone : MonoBehaviour
{
    [SerializeField]
    private float windForce = 5f; // 바람의 연속적인 힘
    [SerializeField]
    private float upperHeight = 3f; // 윈드 존 기준 상단 높이
    [SerializeField]
    private float lowerHeight = 0.5f; // 윈드 존 기준 하단 높이
    [SerializeField]
    private float horizontalRange = 2f; // 발판 중심으로부터의 수평 범위
    [SerializeField]
    private float defaultGravityScale = 1f; // 플레이어의 기본 중력 값

    private Transform playerTransform;
    private Rigidbody2D playerRb;
    private PlayerControllerbyBae playerController;
    private bool playerInZone = false; // 플레이어가 WindZone 안에 있는지 여부

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerTransform = collision.transform;
            playerRb = collision.GetComponent<Rigidbody2D>();
            playerController = collision.GetComponent<PlayerControllerbyBae>();

            if (playerRb != null)
            {
                defaultGravityScale = playerRb.gravityScale; // 초기 중력값 저장
            }
            playerInZone = true;
        }
    }

    [System.Obsolete]
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = false;
            if (playerRb != null)
            {
                // 중력과 속도 복원
                playerRb.gravityScale = defaultGravityScale;
                playerRb.velocity = new Vector2(playerRb.velocity.x, playerRb.velocity.y);
            }
        }
    }

    [System.Obsolete]
    private void FixedUpdate()
    {
        if (!playerInZone || playerTransform == null || playerRb == null || playerController == null) return;

        // 발판 기준 높이와 수평 거리 계산
        float playerHeight = playerTransform.position.y;
        float playerHorizontalDistance = Mathf.Abs(playerTransform.position.x - transform.position.x);
        float zoneBottom = transform.position.y + lowerHeight;
        float zoneTop = transform.position.y + upperHeight;

        // 조건 확인: 글라이딩 중이며 플레이어가 높이와 수평 범위 내에 있을 때
        if (playerController.IsGliding && playerHeight <= zoneTop && playerHorizontalDistance <= horizontalRange)
        {
            // 중력을 제거
            playerRb.gravityScale = 0;

            // 위쪽 방향으로 지속적으로 힘을 가함
            Vector2 windDirection = (Vector2)transform.up.normalized;
            playerRb.AddForce(windDirection * windForce, ForceMode2D.Force);

            // 상단(upperHeight)에 도달하면 속도를 제한
            if (playerHeight >= zoneTop)
            {
                playerRb.velocity = new Vector2(playerRb.velocity.x, Mathf.Min(playerRb.velocity.y, 0));
            }
        }
        else
        {
            // 영역을 벗어나거나 글라이딩을 중지했을 경우 중력 복원
            playerRb.gravityScale = defaultGravityScale;
            playerRb.velocity = new Vector2(playerRb.velocity.x, playerRb.velocity.y);
        }
    }
}




/*using UnityEngine;

public class WindZone : MonoBehaviour
{
    [SerializeField]
    private float constantWindSpeed = 5f; // 바람의 일정한 속도
    [SerializeField]
    private float upperHeight = 3f; // 윈드 존 기준 상단 높이
    [SerializeField]
    private float lowerHeight = 0.5f; // 윈드 존 기준 하단 높이
    [SerializeField]
    private float horizontalRange = 2f; // 발판 중심으로부터의 수평 범위
    [SerializeField]
    private float defaultGravityScale = 1f; // 플레이어의 기본 중력 값

    private Transform playerTransform;
    private Rigidbody2D playerRb;
    private PlayerControllerbyBae playerController;
    private bool playerInZone = false; // 플레이어가 WindZone 안에 있는지 여부

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어가 WindZone에 들어왔을 때
        if (collision.CompareTag("Player"))
        {
            playerTransform = collision.transform;
            playerRb = collision.GetComponent<Rigidbody2D>();
            playerController = collision.GetComponent<PlayerControllerbyBae>();

            if (playerRb != null)
            {
                defaultGravityScale = playerRb.gravityScale; // 초기 중력값 저장
            }
            playerInZone = true;
        }
    }

    [System.Obsolete]
    private void OnTriggerExit2D(Collider2D collision)
    {
        // 플레이어가 WindZone을 나갔을 때
        if (collision.CompareTag("Player"))
        {
            playerInZone = false;
            if (playerRb != null)
            {
                // 중력과 속도 복원
                playerRb.gravityScale = defaultGravityScale;
                playerRb.velocity = new Vector2(playerRb.velocity.x, playerRb.velocity.y);
            }
        }
    }

    [System.Obsolete]
    private void FixedUpdate()
    {
        if (!playerInZone || playerTransform == null || playerRb == null || playerController == null) return;

        // 발판 기준 높이와 수평 거리 계산
        float playerHeight = playerTransform.position.y;
        float playerHorizontalDistance = Mathf.Abs(playerTransform.position.x - transform.position.x);
        float zoneBottom = transform.position.y + lowerHeight;
        float zoneTop = transform.position.y + upperHeight;

        // 조건 확인: 플레이어가 높이 범위와 수평 범위 내에 있고 글라이딩 중인지
        if (playerHeight >= zoneBottom && playerHeight <= zoneTop &&
            playerHorizontalDistance <= horizontalRange && playerController.IsGliding)
        {
            // 중력을 제거
            playerRb.gravityScale = 0;

            // 초록색 축(로컬 Y축) 방향으로 바람 적용
            Vector2 windDirection = (Vector2)transform.up.normalized;

            // 등속 운동 적용
            playerRb.velocity = windDirection * constantWindSpeed;
        }
        else
        {
            // 영역을 벗어나면 중력을 원래 값으로 복원
            playerRb.gravityScale = defaultGravityScale;
            playerRb.velocity = new Vector2(playerRb.velocity.x, playerRb.velocity.y);
        }
    }
}


*/

