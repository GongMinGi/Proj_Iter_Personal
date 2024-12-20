using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Rooftop_Begin : MonoBehaviour
{

    public Image blackoutImage;          // 검은색 이미지 (UI)
    public float fadeSpeed = 2f;         // 이미지가 내려가는 속도

    void Start()
    {
        
        if (blackoutImage != null)      // 씬이 시작되면 코루틴 실행
        {

            StartCoroutine(FadeOutEffect());

        }
        else
        {

            Debug.LogError("Blackout Image is not assigned!");      // 블랙아웃 이미지 없으면 이렇게 됨

        }

    }

    private IEnumerator FadeOutEffect()
    {

        RectTransform blackoutRect = blackoutImage.rectTransform;

        Vector3 startPosition = Vector3.zero;       // 초기 위치를 화면 중앙으로 설정
        Vector3 endPosition = new Vector3(0, -Screen.height, 0);        // 화면 아래로 이동

        float elapsedTime = 0f;
        
        while (elapsedTime < 1f / fadeSpeed)        // 이미지가 서서히 아래로 내려가는 효과
        {

            float t = elapsedTime / (1f / fadeSpeed);

            blackoutRect.localPosition = Vector3.Lerp(startPosition, endPosition, t);

            elapsedTime += Time.deltaTime;

            yield return null;

        }
        
        blackoutRect.localPosition = endPosition;       // 위치를 최종 값으로 고정 

        blackoutImage.gameObject.SetActive(false);      // 이미지 비활성화

    }

}