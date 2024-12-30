using UnityEngine;
using UnityEngine.UI;

public class BarController : MonoBehaviour
{
    public Image chargeBar;          // 게이지 바 (UI 이미지)
    public float chargeDuration = 2f; // 게이지가 완전히 차는 데 걸리는 시간 (초)
    public float fadeOutDuration = 1f; // 게이지 바가 사라지는 페이드아웃 시간 (초)

    private bool isCharging = false; // 충전 중인지 확인
    private bool isFadingOut = false; // 페이드아웃 중인지 확인
    private bool isInTrigger = false; // 트리거 범위 내에 있는지 확인
    private float chargeTimer = 0f;  // 충전 시간을 추적

    void Start()
    {
        if (chargeBar != null)
        {
            chargeBar.fillAmount = 0;  // 게이지 초기화
            SetChargeBarAlpha(0); // 시작 시 알파값 0으로 설정 (안 보이게)
        }
    }

    void Update()
    {
        if (isInTrigger && !isFadingOut)
        {
            // 마우스 좌클릭 유지 시 충전 시작
            if (Input.GetMouseButton(0))
            {
                if (!isCharging)
                {
                    StartCharging(); // 충전 시작
                }

                UpdateCharging(); // 충전 업데이트
            }
            else if (isCharging)
            {
                StopCharging(); // 충전 취소
            }
        }
    }

    private void StartCharging()
    {
        isCharging = true;
        chargeTimer = 0f; // 타이머 초기화
        if (chargeBar != null)
        {
            chargeBar.fillAmount = 0;
        }
    }

    private void UpdateCharging()
    {
        if (chargeBar != null)
        {
            chargeTimer += Time.deltaTime;
            chargeBar.fillAmount = chargeTimer / chargeDuration; // 게이지 채우기

            if (chargeTimer >= chargeDuration)
            {
                CompleteCharging(); // 충전 완료
            }
        }
    }

    private void StopCharging()
    {
        isCharging = false;
        if (chargeBar != null)
        {
            chargeBar.fillAmount = 0; // 게이지 초기화
        }
    }

    private void CompleteCharging()
    {
        isCharging = false; // 충전 상태 종료
        if (chargeBar != null)
        {
            chargeBar.fillAmount = 1; // 게이지를 완전히 채움
            Debug.Log("Charging Complete!"); // 디버그 메시지 출력

            // 페이드아웃 시작
            StartCoroutine(FadeOutChargeBar());
        }
    }

    private System.Collections.IEnumerator FadeOutChargeBar()
    {
        isFadingOut = true;
        float fadeTimer = 0f;
        Color color = chargeBar.color;

        while (fadeTimer < fadeOutDuration)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, fadeTimer / fadeOutDuration); // 알파값 서서히 감소
            color.a = alpha;
            chargeBar.color = color;
            yield return null;
        }

        color.a = 0; // 완전히 투명 설정
        chargeBar.color = color;

        isFadingOut = false;

        // 게이지 초기화
        chargeTimer = 0f;
        chargeBar.fillAmount = 0;
        Debug.Log("Charge Bar faded out.");
    }

    private void SetChargeBarAlpha(float alpha)
    {
        if (chargeBar != null)
        {
            Color color = chargeBar.color;
            color.a = alpha; // 알파값 변경
            chargeBar.color = color;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isFadingOut)
        {
            isInTrigger = true;
            SetChargeBarAlpha(1); // 게이지 바 보이기
            Debug.Log("Player entered trigger. Charge bar visible.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInTrigger = false;
            if (!isCharging && !isFadingOut) // 충전 중이거나 페이드아웃 중이 아닐 경우에만 숨기기
            {
                SetChargeBarAlpha(0); // 게이지 바 숨기기
                Debug.Log("Player exited trigger. Charge bar hidden.");
            }
        }
    }
}
