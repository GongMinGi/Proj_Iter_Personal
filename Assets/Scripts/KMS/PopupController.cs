using UnityEngine;

public class PopupController : MonoBehaviour
{

    private bool isPopupActive = false;

    void Start()
    {

        gameObject.SetActive(false);

    }

    void Update()
    {
        
        if (isPopupActive && Input.anyKeyDown)      // 팝업이 활성화된 상태에서 아무 키나 누르면 팝업 비활성화
        {

            ClosePopup();

        }

    }

    public void ShowPopup()
    {

        gameObject.SetActive(true);

        isPopupActive = true;

    }

    public void ClosePopup()
    {

        gameObject.SetActive(false);

        isPopupActive = false;

    }

}
