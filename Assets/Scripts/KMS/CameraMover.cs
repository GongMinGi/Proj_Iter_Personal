using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour
{

    [Header("ī�޶� �̵�")]
    
    [SerializeField] 
    private Transform[] cameraSpots; //ī�޶� �̵� ��ġ ���
    
    [SerializeField] 
    private float[] moveSpeeds; // �� ��ġ�� �̵��ϴ� �ӵ�

    [SerializeField]
    private float smoothTime = 0.3f; // �̵� �ε巯���� ���� �ð� ����

    [Header("ī�޶� ��")]
    
    [SerializeField]
    private float[] zoomLevels; // ī�޶� Ȯ�� /��� ����

    [SerializeField] 
    private float zoomSmoothTime = 0.5f; // �� �ε巯���� ���� �ð� ����

    private int currentSpotIndex = 0; // ���� ī�޶� �̵� �ε���
    private bool isMoving = false;  // �̵������� ���� �÷���

    private Vector3 velocity = Vector3.zero; // ��ġ �̵��� ���� �ӵ� ����
    private float zoomVelocity = 0f; // �� �̵��� ���� �ӵ� ����

    private Camera mainCamera; // ���� ī�޶� ����

    private StopPlayerOnTrigger stopPlayerOnTrigger;    // StopPlayerOnTrigger ����

    private void Start()
    {

        Debug.Log("�ƽ� ����");

        mainCamera = Camera.main; //���� ī�޶� ��������

        stopPlayerOnTrigger = FindFirstObjectByType<StopPlayerOnTrigger>();     // StopPlayerOnTrigger ���� �ʱ�ȭ

        //ī�޶� �̵��� �ʿ��� �迭�� �������� ���� ��� ���� ���
        if (cameraSpots.Length == 0 || moveSpeeds.Length == 0 || zoomLevels.Length == 0)
        {

            Debug.LogError("Camera spots, move speeds, or zoom levels are not assigned!");

        }
        // �迭 ���̰� ��ġ���� �ʴ� ��� ���� ���
        else if (cameraSpots.Length != moveSpeeds.Length || cameraSpots.Length != zoomLevels.Length)
        {

            Debug.LogError("The number of camera spots, move speeds, and zoom levels must match!");

        }

        //stopPlayerOnTrigger ������ ���� ��� ���� �޼��� ���
        if (stopPlayerOnTrigger == null)
        {

            Debug.LogError("StopPlayerOnTrigger script not found!");

        }

    }

    public void MoveToNextSpot()
    {
        //���� �ε����� �迭 ������ �ʰ��߰ų� �̹� �̵� ���̸� ����
        if (currentSpotIndex >= cameraSpots.Length || isMoving)
        {

            Debug.Log("�����մϴ�");
            return;

        }

        //ī�޶� �̵��� �����ϴ� �ڷ�ƾ ����
        StartCoroutine(MoveCamera(cameraSpots[currentSpotIndex].position, moveSpeeds[currentSpotIndex], zoomLevels[currentSpotIndex]));

    }

    private IEnumerator MoveCamera(Vector3 targetPosition, float speed, float targetZoom)
    {

        isMoving = true; // �̵� �� �÷��� ����
        Debug.Log("isMoving Ʈ��");

        float startZoom = mainCamera.orthographicSize; // ���� �� ũ�� ����

        targetPosition.z = mainCamera.transform.position.z; // ī�޶��� z ��ġ ����

        //��ǥ ��ġ�� �� ���� ������ ������ �ݺ�
        while (Vector3.Distance(new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 0),
                                              new Vector3(targetPosition.x, targetPosition.y, 0)) > 0.01f ||
               Mathf.Abs(mainCamera.orthographicSize - targetZoom) > 0.01f)
        {

            Vector3 currentPosition = mainCamera.transform.position;

            // ī�޶� �̵��� �ε巴�� ó��
            mainCamera.transform.position = Vector3.SmoothDamp(
                currentPosition,
                new Vector3(targetPosition.x, targetPosition.y, currentPosition.z),
                ref velocity,
                smoothTime,
                speed);

            //ī�޶� ���� �ε巴�� ����
            mainCamera.orthographicSize = Mathf.SmoothDamp(
                mainCamera.orthographicSize,
                targetZoom,
                ref zoomVelocity,
                zoomSmoothTime);

            yield return null; // ���� ������ ���� ���

        }

        //���������� ��Ȯ�� ��ġ�� �� �� ����
        mainCamera.transform.position = new Vector3(targetPosition.x, targetPosition.y, mainCamera.transform.position.z);
        mainCamera.orthographicSize = targetZoom;

        Debug.Log($"Camera has moved to spot {currentSpotIndex + 1}.");

        currentSpotIndex++; // ���� ��ġ�� �̵��ϱ� ���� �ε��� ����
        isMoving = false; // �̵� �Ϸ� �� �÷��� ����

        // ��ġ�� �����ִٸ� ��� �̵�
        if (currentSpotIndex < cameraSpots.Length)
        {

            MoveToNextSpot();

        }
        else
        {

            Debug.Log("All camera movements are completed.");

            if (stopPlayerOnTrigger != null)
            {

                stopPlayerOnTrigger.UnfreezePlayerMovement();       // StopPlayerOnTrigger�� UnfreezePlayerMovement ȣ��
            }

        }

    }

}
