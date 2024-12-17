using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Rooftop_Begin : MonoBehaviour
{
    public Image blackoutImage;          // 검은색 이미지 (UI)
    public float fadeSpeed = 2f;         // 이미지가 내려가는 속도

    void Start()
    {
        // 씬이 시작되면 코루틴 실행
        if (blackoutImage != null)
        {
            StartCoroutine(FadeOutEffect());
        }
        else
        {
            Debug.LogError("Blackout Image is not assigned!");
        }
    }

    private IEnumerator FadeOutEffect()
    {
        RectTransform blackoutRect = blackoutImage.rectTransform;

        // 초기 위치를 화면 중앙으로 설정
        Vector3 startPosition = Vector3.zero; // 화면 중앙
        Vector3 endPosition = new Vector3(0, -Screen.height, 0); // 화면 아래로 이동

        float elapsedTime = 0f;

        // 이미지가 서서히 아래로 내려가는 효과
        while (elapsedTime < 1f / fadeSpeed)
        {
            float t = elapsedTime / (1f / fadeSpeed);
            blackoutRect.localPosition = Vector3.Lerp(startPosition, endPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 위치를 최종 값으로 고정하고 이미지 비활성화
        blackoutRect.localPosition = endPosition;
        blackoutImage.gameObject.SetActive(false);
        Debug.Log("Blackout Image has faded out.");
    }
}
