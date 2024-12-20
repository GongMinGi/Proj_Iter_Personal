using UnityEngine;

public class PopupController : MonoBehaviour
{

    [SerializeField]
    private KeyCode closeKey = KeyCode.Q;

    private bool isPopupActive = false;

    public bool useAnyKey = false;

    void Start()
    {

        gameObject.SetActive(false);

    }

    void Update()
    {
        
        if (isPopupActive && ((useAnyKey && Input.anyKeyDown) || Input.GetKeyDown(closeKey)))  
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
