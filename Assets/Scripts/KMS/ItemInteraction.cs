
using UnityEngine;
using System.Collections;

public class ItemInteraction : MonoBehaviour
{

    [SerializeField]
    private PopupController popupController; 

    [SerializeField]
    private Tuto_BlinkUI[] blinkUIs; 

    [SerializeField]
    private GameObject[] UIImages; 

    [SerializeField]
    private bool[] initialActiveStates;     // 초기 활성화 상태 설정

    [HideInInspector]
    public bool isPlayerNearby = false;

    void Start()
    {

        for (int i = 0; i < UIImages.Length; i++)   // UIImages 초기 상태 설정
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

        if (isPlayerNearby && IsInteractionKeyPressed())    // 플레이어가 근처에 있고, 키 입력 감지
        {

            HandleInteraction();

        }

    }

    private bool IsInteractionKeyPressed()
    {

        if (isPlayerNearby)
        {

            foreach (var blinkUI in blinkUIs)    // 모든 blinkUIs의 fadeOutKey를 확인
            {

                if (blinkUI != null && Input.GetKeyDown(blinkUI.fadeOutKey))
                {

                    return true;

                }

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

        foreach (var blinkUI in blinkUIs)   // 모든 튜토리얼 이미지가 페이드아웃을 시작
        {

            if (blinkUI != null)
            {

                blinkUI.StartFadeOut();

            }

        }

        bool allFaded = false;

        while (!allFaded)   // 모든 튜토리얼 이미지의 페이드아웃이 완료될 때까지 대기
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


        if (popupController != null)
        {

            popupController.ShowPopup();       // 페이드아웃 완료 후 팝업 표시

        }
        
        gameObject.SetActive(false);        // 아이템 오브젝트 비활성화

    }

    private void DisableObject()
    {

        if (popupController != null)
        {

            popupController.ShowPopup();    // 팝업 표시

        }

        gameObject.SetActive(false);     // 아이템 비활성화

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

        if (other.CompareTag("Player"))  // 플레이어가 범위를 벗어나면
        {

            isPlayerNearby = false;

            for (int i = 0; i < UIImages.Length; i++)
            {

                if (UIImages[i] != null && i < blinkUIs.Length && blinkUIs[i] != null)
                {

                    blinkUIs[i].StartFadeOut();  // 페이드아웃 시작

                    StartCoroutine(DisableUIAfterFadeOut(UIImages[i], blinkUIs[i]));    // 페이드아웃 완료 후 비활성화

                }
                else if (UIImages[i] != null)
                {

                    UIImages[i].SetActive(false);   // blinkUIs가 없으면 즉시 비활성화

                }

            }

        }

    }

    private IEnumerator DisableUIAfterFadeOut(GameObject uiImage, Tuto_BlinkUI blinkUI)
    {

        while (!blinkUI.isFadedOut)     // 페이드아웃 완료 상태 확인
        {

            yield return null;  // 완료될 때까지 대기

        }

        uiImage.SetActive(false);   // 페이드아웃 완료 후 비활성화

    }

}
