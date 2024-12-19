using UnityEngine;
using System.Collections;

public class ItemInteraction : MonoBehaviour
{
    [SerializeField] private PopupController popupController; // 팝업 UI 제어
    [SerializeField] private Tuto_BlinkUI[] blinkUIs; // 여러 개의 <PressEforItem> 제어
    [SerializeField] private GameObject[] pressEImages; // 여러 개의 <PressEforItem> 오브젝트
    [SerializeField] private bool[] initialActiveStates; // 초기 활성화 여부 설정
    [SerializeField] private KeyCode interactionKey = KeyCode.E; // 사용자 설정 가능 키

    private bool isPlayerNearby = false; // 플레이어가 근처에 있는지 확인

    void Start()
    {
        // 각 Image의 초기 상태를 설정
        for (int i = 0; i < pressEImages.Length; i++)
        {
            if (pressEImages[i] != null)
            {
                // initialActiveStates의 값에 따라 활성화/비활성화 설정
                bool shouldActivate = i < initialActiveStates.Length && initialActiveStates[i];
                pressEImages[i].SetActive(shouldActivate);

                // 활성화된 경우 반짝임 효과도 시작
                if (shouldActivate && i < blinkUIs.Length && blinkUIs[i] != null)
                {
                    blinkUIs[i].enabled = true;
                }
                else if (i < blinkUIs.Length && blinkUIs[i] != null)
                {
                    blinkUIs[i].enabled = false;
                }
            }
        }
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(interactionKey))
        {
            HandleInteraction();
        }
    }

    private void HandleInteraction()
    {
        if (blinkUIs.Length > 0)
        {
            // 모든 Press E Image의 페이드아웃 실행
            StartCoroutine(HandleFadeAndDisable());
        }
        else
        {
            // blinkUI가 없으면 바로 비활성화 처리
            DisableObject();
        }
    }

    private IEnumerator HandleFadeAndDisable()
    {
        // 모든 이미지의 페이드아웃 실행
        foreach (var blinkUI in blinkUIs)
        {
            if (blinkUI != null)
            {
                blinkUI.StartFadeOut();
            }
        }

        // 모든 이미지의 페이드아웃이 끝날 때까지 대기
        bool allFaded = false;
        while (!allFaded)
        {
            allFaded = true;
            foreach (var blinkUI in blinkUIs)
            {
                if (blinkUI != null && blinkUI.tutorialImage.color.a > 0f)
                {
                    allFaded = false;
                    break;
                }
            }
            yield return null;
        }

        // 페이드아웃 완료 후 <ParticleForUmbrella> 비활성화
        DisableObject();
    }

    private void DisableObject()
    {
        if (popupController != null)
        {
            popupController.ShowPopup(); // 팝업 UI 표시
        }

        gameObject.SetActive(false); // <ParticleForUmbrella> 비활성화
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;

            // 모든 Press E 이미지를 활성화하고 반짝임 시작
            for (int i = 0; i < pressEImages.Length; i++)
            {
                if (pressEImages[i] != null)
                {
                    pressEImages[i].SetActive(true);
                }

                if (i < blinkUIs.Length && blinkUIs[i] != null)
                {
                    blinkUIs[i].enabled = true;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;

            // 모든 Press E 이미지를 비활성화하고 반짝임 중단
            for (int i = 0; i < pressEImages.Length; i++)
            {
                if (pressEImages[i] != null)
                {
                    pressEImages[i].SetActive(false);
                }

                if (i < blinkUIs.Length && blinkUIs[i] != null)
                {
                    blinkUIs[i].enabled = false;
                }
            }
        }
    }
}
