using UnityEngine;

public class ItemInteraction : MonoBehaviour
{

    [SerializeField]
    private PopupController popupController;
    private bool isPlayerNearby = false;        // 플레이어가 근처에 있는지 체크

    void Update()
    {
        
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))      // 플레이어가 아이템 근처에 있을 때 E 키를 누르면 실행
        {

            HideItem();

        }

    }

    private void HideItem()
    {
        
        gameObject.SetActive(false);        // 아이템을 비활성화

        if (popupController != null)
        {

            popupController.ShowPopup();        // UI 팝업 활성화

        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {

            isPlayerNearby = true;

        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {

            isPlayerNearby = false;

        }

    }

}
