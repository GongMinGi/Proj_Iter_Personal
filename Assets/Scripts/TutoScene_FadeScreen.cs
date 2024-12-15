using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Made by KMS
public class TutoScene_FadeScreen : MonoBehaviour
{
    public Image fadeImage; // 검정 화면 이미지
    public float fadeDuration = 2f; // 페이드 시간

    void Start()
    {
        fadeImage.gameObject.SetActive(true);
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        Color color = fadeImage.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        fadeImage.gameObject.SetActive(false); // 페이드 종료 후 비활성화
    }
}
