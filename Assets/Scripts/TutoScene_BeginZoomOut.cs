using System.Collections;
using UnityEngine;

// Made by KMS
public class TutoScene_BeginZoomOut : MonoBehaviour
{
    public Camera mainCamera;      // 메인 카메라
    public Transform target;       // 캐릭터 위치
    public float zoomInSize = 2f;  // 줌 인 시 카메라 크기
    public float normalSize = 5f;  // 원래 상태의 카메라 크기
    public float zoomDuration = 2f; // 줌 인/아웃 시간

    private float elapsedTime = 0f;

    void Start()
    {
        // 초기 카메라 위치를 캐릭터에 맞추고 줌 인 시작
        mainCamera.transform.position = new Vector3(target.position.x, target.position.y, mainCamera.transform.position.z);
        mainCamera.orthographicSize = zoomInSize;

        // 줌 아웃을 시작
        StartCoroutine(ZoomOutEffect());
    }

    IEnumerator ZoomOutEffect()
    {
        while (elapsedTime < zoomDuration)
        {
            elapsedTime += Time.deltaTime;
            // Orthographic Size를 점진적으로 변경
            mainCamera.orthographicSize = Mathf.Lerp(zoomInSize, normalSize, elapsedTime / zoomDuration);
            yield return null;
        }

        // 줌 아웃 완료 후 정확한 값으로 설정
        mainCamera.orthographicSize = normalSize;
    }
}
