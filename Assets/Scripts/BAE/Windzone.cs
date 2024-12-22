using UnityEngine;


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

    [System.Obsolete]
    private void FixedUpdate()
    {
        // 플레이어 오브젝트 찾기
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerRb = player.GetComponent<Rigidbody2D>();
                playerController = player.GetComponent<PlayerControllerbyBae>();
                if (playerRb != null)
                {
                    defaultGravityScale = playerRb.gravityScale; // 초기 중력값 저장
                }
            }
        }

        // 플레이어가 없거나 컴포넌트가 누락된 경우 실행 중단
        if (playerTransform == null || playerRb == null || playerController == null) return;

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

            // 속도 초기화
            playerRb.velocity = new Vector2(0, playerRb.velocity.y);
        }
    }
}





/*using UnityEngine;

public class WindZone : MonoBehaviour
{
    [SerializeField]
    private float constantWindSpeed = 5f; // 일정한 이동 속도
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

    [System.Obsolete]
    private void FixedUpdate()
    {
        // 플레이어 오브젝트 찾기
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerRb = player.GetComponent<Rigidbody2D>();
                playerController = player.GetComponent<PlayerControllerbyBae>();
                if (playerRb != null)
                {
                    defaultGravityScale = playerRb.gravityScale; // 초기 중력값 저장
                }
            }
        }

        // 플레이어가 없거나 컴포넌트가 누락된 경우 실행 중단
        if (playerTransform == null || playerRb == null || playerController == null) return;

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

            // 윈드존 방향에 따른 등속 운동
            Vector2 windDirection = (Vector2)transform.up.normalized; // 윈드존의 방향
            playerRb.velocity = windDirection * constantWindSpeed;   // 방향에 따라 일정한 속도로 이동
        }
        else
        {
            // 영역을 벗어나면 중력을 원래 값으로 복원
            playerRb.gravityScale = defaultGravityScale;

            // 속도 초기화
            playerRb.velocity = new Vector2(0, playerRb.velocity.y);
        }
    }
}




/*using UnityEngine;

public class WindZone : MonoBehaviour
{
    [SerializeField]
    private float constantWindSpeed = 5f; // 일정한 상승 속도
    [SerializeField]
    private float windForce = 10f; // 프레임마다 적용되는 작은 힘
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

    [System.Obsolete]
    private void FixedUpdate()
    {
        // 플레이어 오브젝트 찾기
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerRb = player.GetComponent<Rigidbody2D>();
                playerController = player.GetComponent<PlayerControllerbyBae>();
                if (playerRb != null)
                {
                    defaultGravityScale = playerRb.gravityScale; // 초기 중력값 저장
                }
            }
        }

        // 플레이어가 없거나 컴포넌트가 누락된 경우 실행 중단
        if (playerTransform == null || playerRb == null || playerController == null) return;

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

            // 윈드존의 "위쪽 방향"으로 속도 적용 (등속 운동)
            Vector2 windDirection = transform.up.normalized; // 윈드존의 방향
            playerRb.velocity = windDirection * constantWindSpeed;

            // 선택: 바람의 힘을 추가로 더하고 싶다면
            // playerRb.AddForce(windDirection * windForce * Time.fixedDeltaTime, ForceMode2D.Force);
        }
        else
        {
            // 영역을 벗어나면 중력을 원래 값으로 복원
            playerRb.gravityScale = defaultGravityScale;
        }
    }
}




/*
 * using UnityEngine;

public class WindZone : MonoBehaviour
{
    [SerializeField]
    private float constantWindSpeed = 5f; // 일정한 상승 속도 (등속 운동 시)
    [SerializeField]
    private float windForce = 10f; // 프레임마다 적용되는 작은 힘
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

    [System.Obsolete]
    private void FixedUpdate()
    {
        // 플레이어 오브젝트 찾기
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerRb = player.GetComponent<Rigidbody2D>();
                playerController = player.GetComponent<PlayerControllerbyBae>();
                if (playerRb != null)
                {
                    defaultGravityScale = playerRb.gravityScale; // 초기 중력값 저장
                }
            }
        }

        // 플레이어가 없거나 컴포넌트가 누락된 경우 실행 중단
        if (playerTransform == null || playerRb == null || playerController == null) return;

        // 발판 기준 높이와 수평 거리 계산
        float playerHeight = playerTransform.position.y;
        float playerHorizontalDistance = Mathf.Abs(playerTransform.position.x - transform.position.x);
        float zoneBottom = transform.position.y + lowerHeight;
        float zoneTop = transform.position.y + upperHeight;

        // 조건 확인: 플레이어가 높이 범위와 수평 범위 내에 있고 글라이딩 중인지
        if (playerHeight >= zoneBottom && playerHeight <= zoneTop &&
            playerHorizontalDistance <= horizontalRange && playerController.IsGliding)
        {
            // 중력을 제거하고 힘 또는 등속 운동 적용
            playerRb.gravityScale = 0; // 중력 제거

            // 등속 운동 (매 프레임 일정한 속도로 상승)
            playerRb.velocity = new Vector2(0, constantWindSpeed);

            // 아래는 추가 힘을 주는 경우 (선택적으로 사용할 수 있음)
            // playerRb.AddForce(new Vector2(0, windForce * Time.fixedDeltaTime), ForceMode2D.Force);
        }
        else
        {
            // 영역을 벗어나면 중력을 원래 값으로 복원
            playerRb.gravityScale = defaultGravityScale;
        }
    }
}




/*using UnityEngine;

public class WindZone : MonoBehaviour
{
    [SerializeField]
    private float constantWindSpeed = 5f; // 일정한 상승 속도
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

    [System.Obsolete]
    private void FixedUpdate()
    {
        // 플레이어 오브젝트 찾기
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerRb = player.GetComponent<Rigidbody2D>();
                playerController = player.GetComponent<PlayerControllerbyBae>();
                if (playerRb != null)
                {
                    defaultGravityScale = playerRb.gravityScale; // 초기 중력값 저장
                }
            }
        }

        // 플레이어가 없거나 컴포넌트가 누락된 경우 실행 중단
        if (playerTransform == null || playerRb == null || playerController == null) return;

        // 발판 기준 높이와 수평 거리 계산
        float playerHeight = playerTransform.position.y;
        float playerHorizontalDistance = Mathf.Abs(playerTransform.position.x - transform.position.x);
        float zoneBottom = transform.position.y + lowerHeight;
        float zoneTop = transform.position.y + upperHeight;

        // 조건 확인: 플레이어가 높이 범위와 수평 범위 내에 있고 글라이딩 중인지
        if (playerHeight >= zoneBottom && playerHeight <= zoneTop &&
            playerHorizontalDistance <= horizontalRange && playerController.IsGliding)
        {
            // 중력을 제거하고 일정한 속도로 상승
            playerRb.gravityScale = 0; // 중력 제거

            // 등속 상승 속도 강제로 유지
            playerRb.velocity = new Vector2(0, constantWindSpeed);
        }
        else
        {
            // 영역을 벗어나면 중력을 원래 값으로 복원
            playerRb.gravityScale = defaultGravityScale;
        }
    }
}


/*
 * using System.Collections;
using UnityEngine;

public class WindZone : MonoBehaviour
{
    [SerializeField]
    private float maxWindForce = 15f; // 바람의 최대 힘
    [SerializeField]
    private float windAccelerationTime = 2f; // 바람의 힘이 최대에 도달하는 시간 (초 단위)
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
    private Coroutine windCoroutine;

    private void FixedUpdate()
    {
        // 플레이어 오브젝트 찾기
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerRb = player.GetComponent<Rigidbody2D>();
                playerController = player.GetComponent<PlayerControllerbyBae>();
                if (playerRb != null)
                {
                    defaultGravityScale = playerRb.gravityScale; // 초기 중력값 저장
                }
            }
        }

        // 플레이어가 없거나 컴포넌트가 누락된 경우 실행 중단
        if (playerTransform == null || playerRb == null || playerController == null) return;

        // 발판 기준 높이와 수평 거리 계산
        float playerHeight = playerTransform.position.y;
        float playerHorizontalDistance = Mathf.Abs(playerTransform.position.x - transform.position.x);
        float zoneBottom = transform.position.y + lowerHeight;
        float zoneTop = transform.position.y + upperHeight;

        // 조건 확인: 플레이어가 높이 범위와 수평 범위 내에 있고 글라이딩 중인지
        if (playerHeight >= zoneBottom && playerHeight <= zoneTop &&
            playerHorizontalDistance <= horizontalRange && playerController.IsGliding)
        {
            // 코루틴 시작 (중복 방지)
            if (windCoroutine == null)
            {
                windCoroutine = StartCoroutine(ApplyWindForce());
            }
        }
        else
        {
            // 코루틴 중단 및 상태 복구
            if (windCoroutine != null)
            {
                StopCoroutine(windCoroutine);
                windCoroutine = null;
                playerRb.gravityScale = defaultGravityScale; // 중력 복구
            }
        }
    }

    private IEnumerator ApplyWindForce()
    {
        playerRb.gravityScale = 0; // 중력 제거
        float currentWindForce = 0f; // 초기 바람의 힘
        float elapsedTime = 0f; // 경과 시간

        // 바람의 힘을 비선형적으로 증가
        while (elapsedTime < windAccelerationTime)
        {
            elapsedTime += Time.fixedDeltaTime; // 경과 시간 증가

            // 비선형 곡선을 사용하여 바람 힘 계산 (점점 빨라짐)
            float progress = elapsedTime / windAccelerationTime; // 0 ~ 1로 변환
            currentWindForce = Mathf.Pow(progress, 3) * maxWindForce; // 제곱 함수 적용

            // 상승 효과 적용
            playerRb.AddForce(new Vector2(0, currentWindForce), ForceMode2D.Force);

            yield return new WaitForFixedUpdate(); // FixedUpdate 주기마다 실행
        }

        // 최대 바람 힘 유지
        while (true)
        {
            playerRb.AddForce(new Vector2(0, maxWindForce), ForceMode2D.Force);
            yield return new WaitForFixedUpdate();
        }
    }

}

*/

