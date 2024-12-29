
using UnityEngine;

public class WindZoneglide : MonoBehaviour
{
    [SerializeField]
    private float windForce = 6f; // 바람의 지속적인 힘
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













/*
using UnityEngine;

public class WindZoneglide : MonoBehaviour
{
    [SerializeField]
    private float windForce = 6f; // 바람의 지속적인 힘
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

    private void FixedUpdate()
    {
        if (!playerInZone || playerTransform == null || playerRb == null || playerController == null) return;

        // 플레이어의 위치 및 WindZone 범위 계산
        float playerHeight = playerTransform.position.y;
        float playerHorizontalDistance = Mathf.Abs(playerTransform.position.x - transform.position.x);
        float zoneBottom = transform.position.y + lowerHeight;
        float zoneTop = transform.position.y + upperHeight;

        // WindZone 방향 시각화 (디버깅용)
        Debug.DrawLine(transform.position, transform.position + transform.up * 2, Color.red);

        // 글라이딩 중이고, WindZone 범위 내에 있는 경우
        if (playerController.IsGliding &&
            playerHeight >= zoneBottom &&
            playerHeight <= zoneTop &&
            playerHorizontalDistance <= horizontalRange)
        {
            // 중력을 제거
            playerRb.gravityScale = 0;

            // 바람의 힘을 높이에 따라 선형적으로 증가시키기
            float normalizedHeight = (playerHeight - zoneBottom) / (zoneTop - zoneBottom); // 0 ~ 1 사이 값
            float dynamicWindForce = Mathf.Lerp(1f, windForce, normalizedHeight); // 낮은 높이에서는 약한 힘, 높은 높이에서는 강한 힘

            // WindZone의 로컬 Y축 방향 계산
            Vector2 windDirection = transform.up.normalized;

            // 위치 업데이트: MovePosition 사용
            Vector2 newPosition = playerRb.position + windDirection * dynamicWindForce * Time.fixedDeltaTime;
            playerRb.MovePosition(newPosition);
        }
        else
        {
            // WindZone을 벗어나거나 글라이딩을 중지하면 중력을 복원
            playerRb.gravityScale = defaultGravityScale;
        }
    }
}

*/



/*
using UnityEngine;

public class WindZoneglide : MonoBehaviour
{
    [SerializeField]
    private float windForce = 6f; // 바람의 지속적인 힘
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

            // 바람의 힘을 높이에 따라 선형적으로 증가시키기
            float normalizedHeight = (playerHeight - zoneBottom) / (zoneTop - zoneBottom); // 0 ~ 1 사이 값
            float dynamicWindForce = Mathf.Lerp(1f, windForce, normalizedHeight); // 낮은 높이에서는 약한 힘, 높은 높이에서는 강한 힘

            // WindZone의 로컬 Y축 방향 계산
            Vector2 windDirection = transform.up.normalized; // WindZone의 로컬 Y축 방향
            Debug.Log($"Wind Direction: {windDirection}");

            // 지속적인 바람의 힘 적용 (AddForce로 변경)
            playerRb.AddForce(windDirection * dynamicWindForce, ForceMode2D.Force);
        }
        else
        {
            // WindZone을 벗어나거나 글라이딩을 중지하면 중력을 복원
            playerRb.gravityScale = defaultGravityScale;
        }
    }
}


*/