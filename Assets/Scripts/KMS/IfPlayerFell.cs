using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IfPlayerFell : MonoBehaviour
{

    private Vector3 lastSafePosition;       // 낭떠러지에 떨어지지 않은 마지막 안전한 위치 저장
    private Rigidbody2D playerRigidbody;    // 플레이어 Rigidbody2D

    public float dangerYThreshold = -100f;

    [Header("Fade Settings")]
    public Image fadeScreen;    // Inspector에서 드래그하여 설정
    public float fadeDuration = 1.0f;   // 페이드 인/아웃 지속 시간

    private void Awake()
    {

        playerRigidbody = GetComponent<Rigidbody2D>();

        lastSafePosition = transform.position;

        if (fadeScreen == null)
        {

            Debug.LogError("FadeScreen이 설정되지 않았습니다. Inspector에서 FadeScreen을 설정해주세요.");

        }
        else
        {

            fadeScreen.gameObject.SetActive(false);     // 처음엔 비활성화

        }

    }

    void Update()
    {

        if (transform.position.y < dangerYThreshold) 
        {

            StartCoroutine(ComeBack());

        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("FellPoint"))
        {

            lastSafePosition = transform.position;
            Debug.Log("lastSafePosition updated");

        }

    }

    IEnumerator ComeBack()
    {

        if (fadeScreen == null)
        {

            Debug.LogError("FadeScreen이 설정되지 않았습니다. 페이드 효과를 건너뜁니다.");

            yield break;

        }

        fadeScreen.gameObject.SetActive(true);      // 페이드스크린 활성화

        // 페이드아웃
        float fadeTimer = 0.0f;
        Color fadeColor = fadeScreen.color;

        while (fadeTimer < fadeDuration)
        {

            fadeTimer += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(0, 1, fadeTimer / fadeDuration);
            fadeScreen.color = fadeColor;

            yield return null;

        }

        fadeColor.a = 1;
        fadeScreen.color = fadeColor;

        
        transform.position = lastSafePosition;      // 플레이어 위치를 안전한 위치로 복구하고

        playerRigidbody.linearVelocity = Vector3.zero;      // 속도 0으로 만들고

        // 페이드인
        fadeTimer = 0.0f;

        while (fadeTimer < fadeDuration)
        {

            fadeTimer += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(1, 0, fadeTimer / fadeDuration);
            fadeScreen.color = fadeColor;

            yield return null;

        }

        fadeColor.a = 0;
        fadeScreen.color = fadeColor;

        
        fadeScreen.gameObject.SetActive(false);     // 페이드스크린 비활성화
    }

}
