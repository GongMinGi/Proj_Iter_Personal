using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{

    public static PauseMenuManager Instance;    // Singleton 인스턴스

    [SerializeField]
    private GameObject pauseMenuUI;     // Pause Menu UI
    
    private bool isPaused = false;

    private void Awake()
    {
        
        if (Instance == null)       // Singleton 설정
        {

            Instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 전환 시 오브젝트 유지

        }
        else
        {

            Destroy(gameObject);    // 기존 인스턴스가 있으면 삭제

        }

        if (pauseMenuUI != null)
        {

            pauseMenuUI.SetActive(false);   // 게임 시작 시 UI 비활성화

        }
        
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))       // ESC 키로 Pause/Resume 제어
        {

            if (isPaused)
            {

                ResumeGame();

            }
            else
            {

                PauseGame();

            }
                
        }

    }

    public void PauseGame()
    {

        pauseMenuUI.SetActive(true);    // Pause Menu 활성화
        Time.timeScale = 0f;    // 게임 정지
        isPaused = true;

    }

    public void ResumeGame()
    {

        pauseMenuUI.SetActive(false);   // Pause Menu 비활성화
        Time.timeScale = 1f;    // 게임 재개
        isPaused = false;

    }

    public void SaveGame()
    {
        PlayerHealth.instance.SaveData();
        Debug.Log("Game Saved!");

    }

    public void OpenSettings()
    {

        Debug.Log("Settings Menu Opened!");

    }

    public void QuitGame()
    {

        Time.timeScale = 1f;    // TimeScale 초기화
        Debug.Log("Game Quit!");

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

}
