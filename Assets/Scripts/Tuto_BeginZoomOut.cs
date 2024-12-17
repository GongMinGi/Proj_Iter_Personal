using UnityEngine;
using System.Collections;

// Made by KMS
public class Tuto_BeginZoomOut : MonoBehaviour
{

    public Camera mainCamera;      // 메인 카메라
    public Transform target;       // 캐릭터 위치
    public float zoomInSize = 2f;  // 줌 인 시 카메라 크기
    public float normalSize = 5f;  // 원래 상태의 카메라 크기
    public float zoomDuration = 2f; // 줌 인 줌 아웃 시간

    private float elapsedTime = 0f;

    void Start()
    {
        
        mainCamera.transform.position = new Vector3(target.position.x, target.position.y, mainCamera.transform.position.z);     // 초기 카메라 위치를 캐릭터에 맞추고 줌 인 시작
        mainCamera.orthographicSize = zoomInSize;
        
        StartCoroutine(ZoomOutEffect());        // 줌 아웃을 시작

    }

    void Update()
    {

        mainCamera.transform.position = new Vector3(target.position.x, target.position.y, mainCamera.transform.position.z);

    }


    IEnumerator ZoomOutEffect()
    {

        while (elapsedTime < zoomDuration)
        {

            elapsedTime += Time.deltaTime;

            mainCamera.orthographicSize = Mathf.Lerp(zoomInSize, normalSize, elapsedTime / zoomDuration);       // Orthographic Size를 점진적으로 변경

            yield return null;

        }
        
        mainCamera.orthographicSize = normalSize;       // 줌 아웃 완료 후 정확한 값으로 설정

    }

}