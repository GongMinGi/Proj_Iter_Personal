using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ToRealGame : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private Transform cameraTransform;
    private Vector3 cameraOriginalPosition;
    public float dangerYThreshold = -50f;

    [Header("Fade Settings")]
    public Image fadeScreen;
    public float fadeSpeed = 1.0f;

    [Header("Scene Settings")]
    public string sceneToLoad = "CharacterAndTile";

    private bool isTransitioning = false;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        cameraTransform = Camera.main.transform;

        // 페이드 화면 초기화
        if (fadeScreen == null)
        {
            Debug.LogError("FadeScreen이 설정되지 않았습니다. Inspector에서 FadeScreen을 설정해주세요.");
        }
        else
        {
            fadeScreen.gameObject.SetActive(false);
            Color initialColor = fadeScreen.color;
        }
    }

    private void Update()
    {
        if (!isTransitioning && transform.position.y < dangerYThreshold)
        {
            playerRigidbody.linearVelocity = Vector2.zero; // 플레이어 움직임 멈춤

            // 카메라 원래 위치 저장
            cameraOriginalPosition = cameraTransform.position;

            // 카메라와 플레이어의 Parent 관계 해제
            if (cameraTransform.parent == transform)
            {
                cameraTransform.SetParent(null, true); // 부모 관계 해제 (월드 좌표 유지)
            }

            StartCoroutine(TransitionScene());
        }
    }

    IEnumerator TransitionScene()
    {
        isTransitioning = true;

        // 카메라 위치 고정
        Vector3 fixedCameraPosition = cameraTransform.position;

        // 0.5초 대기
        yield return new WaitForSeconds(0.8f);

        // 검은 화면 페이드 인 효과
        yield return StartCoroutine(FadeOutEffect());

        yield return new WaitForSeconds(0.5f);

        // 씬 전환
        SceneManager.LoadScene(sceneToLoad);
    }

    private IEnumerator FadeOutEffect()
    {
        RectTransform fadeScreenRect = fadeScreen.rectTransform;

        // 초기 위치: 화면 아래
        Vector3 startPosition = new Vector3(0, -Screen.height, 0);
        Vector3 endPosition = Vector3.zero; // 화면 중앙으로 이동

        fadeScreenRect.localPosition = startPosition; // 초기 위치 설정
        fadeScreen.gameObject.SetActive(true);

        float elapsedTime = 0f;

        while (elapsedTime < 1f / fadeSpeed) // 페이드 속도에 따라 시간 계산
        {
            float t = elapsedTime / (1f / fadeSpeed);

            fadeScreenRect.localPosition = Vector3.Lerp(startPosition, endPosition, t);

            // 카메라 위치를 계속 고정
            cameraTransform.position = cameraOriginalPosition;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        fadeScreenRect.localPosition = endPosition; // 최종 위치 고정
    }
}
