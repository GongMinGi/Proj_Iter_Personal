using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tuto_BlinkUI : MonoBehaviour
{
    public Image tutorialImage; // 반짝임 효과를 적용할 이미지
    public float fadeSpeed = 2f; // 페이드 속도
    public bool useAnyKey = false; // 모든 키 입력 감지 여부

    private Coroutine blinkCoroutine;

    void OnEnable()
    {
        // 활성화될 때마다 반짝임 시작
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        blinkCoroutine = StartCoroutine(BlinkImage());
    }

    public void StartFadeOut()
    {
        // 페이드아웃 실행
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        StartCoroutine(FadeOutImage());
    }

    IEnumerator BlinkImage()
    {
        Color color = tutorialImage.color;

        while (true)
        {
            while (color.a < 1f)
            {
                color.a += fadeSpeed * Time.deltaTime;
                tutorialImage.color = color;
                yield return null;
            }

            while (color.a > 0.2f)
            {
                color.a -= fadeSpeed * Time.deltaTime;
                tutorialImage.color = color;
                yield return null;
            }
        }
    }

    IEnumerator FadeOutImage()
    {
        Color color = tutorialImage.color;

        while (color.a > 0f)
        {
            color.a -= fadeSpeed * Time.deltaTime;
            tutorialImage.color = color;
            yield return null;
        }

        tutorialImage.gameObject.SetActive(false); // 완전히 투명해진 뒤 비활성화
    }
}
