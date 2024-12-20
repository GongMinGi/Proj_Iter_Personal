using UnityEngine;

public class StopPlayerOnTrigger : MonoBehaviour
{

    [Header("Target Settings")]

    [SerializeField]
    private string targetTag = "DiscoveryObject";   // 목표 오브젝트의 태그

    [Header("UI Elements")]

    [SerializeField]
    private GameObject uiCanvas;     // 비활성화할 UI

    private PlayerController playerController;    // 플레이어 이동 제어 스크립트
    private Rigidbody2D playerRigidbody;          // 플레이어 Rigidbody2D
    private CameraMover cameraMover;              // 카메라 이동 스크립트

    private bool isTriggered = false;             // 이미 트리거가 실행되었는지 확인

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag(targetTag) && !isTriggered)        // 목표 태그와 충돌했으며, 이미 실행된 적이 없다면
        {

            isTriggered = true; // 중복 실행 방지

            StopPlayerMovement(); // 플레이어 이동 멈춤
            DisableUI();          // UI 비활성화 및 카메라 이동 시작

        }

    }

    private void StopPlayerMovement()
    {
        
        playerRigidbody.linearVelocity = new Vector2(0, playerRigidbody.linearVelocity.y);      // 수평 속도를 0으로 설정하고 
        playerRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX;                     // X축 이동을 비활성화

        Debug.Log("플레이어 이동이 중지되었습니다.");

    }

    private void DisableUI()
    {

        if (uiCanvas != null)
        {

            uiCanvas.SetActive(false);      // UI 비활성화 후

        }

        StartCameraMovement();      // 카메라 이동 시작

    }   

    private void StartCameraMovement()
    {

        if (cameraMover != null)
        {

            cameraMover.MoveToNextSpot();
            Debug.Log("카메라 이동을 시작합니다.");

        }

    }

    public void UnfreezePlayerMovement()
    {
        
        playerRigidbody.constraints = RigidbodyConstraints2D.None;      // 플레이어의 이동 고정 해제
        playerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;        

        if (uiCanvas != null)
        {

            uiCanvas.SetActive(true);
            Debug.Log("UI가 다시 활성화되었습니다.");
            
        }

        Debug.Log("플레이어 이동이 다시 활성화되었습니다.");

    }

}
