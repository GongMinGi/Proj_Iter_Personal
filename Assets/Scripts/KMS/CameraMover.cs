using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour
{

    [Header("카메라 이동")]
    
    [SerializeField] 
    private Transform[] cameraSpots;
    
    [SerializeField] 
    private float[] moveSpeeds;

    [SerializeField]
    private float smoothTime = 0.3f;

    [Header("카메라 줌")]
    
    [SerializeField]
    private float[] zoomLevels;

    [SerializeField] 
    private float zoomSmoothTime = 0.5f;

    private int currentSpotIndex = 0;
    private bool isMoving = false;

    private Vector3 velocity = Vector3.zero;
    private float zoomVelocity = 0f;

    private Camera mainCamera;

    private StopPlayerOnTrigger stopPlayerOnTrigger;    // StopPlayerOnTrigger 참조

    private void Start()
    {

        Debug.Log("컷신 시작");

        mainCamera = Camera.main;

        stopPlayerOnTrigger = FindFirstObjectByType<StopPlayerOnTrigger>();     // StopPlayerOnTrigger 참조 초기화

        if (cameraSpots.Length == 0 || moveSpeeds.Length == 0 || zoomLevels.Length == 0)
        {

            Debug.LogError("Camera spots, move speeds, or zoom levels are not assigned!");

        }
        else if (cameraSpots.Length != moveSpeeds.Length || cameraSpots.Length != zoomLevels.Length)
        {

            Debug.LogError("The number of camera spots, move speeds, and zoom levels must match!");

        }

        if (stopPlayerOnTrigger == null)
        {

            Debug.LogError("StopPlayerOnTrigger script not found!");

        }

    }

    public void MoveToNextSpot()
    {

        if (currentSpotIndex >= cameraSpots.Length || isMoving)
        {

            Debug.Log("리턴합니다");
            return;

        }

        StartCoroutine(MoveCamera(cameraSpots[currentSpotIndex].position, moveSpeeds[currentSpotIndex], zoomLevels[currentSpotIndex]));

    }

    private IEnumerator MoveCamera(Vector3 targetPosition, float speed, float targetZoom)
    {

        isMoving = true;
        Debug.Log("isMoving 트루");

        float startZoom = mainCamera.orthographicSize;

        targetPosition.z = mainCamera.transform.position.z;

        while (Vector3.Distance(new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 0),
                                              new Vector3(targetPosition.x, targetPosition.y, 0)) > 0.01f ||
               Mathf.Abs(mainCamera.orthographicSize - targetZoom) > 0.01f)
        {

            Vector3 currentPosition = mainCamera.transform.position;

            mainCamera.transform.position = Vector3.SmoothDamp(
                currentPosition,
                new Vector3(targetPosition.x, targetPosition.y, currentPosition.z),
                ref velocity,
                smoothTime,
                speed);

            mainCamera.orthographicSize = Mathf.SmoothDamp(
                mainCamera.orthographicSize,
                targetZoom,
                ref zoomVelocity,
                zoomSmoothTime);

            yield return null;

        }

        mainCamera.transform.position = new Vector3(targetPosition.x, targetPosition.y, mainCamera.transform.position.z);
        mainCamera.orthographicSize = targetZoom;

        Debug.Log($"Camera has moved to spot {currentSpotIndex + 1}.");

        currentSpotIndex++;
        isMoving = false;

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
