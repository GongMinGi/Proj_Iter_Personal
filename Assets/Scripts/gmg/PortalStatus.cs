using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PortalStatus : MonoBehaviour
{
    [SerializeField]
    private string nextSceneName;

    private Rigidbody2D playerRigidbody;    //플레이어에 부착된 rigidbody를 저장
    private Transform cameraTransform;      //메인 카메라의 trasnform 저장
    private Vector3 cameraOriginalPosition; //카메라의 원래위치 저장(씬 전환 시 카메라 위치고정을 위해)

    [Header("Fade Settings")]
    public Image fadeScreen;                // 페이드 효과에 사용할 이미지
    public float fadeSpeed = 1.0f;          //페이드 효과의 속도를 조절하는 변수

    [Header("Scene Settings")]
    public string sceneToLoad;               //전환할 씬의 이름

    private bool isTransitioning = false;       //씬 전환이 진행중인지를 나타내는 변수(중복 실행 방지)

    private void Awake()
    {
        cameraTransform = Camera.main.transform;

        // 페이드 화면 초기화
        if (fadeScreen == null)
        {
            Debug.LogError("FadeScreen이 설정되지 않았습니다. Inspector에서 FadeScreen을 설정해주세요.");
        }
        else
        {
            fadeScreen.gameObject.SetActive(false);     //시작시 fadescreen을 비활성화해서 화면에 나오지 않도록 함
            Color initialColor = fadeScreen.color;      //fadescreen의 초기 색상을 가져옴... 안씀
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !isTransitioning)
        {
            playerRigidbody = other.GetComponent<Rigidbody2D>();

            playerRigidbody.linearVelocity = Vector2.zero; // 플레이어 움직임 멈춤

            // 카메라 원래 위치 저장 (페이드 효과동안 카메라를 고정하기 위해)
            cameraOriginalPosition = cameraTransform.position;

            // 카메라와 플레이어의 Parent 관계 해제
            if (cameraTransform.parent == transform)
            {
                cameraTransform.SetParent(null, true); // 부모 관계 해제 (월드 좌표 유지)
            }

            //씬 전환을 위한 코루틴 시작
            StartCoroutine(TransitionScene());
        }
    }



    // 페이즈 효과와 씬 전환을 순차적으로 진행
    IEnumerator TransitionScene()
    {
        // 씬 전환 중임을 표시하여 중복 실행 방지
        isTransitioning = true;

        // 카메라 위치 고정 (fade 효과 동안 카메라 위치를 고정하는 데 사용)
        Vector3 fixedCameraPosition = cameraTransform.position;

        // 전환 시작 전에 0.8초 대기
        yield return new WaitForSeconds(0.8f);

        // 검은 화면 페이드 인 효과
        yield return StartCoroutine(FadeOutEffect());

        //페이드 효과 이후 잠깐 정지
        yield return new WaitForSeconds(0.5f);

        // 씬 전환
        SceneManager.LoadScene(sceneToLoad);
    }



    // 페이드아웃(검은 화면이 나타나는) 효과 구현
    private IEnumerator FadeOutEffect()
    {
        //fadescreen의 rectTranform을 가져옴( 화면 내 위치 및 크기를 조절하기 위해)
        RectTransform fadeScreenRect = fadeScreen.rectTransform;

        // 초기 위치: 화면 아래
        Vector3 startPosition = new Vector3(0, -Screen.height, 0);
        Vector3 endPosition = Vector3.zero; // 화면 중앙으로 이동

        fadeScreenRect.localPosition = startPosition;   // fadescreen의 초기 위치 설정
        fadeScreen.gameObject.SetActive(true);          //fadescreen 오브젝트를 활성화하여 화면에 표시

        float elapsedTime = 0f;              // 경과시간을 저장할 변수 초기화

        while (elapsedTime < 1f / fadeSpeed) // 페이드 속도에 따라 시간 계산 1/fadespeed초 동안 진행
        {
            float t = elapsedTime / (1f / fadeSpeed); // t 값은 0과 1 사이의 진행률( 현재 진행 정도)

            fadeScreenRect.localPosition = Vector3.Lerp(startPosition, endPosition, t);

            // 카메라 위치를 계속 고정 (페이드 효과 동안 카메라의 위치를 계속 원래 위치로 고정)
            cameraTransform.position = cameraOriginalPosition;

            // 경과 시간 증가 ( 프레임당 경과 시간)
            elapsedTime += Time.deltaTime;

            // 다음 프레임까지 대기
            yield return null;
        }

        // 반복문 종료 후 fadeScreen의 위치를 정확히 종료 위치로 설정
        fadeScreenRect.localPosition = endPosition; // 최종 위치 고정
    }
}
