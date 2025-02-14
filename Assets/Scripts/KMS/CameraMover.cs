using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour
{

    [Header("카메라 이동")]
    
    [SerializeField] 
    private Transform[] cameraSpots; //카메라 이동 위치 목록
    
    [SerializeField] 
    private float[] moveSpeeds; // 각 위치로 이동하는 속도

    [SerializeField]
    private float smoothTime = 0.3f; // 이동 부드러움을 위한 시간 변수

    [Header("카메라 줌")]
    
    [SerializeField]
    private float[] zoomLevels; // 카메라 확대 /축소 레벨

    [SerializeField] 
    private float zoomSmoothTime = 0.5f; // 줌 부드러움을 위한 시간 변수

    private int currentSpotIndex = 0; // 현재 카메라 이동 인덱스
    private bool isMoving = false;  // 이동중인지 여부 플래그

    private Vector3 velocity = Vector3.zero; // 위치 이동을 위한 속도 변수
    private float zoomVelocity = 0f; // 줌 이동을 위한 속도 변수

    private Camera mainCamera; // 메인 카메라 참조

    private StopPlayerOnTrigger stopPlayerOnTrigger;    // StopPlayerOnTrigger 참조

    private void Start()
    {

        Debug.Log("컷신 시작");

        mainCamera = Camera.main; //메인 카메라 가져오기

        stopPlayerOnTrigger = FindFirstObjectByType<StopPlayerOnTrigger>();     // StopPlayerOnTrigger 참조 초기화

        //카메라 이동에 필요한 배열이 설정되지 않은 경우 오류 출력
        if (cameraSpots.Length == 0 || moveSpeeds.Length == 0 || zoomLevels.Length == 0)
        {

            Debug.LogError("Camera spots, move speeds, or zoom levels are not assigned!");

        }
        // 배열 길이가 일치하지 않는 경우 오류 출력
        else if (cameraSpots.Length != moveSpeeds.Length || cameraSpots.Length != zoomLevels.Length)
        {

            Debug.LogError("The number of camera spots, move speeds, and zoom levels must match!");

        }

        //stopPlayerOnTrigger 참조가 없을 경우 오류 메세지 출력
        if (stopPlayerOnTrigger == null)
        {

            Debug.LogError("StopPlayerOnTrigger script not found!");

        }

    }

    public void MoveToNextSpot()
    {
        //현재 인덱스가 배열 범위를 초과했거나 이미 이동 중이면 리턴
        if (currentSpotIndex >= cameraSpots.Length || isMoving)
        {

            Debug.Log("리턴합니다");
            return;

        }

        //카메라 이동을 시작하는 코루틴 실행
        StartCoroutine(MoveCamera(cameraSpots[currentSpotIndex].position, moveSpeeds[currentSpotIndex], zoomLevels[currentSpotIndex]));

    }

    private IEnumerator MoveCamera(Vector3 targetPosition, float speed, float targetZoom)
    {

        isMoving = true; // 이동 중 플래그 설정
        Debug.Log("isMoving 트루");

        float startZoom = mainCamera.orthographicSize; // 현재 줌 크기 저장

        targetPosition.z = mainCamera.transform.position.z; // 카메라의 z 위치 유지

        //목표 위치와 줌 값에 도달할 때까지 반복
        while (Vector3.Distance(new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 0),
                                              new Vector3(targetPosition.x, targetPosition.y, 0)) > 0.01f ||
               Mathf.Abs(mainCamera.orthographicSize - targetZoom) > 0.01f)
        {

            Vector3 currentPosition = mainCamera.transform.position;

            // 카메라 이동을 부드럽게 처리
            mainCamera.transform.position = Vector3.SmoothDamp(
                currentPosition,
                new Vector3(targetPosition.x, targetPosition.y, currentPosition.z),
                ref velocity,
                smoothTime,
                speed);

            //카메라 줌을 부드럽게 조절
            mainCamera.orthographicSize = Mathf.SmoothDamp(
                mainCamera.orthographicSize,
                targetZoom,
                ref zoomVelocity,
                zoomSmoothTime);

            yield return null; // 다음 프레임 까지 대기

        }

        //최종적으로 정확한 위치와 줌 값 적용
        mainCamera.transform.position = new Vector3(targetPosition.x, targetPosition.y, mainCamera.transform.position.z);
        mainCamera.orthographicSize = targetZoom;

        Debug.Log($"Camera has moved to spot {currentSpotIndex + 1}.");

        currentSpotIndex++; // 다음 위치로 이동하기 위해 인덱스 증가
        isMoving = false; // 이동 완료 후 플래그 해제

        // 위치가 남아있다면 계속 이동
        if (currentSpotIndex < cameraSpots.Length)
        {

            MoveToNextSpot();

        }
        else
        {

            Debug.Log("All camera movements are completed.");

            if (stopPlayerOnTrigger != null)
            {

                stopPlayerOnTrigger.UnfreezePlayerMovement();       // StopPlayerOnTrigger의 UnfreezePlayerMovement 호출
            }

        }

    }

}
