using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossIntroController : MonoBehaviour
{
    [Header("카메라 및 대상")]
    public Camera mainCamera;
    public Transform bossTransform;
    public Transform playerTransform;
    private Vector3 bossPos;

    [Header("카메라 이동 설정")]
    public float cameraMoveDuration = 2f; //카메라 이동 시간

    [Header("보스 UI")]
    public Text bossNameText;               // 보스 이름을 적을 텍스트 변수
    public float bossNameDisplayTime = 2f;  // 보스 이름을 나타낼 시간
    public Slider bossHealthBar;            // 보스 체력바( slilder)
    public float healthBarFillDuration = 2f; // 체력바를 채우는 시간

    [Header("보스 애니메이터")]
    public Animator bossAnimator;

    [Header("플레이어 컨트롤")]
    public MonoBehaviour playerController;  // 플레이어 컨트롤 스크립트
    public Rigidbody2D playerRigidbody;

    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;



    void Start()
    {
        // 보스 UI의 초기상태는 false로 설정 (보스전 시 활성화)
        if(bossNameText != null)
        {
            bossNameText.gameObject.SetActive(false);
        }
        if(bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(false); 
        }
    }

    //보스 방에 들어가면 이 함수를 호출하여 연출 시작
    public void StartBossIntro()
    {
        StartCoroutine(BossIntroSequence());
    }

    IEnumerator BossIntroSequence()
    {

        //1. 플레이어 제어 비활성화
        if(playerController != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
            playerController.enabled = false;
        }


        //2. 현재 카메라 위치와 회전 저장( 복귀용)
        originalCameraPosition = mainCamera.transform.position;
        originalCameraRotation = mainCamera.transform.rotation;

        //보스 위치 중심이 밑바닥으로 맞춰져있어서 조금 띄워줘야 카메라가 보스에 중심을 잡음
        bossPos = bossTransform.position;
        bossPos.y += 2f;

        //3. 카메라를 보스 위치로 이동
        yield return StartCoroutine(MoveCamera(bossPos, bossTransform.rotation, cameraMoveDuration));


        //4. 보스 이름 텍스트 표시
        if(bossNameText != null)
        {
            bossNameText.text = "Project_ZEUS";
            bossNameText.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(bossNameDisplayTime);
        if (bossNameText != null)
            bossNameText.gameObject.SetActive(false);

        
        // 5.체력바 UI 활성화 및 채우기 애니메이션
        if(bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(true);
            float elapsedTime = 0f;

            //초기 체력바를 0으로 설정하고 0~100%까지 점점 차오르게 선형보간
            while( elapsedTime < healthBarFillDuration)
            {
                bossHealthBar.value = Mathf.Lerp(0, bossHealthBar.maxValue, elapsedTime/healthBarFillDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            bossHealthBar.value = bossHealthBar.value;
        }

        // 6. 카메라를 플레이어로 복귀
        yield return StartCoroutine(MoveCamera(originalCameraPosition, originalCameraRotation, cameraMoveDuration));

        // 7. 플레이어 제어권 재활성화
        if(playerController != null)
        {
            playerController.enabled = true;
        }

        // 8. 보스 애니메이터 활성화 -> 전투 시작
        if(bossAnimator != null)
        {
            bossAnimator.enabled = true ;
        }
        

    }

    // 카메라 이동을 위한 코루틴( 선형 보간)
    IEnumerator MoveCamera(Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;

        targetPosition.z = startPos.z; // 카메라와 오브젝트의 z축은 다르게 설정되어있음, 카메라z축이 바뀌면 확대정도가 바뀌므로 바뀌지않게 조정


        float elapsed = 0f;  // Time.deltaTime 에 맞춰서 증가
        while (elapsed < duration)
        {
            float t = elapsed / duration; // deltatime/ 2초 가되므로 점점 targetpositon에다가감
            mainCamera.transform.position = Vector3.Lerp(startPos, targetPosition, t);
            mainCamera.transform.rotation = Quaternion.Lerp(startRot, targetRotation, t);
            elapsed += Time.deltaTime;
            yield return null;

        }
        // 2초가 지나면 반복문을 빠져나와 targetPosition으로 위치를 설정해줌
        mainCamera.transform.position = targetPosition;
        mainCamera.transform.rotation = targetRotation;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
