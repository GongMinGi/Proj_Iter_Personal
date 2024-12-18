using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ElevatorMovement : MonoBehaviour
{

    public Transform player;                // 플레이어 Transform
    public Transform mainCamera;            // 메인 카메라 Transform

    public Image blackoutImage;             // 검은색 단색 이미지 (UI)

    public float moveSpeed = 2f;            // 엘리베이터 이동 속도
    public float moveDuration = 5f;         // 엘리베이터가 올라갈 시간
    public float blackoutSpeed = 2f;        // 검은색 이미지가 내려오는 속도

    private bool isPlayerOnPlatform = false; // 플레이어가 플랫폼 위에 있는지 확인
    private bool isMoving = false;          // 엘리베이터가 움직이는지 확인

    private float moveTimer = 0f;           // 이동 시간 타이머

    private Vector3 cameraFixedPosition;    // 카메라 고정 위치 저장

    void Start()
    {
        
        if (blackoutImage != null)      // 블랙아웃 이미지 초기 상태를 비활성화
        {

            blackoutImage.gameObject.SetActive(false);
        
        }

    }

    void Update()
    {
        
        if (isPlayerOnPlatform && !isMoving && Input.GetKeyDown(KeyCode.E))     // 플레이어가 플랫폼 위에 있고 E 키를 누르면 엘리베이터 시작
        {

            isMoving = true;

            moveTimer = 0f; // 타이머 초기화

            FixCameraPosition(); // 카메라 위치 고정

        }

        if (isMoving)        // 엘리베이터 이동
        {

            MoveElevatorUp();

        }

    }

    private void MoveElevatorUp()
    {

        if (moveTimer < moveDuration)
        {
            
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;      // 플랫폼의 Y 좌표를 서서히 증가
            player.position += Vector3.up * moveSpeed * Time.deltaTime;         // 플레이어의 Y 좌표를 서서히 증가

            moveTimer += Time.deltaTime;        // 타이머 증가
            
            if (moveDuration - moveTimer <= 1f)     // 시간이 1초 남았을 때 검은 이미지 애니메이션 시작
            {

                StartCoroutine(BlackoutEffect());

            }

        }
        else
        {

            isMoving = false;

            SceneManager.LoadScene("LabARooftopScene");

        }

    }

    private void FixCameraPosition()
    {
        
        cameraFixedPosition = mainCamera.position;      // 카메라의 현재 위치를 저장하고 고정

    }

    private void LateUpdate()
    {
        
        if (isMoving)       
        {

            mainCamera.position = cameraFixedPosition;      // 카메라를 고정된 위치에 유지

        }

    }

    private IEnumerator BlackoutEffect()
    {

        if (blackoutImage.gameObject.activeSelf) yield break;   // 중복 실행 방지

        blackoutImage.gameObject.SetActive(true);   // 검은색 이미지 활성화

        RectTransform blackoutRect = blackoutImage.rectTransform;

        Vector3 startPosition = new Vector3(0, Screen.height, 0);   // 이미지 시작 위치 (화면 위쪽 바깥) 설정
        Vector3 endPosition = Vector3.zero;     // 이미지 끝 위치 (화면 중앙) 설정

        float elapsedTime = 0f;

        while (elapsedTime < 1f / blackoutSpeed)
        {
            
            float t = elapsedTime / (1f / blackoutSpeed);
            blackoutRect.localPosition = Vector3.Lerp(startPosition, endPosition, t);       // 이미지의 위치를 위에서 아래로 서서히 이동

            elapsedTime += Time.deltaTime;

            yield return null;

        }

        blackoutRect.localPosition = endPosition; // 위치 고정

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {

            isPlayerOnPlatform = true;

        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {

            isPlayerOnPlatform = false;

        }

    }

}
