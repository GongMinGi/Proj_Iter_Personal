using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IfPlayerFell : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector3 lastSafePosition;       // 낭떠러지에 떨어지지 않은 마지막 안전한 위치 저장
    public float dangerYThreshold = -100f;

    [Header("Fade Settings")]
    public Image fadeScreen; // Inspector에서 드래그하여 설정
    public float fadeDuration = 1.0f; // 페이드 인/아웃 지속 시간

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastSafePosition = transform.position;

        if (fadeScreen == null)
        {
            Debug.LogError("FadeScreen이 설정되지 않았습니다. Inspector에서 FadeScreen을 설정해주세요.");
        }
        else
        {
            fadeScreen.gameObject.SetActive(false); // 처음엔 비활성화
        }
    }

    void Update()
    {
        CheckFell();
    }

    private void CheckFell()
    {
        if (!IsEdgeAhead(transform.position))
        {
            Debug.Log($"Last Safe Position updated to: {lastSafePosition}"); // 디버그 로그 추가
            lastSafePosition = transform.position;
        }
        else
        {
            if (transform.position.y < dangerYThreshold)
            {
                StartCoroutine(ComeBack());
            }
        }
    }

    private bool IsEdgeAhead(Vector3 position)
    {
        Vector2 frontVec = new Vector2(position.x + (spriteRenderer.flipX ? -0.5f : 0.5f), position.y); //스프라이트 앞쪽의 위치 계산
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));  //디버그 레이를 아래로 쏨
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 5, LayerMask.GetMask("Platform")); // "Platform" 레이어로 레이케스트 실행

        if (rayHit.collider == null)    // 레이케스트가 충돌하지 않으면 낭떠러지임
        {
            return true;
        }
        return false;   //낭떠러지가 아님
    }

    IEnumerator ComeBack()
    {
        if (fadeScreen == null)
        {
            Debug.LogError("FadeScreen이 설정되지 않았습니다. 페이드 효과를 건너뜁니다.");
            yield break;
        }

        // 페이드스크린 활성화
        fadeScreen.gameObject.SetActive(true);

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

        // 플레이어 위치를 안전한 위치로 복구
        transform.position = lastSafePosition;

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

        // 페이드스크린 비활성화
        fadeScreen.gameObject.SetActive(false);
    }
}
