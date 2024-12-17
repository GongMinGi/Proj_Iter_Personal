using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tuto_BlinkUI : MonoBehaviour
{

    public Image tutorialImage;     // 반짝이기 효과를 적용할 튜토리얼 이미지
    
    public float fadeSpeed = 2f;    // 페이드 효과 속도

    public KeyCode toggleKey = KeyCode.E; // 특정 키 (Inspector에서 변경 가능)

    public bool useAnyKey = false;  // 모든 키 입력을 감지할지 여부 설정

    private bool isFadingOut = false; // 페이드아웃 중인지 확인하는 변수

    private Coroutine blinkCoroutine; // 반짝이기 효과 코루틴의 참조

    void Start()
    {

        blinkCoroutine = StartCoroutine(BlinkImage());      // 게임 시작 시 반짝이기 효과 코루틴 실행

    }

    void Update()
    {

        if (!isFadingOut && ((useAnyKey && Input.anyKeyDown) || Input.GetKeyDown(toggleKey)))       // 모든 키 입력을 감지하거나, 특정 키만 감지
        {
            
            if (blinkCoroutine != null)
            {

                StopCoroutine(blinkCoroutine);      // 기존의 반짝이기 효과 코루틴 중지

            }

            StartCoroutine(FadeOutImage());     // 페이드아웃 효과 실행

        }

    }

    IEnumerator BlinkImage()
    {

        Color color = tutorialImage.color; // 튜토리얼 이미지의 초기 색상 가져오기

        while (true) // 무한 루프
        {

            while (color.a < 1f)        // 페이드 인: 알파 값을 점차 증가시켜 완전 불투명(1)으로 만듦
            {

                color.a += fadeSpeed * Time.deltaTime; // 투명도 증가
                tutorialImage.color = color;          // 변경된 색상 적용

                yield return null;                   // 다음 프레임까지 대기

            }

            while (color.a > 0.2f)      // 페이드 아웃: 알파 값을 감소시켜 반투명 상태(0.2)로 만듦
            {

                color.a -= fadeSpeed * Time.deltaTime; // 투명도 감소
                tutorialImage.color = color;          // 변경된 색상 적용

                yield return null;                   // 다음 프레임까지 대기

            }

        }

    }

    IEnumerator FadeOutImage()
    {

        isFadingOut = true; // 페이드아웃 상태 설정

        Color color = tutorialImage.color; // 현재 색상 가져오기
        
        while (color.a > 0f)        // 페이드아웃: 알파 값을 점차 감소시켜 완전 투명(0)으로 만듦
        {

            color.a -= fadeSpeed * Time.deltaTime; // 투명도 감소
            tutorialImage.color = color;          // 변경된 색상 적용

            yield return null;                   // 다음 프레임까지 대기

        }
        
        tutorialImage.gameObject.SetActive(false);      // 완전히 투명해진 후 오브젝트를 비활성화

    }

}
