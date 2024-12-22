using UnityEngine;

public class StopPlayerOnTrigger : MonoBehaviour
{

    [Header("Target Settings")]
    [SerializeField]
    private string targetTag = "DiscoveryObject"; // 목표 오브젝트의 태그

    [Header("UI Elements")]
    [SerializeField]
    private GameObject uiCanvas; // 비활성화할 UI

    private PlayerController playerController; // 플레이어 이동 제어 스크립트
    private Rigidbody2D playerRigidbody; // 플레이어 Rigidbody2D
    private CameraMover cameraMover; // 카메라 이동 스크립트

    private bool isTriggered = false; // 이미 트리거가 실행되었는지 확인
    private bool isJumpDisabled = false; // 점프 입력 차단 여부

    private void Start()
    {

        // 컴포넌트 초기화
        playerController = GetComponent<PlayerController>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        cameraMover = Camera.main.GetComponent<CameraMover>();

        // 컴포넌트 존재 여부 확인
        if (playerController == null || playerRigidbody == null)
            Debug.LogError("PlayerController 또는 Rigidbody2D가 없습니다.");
        if (cameraMover == null)
            Debug.LogError("CameraMover 스크립트가 Main Camera에 없습니다.");
        if (uiCanvas == null)
            Debug.LogError("UI Canvas가 설정되지 않았습니다.");

    }

    private void Update()
    {
        
        if (isJumpDisabled && Input.GetKeyDown(KeyCode.Space))      // 점프 입력 차단
        {

            Debug.Log("점프가 비활성화된 상태입니다.");

        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag(targetTag) && !isTriggered)    // 목표 태그와 충돌했으며, 이미 실행된 적이 없다면
        {
            isTriggered = true;  // 중복 실행 방지

            StopPlayerMovement();   // 플레이어 이동 멈춤
            DisableUI();     // UI 비활성화 및 카메라 이동 시작
            DisablePlayerJump();    // 점프 비활성화

        }

    }

    private void StopPlayerMovement()
    {

        playerRigidbody.linearVelocity = Vector2.zero;
        playerRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;

        Debug.Log("플레이어 이동이 중지되었습니다.");

    }

    private void DisableUI()
    {

        if (uiCanvas != null)
        {

            uiCanvas.SetActive(false);  // UI 비활성화 후

        }

        StartCameraMovement();   // 카메라 이동 시작

    }

    private void StartCameraMovement()
    {

        if (cameraMover != null)
        {

            cameraMover.MoveToNextSpot();

            Debug.Log("카메라 이동을 시작합니다.");

        }

    }

    private void DisablePlayerJump()
    {

        isJumpDisabled = true;  // 점프 차단 플래그 활성화

        Debug.Log("플레이어 점프가 비활성화되었습니다.");

    }

    private void EnablePlayerJump()
    {

        isJumpDisabled = false; // 점프 차단 플래그 비활성화

        Debug.Log("플레이어 점프가 활성화되었습니다.");

    }

    public void UnfreezePlayerMovement()
    {

        playerRigidbody.constraints = RigidbodyConstraints2D.None;
        playerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (uiCanvas != null)
        {

            uiCanvas.SetActive(true);

        }

        EnablePlayerJump();

        Debug.Log("플레이어 이동이 다시 활성화되었습니다.");

    }

}