/*
using UnityEngine;


public class WindZone : MonoBehaviour
{
    [SerializeField]
    private float windForce = 1f; // 상승 힘
    [SerializeField]
    private float upperHeight = 3f; // 윈드 존 기준 상단 높이
    [SerializeField]
    private float lowerHeight = 0.5f; // 윈드 존 기준 하단 높이
    [SerializeField]
    private float horizontalRange = 2f; // 발판 중심으로부터의 수평 범위
    [SerializeField]
    private float forceSmoothing = 0.1f; // 부드럽게 힘을 적용하는 정도
    [SerializeField]
    private float defaultGravityScale = 1f; // 플레이어의 기본 중력 값

    private Transform playerTransform;
    private Rigidbody2D playerRb;
    private PlayerControllerbyBae playerController;

    private void FixedUpdate()
    {
        // 플레이어 오브젝트 찾기
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerRb = player.GetComponent<Rigidbody2D>();
                playerController = player.GetComponent<PlayerControllerbyBae>();
                if (playerRb != null)
                {
                    defaultGravityScale = playerRb.gravityScale; // 초기 중력값 저장
                }
            }
        }

        // 플레이어가 없거나 컴포넌트가 누락된 경우 실행 중단
        if (playerTransform == null || playerRb == null || playerController == null) return;

        // 발판 기준 높이와 수평 거리 계산
        float playerHeight = playerTransform.position.y;
        float playerHorizontalDistance = Mathf.Abs(playerTransform.position.x - transform.position.x);
        float zoneBottom = transform.position.y + lowerHeight;
        float zoneTop = transform.position.y + upperHeight;

        // 조건 확인: 플레이어가 높이 범위와 수평 범위 내에 있고 글라이딩 중인지
        if (playerHeight >= zoneBottom && playerHeight <= zoneTop &&
            playerHorizontalDistance <= horizontalRange && playerController.IsGliding)
        {
            // 중력을 제거하고 상승 효과 적용
            playerRb.gravityScale = 0;
            float smoothedForce = windForce * forceSmoothing;
            playerRb.AddForce(new Vector2(0, smoothedForce), ForceMode2D.Force);
        }
        else
        {
            // 영역을 벗어나면 중력을 원래 값으로 복원
            playerRb.gravityScale = defaultGravityScale;
        }
    }
}
*/
/*using UnityEngine;

public class WindZone : MonoBehaviour
{
    [SerializeField]
    private float windForce = 1f; // 상승 힘
    [SerializeField]
    private float upperHeight = 3f; // 윈드 존 기준 상단 높이
    [SerializeField]
    private float lowerHeight = 0.5f; // 윈드 존 기준 하단 높이
    [SerializeField]
    private float horizontalRange = 2f; // 발판 중심으로부터의 수평 범위
    [SerializeField]
    private float forceSmoothing = 0.1f; // 부드럽게 힘을 적용하는 정도
    [SerializeField]
    private float defaultGravityScale = 1f; // 플레이어의 기본 중력 값

    private Transform playerTransform;
    private Rigidbody2D playerRb;
    private PlayerControllerbyBae playerController;

    private void FixedUpdate()
    {
        // 플레이어 오브젝트 찾기
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerRb = player.GetComponent<Rigidbody2D>();
                playerController = player.GetComponent<PlayerControllerbyBae>();
            }
        }

        // 플레이어가 없거나 컴포넌트가 누락된 경우 실행 중단
        if (playerTransform == null || playerRb == null || playerController == null) return;

        // 발판 기준 높이와 수평 거리 계산
        float playerHeight = playerTransform.position.y;
        float playerHorizontalDistance = Mathf.Abs(playerTransform.position.x - transform.position.x);
        float zoneBottom = transform.position.y + lowerHeight;
        float zoneTop = transform.position.y + upperHeight;

        // 조건 확인: 플레이어가 높이 범위와 수평 범위 내에 있고 글라이딩 중인지
        if (playerHeight >= zoneBottom && playerHeight <= zoneTop &&
            playerHorizontalDistance <= horizontalRange && playerController.IsGliding)
        {
            // 부드럽게 상승 효과 적용 (AddForce 사용)
            float smoothedForce = windForce * forceSmoothing;
            playerRb.AddForce(new Vector2(0, smoothedForce), ForceMode2D.Force);
        }
    }
}
*/

