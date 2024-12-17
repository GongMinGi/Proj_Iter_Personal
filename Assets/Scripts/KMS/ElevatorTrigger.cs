using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{

    [SerializeField] 
    private GameObject popupImage;  // Inspector에서 UI 팝업 이미지 연결

    private void Start()
    {

        if (popupImage != null)     // 시작 시 팝업 이미지를 비활성화
        {

            popupImage.SetActive(false);

        }

    }

    private void OnCollisionEnter2D(Collision2D collision)      // 플레이어가 플랫폼에 올라왔는지 확인
    {
        
        if (collision.gameObject.CompareTag("Player"))      
        {

            ShowPopup();

        }

    }

    private void OnCollisionExit2D(Collision2D collision)       // 플레이어가 플랫폼에서 내려갔는지 확인
    {
        
        if (collision.gameObject.CompareTag("Player"))      
        {

            HidePopup();

        }

    }

    private void ShowPopup()
    {

        if (popupImage != null)
        {

            popupImage.SetActive(true);     // UI 팝업 활성화

        }

    }

    private void HidePopup()
    {

        if (popupImage != null)
        {

            popupImage.SetActive(false);    // UI 팝업 비활성화

        }

    }

}
