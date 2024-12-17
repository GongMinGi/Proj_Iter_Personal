using UnityEngine;
using UnityEngine.UI; // UI 관련 클래스 포함

public class ItemInteraction : MonoBehaviour
{
    [SerializeField]
    private GameObject itemUIPopup; // UI 팝업 패널
    private bool isPlayerNearby = false; // 플레이어가 근처에 있는지 체크
    private bool isUIPopupActive = false; // UI 팝업 상태를 체크

    void Start()
    {
        // UI 패널을 비활성화 상태로 시작
        if (itemUIPopup != null)
            itemUIPopup.SetActive(false);
    }

    void Update()
    {
        // UI 팝업이 활성화된 상태에서 아무 키나 누르면 UI 닫기
        if (isUIPopupActive)
        {

            if (Input.anyKeyDown)
            {

                CloseUIPopup();

            }


        }

        // 플레이어가 아이템 근처에 있을 때 E 키를 누르면 실행
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            // 아이템을 숨기고 UI 팝업 표시
            HideItem();
        }

    }

    private void HideItem()
    {
        // 아이템을 비활성화 (SetActive(false))
        gameObject.SetActive(false);

        // UI 팝업 활성화
        if (itemUIPopup != null)
        {
            itemUIPopup.SetActive(true);
            isUIPopupActive = true; // UI 팝업이 활성화됨
        }

        Debug.Log("Item picked up! UI Popup displayed.");
    }

    private void CloseUIPopup()
    {
        // UI 팝업 비활성화
        if (itemUIPopup != null)
        {
            itemUIPopup.SetActive(false);
            isUIPopupActive = false; // UI 팝업 상태 비활성화
        }

        Debug.Log("UI Popup closed.");
    }

    // 플레이어가 Trigger Collider에 들어왔을 때
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true; // 플레이어가 근처에 있음
            Debug.Log("Press 'E' to pick up the item!");
        }
    }

    // 플레이어가 Trigger Collider에서 나갔을 때
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false; // 플레이어가 범위를 벗어남
        }
    }
}
