
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tuto_BlinkUI : MonoBehaviour
{

    public Image tutorialImage;      // 깜박임/페이드아웃 대상 이미지

    public float fadeSpeed = 2f;     // 페이드 속도

    public KeyCode fadeOutKey = KeyCode.E;  // 기본값은 E 키

    public bool useAnyKey = false;      // 아무 키나 입력으로 페이드아웃
    public bool isFadedOut = false;      // 페이드아웃 완료 여부

    private Coroutine activeCoroutine;   // 현재 실행 중인 코루틴


    void OnEnable()
    {

        isFadedOut = false;  // 활성화될 때 페이드아웃 상태 초기화

        StopActiveCoroutine();   // 기존 실행 중인 코루틴 정리

        activeCoroutine = StartCoroutine(FadeInImage());    // 페이드인 시작

    }


    public void StartFadeOut()
    {

        StopActiveCoroutine();   // 기존 실행 중인 코루틴 정리

        activeCoroutine = StartCoroutine(FadeOutImage());

    }


    private void StopActiveCoroutine()
    {

        if (activeCoroutine != null)
        {

            StopCoroutine(activeCoroutine);     // 현재 실행 중인 코루틴 중지

            activeCoroutine = null;

        }

    }


    IEnumerator FadeInImage()
    {

        Color color = tutorialImage.color;
        color.a = 0f;
        tutorialImage.color = color;


        while (color.a < 1f)
        {

            color.a += fadeSpeed * Time.deltaTime;
            tutorialImage.color = color;

            yield return null;

        }

        color.a = 1f;   // 알파 값 고정
        tutorialImage.color = color;

        activeCoroutine = StartCoroutine(BlinkImage());     // 페이드인 완료 후 깜박임 효과 시작

    }


    IEnumerator BlinkImage()
    {

        float timer = 0.8f / fadeSpeed;     // Mathf.PingPong이 최대값을 반환하는 시점

        while (true)
        {

            float alpha = Mathf.PingPong(timer * fadeSpeed, 0.8f) + 0.2f;
            
            tutorialImage.color = new Color(tutorialImage.color.r, tutorialImage.color.g, tutorialImage.color.b, alpha);

            timer += Time.deltaTime;

            yield return null;

        }

    }



    IEnumerator FadeOutImage()
    {

        Color color = tutorialImage.color;


        while (color.a > 0f)
        {

            color.a -= fadeSpeed * Time.deltaTime;  // 알파 값 감소
            tutorialImage.color = color;

            yield return null;

        }

        tutorialImage.gameObject.SetActive(false);  // 페이드아웃 완료 후 비활성화

        isFadedOut = true;  // 페이드아웃 완료 상태 업데이트

    }

}