/*using UnityEngine;

public class WindZone : MonoBehaviour
{
    [SerializeField]
    private float windForce = 1f; // 상승 힘
    [SerializeField]
    private float upperHeight = 3f; // 윈드 존 기준 상단 높이
    [SerializeField]
    private float lowerHeight = 0.5f; // 윈드 존 기준 하단 높이
    [SerializeField]
    private float horizontalRange = 2f; // 발판 중심으로부터의 수평 범위

    private Transform playerTransform;
    private Rigidbody2D playerRb;
    private PlayerControllerbyBae playerController;

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
                playerController = player.GetComponent<PlayerControllerbyBae>();
            }
        }

        // 플레이어가 없거나 컴포넌트가 누락된 경우 실행 중단
        if (playerTransform == null || playerRb == null || playerController == null) return;

        // 발판 기준 높이와 수평 거리 계산
        float playerHeight = playerTransform.position.y;
        float playerHorizontalDistance = Mathf.Abs(playerTransform.position.x - transform.position.x);
        float zoneBottom = transform.position.y + lowerHeight;
        float zoneTop = transform.position.y + upperHeight;

        // 조건 확인: 플레이어가 높이 범위와 수평 범위 내에 있고 글라이딩 중인지
        if (playerHeight >= zoneBottom && playerHeight <= zoneTop &&
            playerHorizontalDistance <= horizontalRange && playerController.IsGliding)
        {
            // 상승 효과 적용
            playerRb.velocity = new Vector2(playerRb.velocity.x, windForce);
        }
    }
}


*/

/*using UnityEngine;

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

*/
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