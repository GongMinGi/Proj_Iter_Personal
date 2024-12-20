using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tuto_BlinkUI : MonoBehaviour
{

    public Image tutorialImage;     // 깜박임/페이드아웃 대상 이미지

    public float fadeSpeed = 2f;    // 페이드 속도

    public KeyCode fadeOutKey = KeyCode.E;  // 기본값은 E 키

    public bool useAnyKey = false;  // 아무 키나 입력으로 페이드아웃
    public bool isFadedOut = false;     // 페이드아웃 완료 여부

    private Coroutine blinkCoroutine;

    void OnEnable()
    {

        isFadedOut = false;     // 활성화될 때 페이드아웃 상태 초기화

        if (blinkCoroutine != null)
        {

            StopCoroutine(blinkCoroutine);

        }

        blinkCoroutine = StartCoroutine(BlinkImage());

    }

    void Update()
    {
        
        if ((useAnyKey && Input.anyKeyDown) || Input.GetKeyDown(fadeOutKey))        // 키 입력 감지
        {

            StartFadeOut();

        }

    }

    public void StartFadeOut()
    {

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

                color.a += fadeSpeed * Time.deltaTime;      // 투명 -> 불투명
                tutorialImage.color = color;

                yield return null;

            }

            
            while (color.a > 0.2f)
            {   

                color.a -= fadeSpeed * Time.deltaTime;      // 불투명 -> 투명
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

            color.a -= fadeSpeed * Time.deltaTime;      // 알파 값 감소
            tutorialImage.color = color;

            yield return null;

        }

        // 페이드아웃 완료
        tutorialImage.gameObject.SetActive(false);
        isFadedOut = true;  // 페이드아웃 완료 상태 업데이트

    }

}
