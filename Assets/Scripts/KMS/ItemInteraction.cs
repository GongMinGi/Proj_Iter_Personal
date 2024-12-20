using UnityEngine;
using System.Collections;

public class ItemInteraction : MonoBehaviour
{
    [SerializeField] 
    private PopupController popupController;     // 팝업 관리 스크립트

    [SerializeField] 
    private Tuto_BlinkUI[] blinkUIs;    // 튜토리얼 UI 스크립트 배열

    [SerializeField] 
    private GameObject[] UIImages;   // UI 이미지 배열

    [SerializeField] 
    private bool[] initialActiveStates;  // 초기 활성화 상태 설정

    private bool isPlayerNearby = false;

    void Start()
    {
        
        for (int i = 0; i < UIImages.Length; i++)       // UIImages 초기 상태 설정
        {

            if (UIImages[i] != null)
            {

                bool shouldActivate = i < initialActiveStates.Length && initialActiveStates[i];

                UIImages[i].SetActive(shouldActivate);

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
        
        if (isPlayerNearby && IsInteractionKeyPressed())        // 플레이어가 근처에 있고, blinkUIs의 fadeOutKey 중 하나를 누르면 상호작용 실행
        {

            HandleInteraction();

        }

    }

    private bool IsInteractionKeyPressed()
    {
        
        foreach (var blinkUI in blinkUIs)       // 모든 blinkUIs의 fadeOutKey를 확인
        {

            if (blinkUI != null && Input.GetKeyDown(blinkUI.fadeOutKey))
            {

                return true;

            }

        }

        return false;

    }

    private void HandleInteraction()
    {

        if (blinkUIs.Length > 0)
        {

            StartCoroutine(HandleFadeAndDisable());

        }
        else
        {

            DisableObject();

        }

    }

    private IEnumerator HandleFadeAndDisable()
    {
        
        foreach (var blinkUI in blinkUIs)       // 모든 튜토리얼 이미지가 페이드아웃을 시작
        {

            if (blinkUI != null)
            {

                blinkUI.StartFadeOut();

            }

        }
        
        bool allFaded = false;      

        while (!allFaded)       // 모든 튜토리얼 이미지의 페이드아웃이 완료될 때까지 대기
        {

            allFaded = true;

            foreach (var blinkUI in blinkUIs)
            {

                if (blinkUI != null && !blinkUI.isFadedOut)
                {

                    allFaded = false;

                    break;

                }

            }

            yield return null;  // 다음 프레임까지 대기

        }

        // 페이드아웃 완료 후 팝업 표시
        if (popupController != null)
        {
            popupController.ShowPopup();
        }

        // 아이템 오브젝트 비활성화
        gameObject.SetActive(false);
    }

    private void DisableObject()
    {
        
        if (popupController != null)
        {

            popupController.ShowPopup();        // 팝업 표시

        }
        
        gameObject.SetActive(false);        // 아이템 비활성화

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player"))     // 플레이어가 범위에 들어오면 UI 활성화
        {

            isPlayerNearby = true;

            for (int i = 0; i < UIImages.Length; i++)
            {

                if (UIImages[i] != null)
                {

                    UIImages[i].SetActive(true);

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
        
        if (other.CompareTag("Player"))     // 플레이어가 범위를 벗어나면 UI 비활성화
        {

            isPlayerNearby = false;

            for (int i = 0; i < UIImages.Length; i++)
            {

                if (UIImages[i] != null)
                {

                    UIImages[i].SetActive(false);

                }

                if (i < blinkUIs.Length && blinkUIs[i] != null)
                {

                    blinkUIs[i].enabled = false;

                }

            }

        }

    }

}
