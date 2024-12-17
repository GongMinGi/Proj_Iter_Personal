using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 씬 전환을 위해 필요
using System.Collections;

public class ElevatorMovement : MonoBehaviour
{
    public Transform player;                // 플레이어 Transform
    public Transform mainCamera;            // 메인 카메라 Transform
    public float moveSpeed = 2f;            // 엘리베이터 이동 속도
    public float moveDuration = 5f;         // 엘리베이터가 올라갈 시간
    public Image blackoutImage;             // 검은색 단색 이미지 (UI)
    public float blackoutSpeed = 2f;        // 검은색 이미지가 내려오는 속도
    public string nextSceneName = "LabARooftopScene"; // 이동할 씬 이름

    private bool isPlayerOnPlatform = false; // 플레이어가 플랫폼 위에 있는지 확인
    private bool isMoving = false;          // 엘리베이터가 움직이는지 확인
    private float moveTimer = 0f;           // 이동 시간 타이머
    private Vector3 cameraFixedPosition;    // 카메라 고정 위치 저장

    void Start()
    {
        // 블랙아웃 이미지 초기 상태를 비활성화
        if (blackoutImage != null)
        {
            blackoutImage.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // 플레이어가 플랫폼 위에 있고 E 키를 누르면 엘리베이터 시작
        if (isPlayerOnPlatform && !isMoving && Input.GetKeyDown(KeyCode.E))
        {
            isMoving = true;
            moveTimer = 0f; // 타이머 초기화
            FixCameraPosition(); // 카메라 위치 고정
        }

        // 엘리베이터 이동
        if (isMoving)
        {
            MoveElevatorUp();
        }
    }

    private void MoveElevatorUp()
    {
        if (moveTimer < moveDuration)
        {
            // 플랫폼과 플레이어의 Y 좌표를 서서히 증가
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            player.position += Vector3.up * moveSpeed * Time.deltaTime;

            // 타이머 증가
            moveTimer += Time.deltaTime;

            // 시간이 1초 남았을 때 검은 이미지 애니메이션 시작
            if (moveDuration - moveTimer <= 1f)
            {
                StartCoroutine(BlackoutEffect());
            }
        }
        else
        {
            isMoving = false;
            Debug.Log("Elevator stopped after reaching the time limit.");
            StartCoroutine(TransitionToNextScene()); // 다음 씬으로 이동
        }
    }

    private void FixCameraPosition()
    {
        // 카메라의 현재 위치를 저장하고 고정
        cameraFixedPosition = mainCamera.position;
    }

    private void LateUpdate()
    {
        // 카메라를 고정된 위치에 유지
        if (isMoving)
        {
            mainCamera.position = cameraFixedPosition;
        }
    }

    private IEnumerator BlackoutEffect()
    {
        if (blackoutImage.gameObject.activeSelf) yield break; // 중복 실행 방지

        blackoutImage.gameObject.SetActive(true); // 검은색 이미지 활성화

        RectTransform blackoutRect = blackoutImage.rectTransform;

        // 이미지 시작 위치 (화면 위 바깥쪽)와 끝 위치 (화면 중앙) 설정
        Vector3 startPosition = new Vector3(0, Screen.height, 0); // 화면 위쪽 바깥
        Vector3 endPosition = Vector3.zero; // 화면 중앙

        float elapsedTime = 0f;

        while (elapsedTime < 1f / blackoutSpeed)
        {
            // 이미지의 위치를 위에서 아래로 서서히 이동
            float t = elapsedTime / (1f / blackoutSpeed);
            blackoutRect.localPosition = Vector3.Lerp(startPosition, endPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        blackoutRect.localPosition = endPosition; // 위치 고정
        Debug.Log("Screen blacked out.");
    }

    private IEnumerator TransitionToNextScene()
    {
        // 블랙아웃이 끝난 후 잠시 기다린 뒤 씬 전환
        yield return new WaitForSeconds(1f);
        Debug.Log("Transitioning to next scene...");
        SceneManager.LoadScene(nextSceneName); // 다음 씬으로 이동
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnPlatform = true;
            Debug.Log("Player is on the platform. Press 'E' to start elevator.");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnPlatform = false;
            Debug.Log("Player left the platform.");
        }
    }
}
