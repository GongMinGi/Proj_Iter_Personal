using UnityEngine;

public class WindZoneglide : MonoBehaviour
{
    [SerializeField] private float windForce = 6f; // 바람의 지속적인 힘
    [SerializeField] private float upperHeight = 3f; // WindZone 기준 상단 높이
    [SerializeField] private float lowerHeight = 0.5f; // WindZone 기준 하단 높이
    [SerializeField] private float horizontalRange = 2f; // WindZone 중심으로부터의 수평 범위
    [SerializeField] private float defaultGravityScale = 1f; // 플레이어의 기본 중력 값

    private Transform playerTransform;
    private Rigidbody2D playerRb;
    private PlayerController playerController;
    private bool playerInZone = false; // 플레이어가 WindZone 안에 있는지 여부

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerTransform = collision.transform;
            playerRb = collision.GetComponent<Rigidbody2D>();
            playerController = collision.GetComponent<PlayerController>();

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

    private void FixedUpdate()
    {
        if (!playerInZone || playerTransform == null || playerRb == null || playerController == null) return;

        float playerHeight = playerTransform.position.y;
        float playerHorizontalDistance = Mathf.Abs(playerTransform.position.x - transform.position.x);
        float zoneBottom = transform.position.y + lowerHeight;
        float zoneTop = transform.position.y + upperHeight;

        if (playerController.isGliding &&
            playerHeight >= zoneBottom &&
            playerHeight <= zoneTop &&
            playerHorizontalDistance <= horizontalRange)
        {
            ApplyWindEffect();
        }
        else
        {
            ResetGravity();
        }
    }

    private void ApplyWindEffect()
    {
        playerRb.gravityScale = 0;

        float playerHeight = playerTransform.position.y;
        float zoneBottom = transform.position.y + lowerHeight;
        float zoneTop = transform.position.y + upperHeight;

        float normalizedHeight = (playerHeight - zoneBottom) / (zoneTop - zoneBottom);
        float dynamicWindForce = Mathf.Lerp(1f, windForce, normalizedHeight);

        Vector2 windDirection = transform.up.normalized;

        // 매 프레임 위치 업데이트
        Vector2 newPosition = playerRb.position + windDirection * dynamicWindForce * Time.fixedDeltaTime;
        playerRb.MovePosition(newPosition);

        // 디버그 로그
        Debug.Log($"WindZone 적용 중: {newPosition}");
    }

    private void ResetGravity()
    {
        if (playerRb != null)
        {
            playerRb.gravityScale = defaultGravityScale;
        }
    }
}




/*
using UnityEngine;

public class WindZoneglide : MonoBehaviour
{
    [SerializeField]
    private float windForce = 6f; // 바람의 지속적인 힘
    [SerializeField]
    private float upperHeight = 3f; // WindZone 기준 상단 높이G
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

    private void FixedUpdate()
    {
        if (!playerInZone || playerTransform == null || playerRb == null || playerController == null) return;

        float playerHeight = playerTransform.position.y;
        float playerHorizontalDistance = Mathf.Abs(playerTransform.position.x - transform.position.x);
        float zoneBottom = transform.position.y + lowerHeight;
        float zoneTop = transform.position.y + upperHeight;

        if (playerController.IsGliding &&
            playerHeight >= zoneBottom &&
            playerHeight <= zoneTop &&
            playerHorizontalDistance <= horizontalRange)
        {
            playerRb.gravityScale = 0;

            float normalizedHeight = (playerHeight - zoneBottom) / (zoneTop - zoneBottom);
            float dynamicWindForce = Mathf.Lerp(1f, windForce, normalizedHeight);

            Vector2 windDirection = transform.up.normalized;

            // 매 프레임 위치 업데이트
            Vector2 newPosition = playerRb.position + windDirection * dynamicWindForce * Time.fixedDeltaTime;
            playerRb.MovePosition(newPosition);

            Debug.Log($"WindZone 적용 중: {newPosition}");
        }
        else
        {
            playerRb.gravityScale = defaultGravityScale;
        }
    }

}
*/






