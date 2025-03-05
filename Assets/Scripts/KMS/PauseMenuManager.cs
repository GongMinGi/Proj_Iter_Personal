using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{

    public static PauseMenuManager Instance;    // Singleton �ν��Ͻ�

    [SerializeField]
    private GameObject pauseMenuUI;     // Pause Menu UI
    
    private bool isPaused = false;

    private void Awake()
    {
        
        if (Instance == null)       // Singleton ����
        {

            Instance = this;
            DontDestroyOnLoad(gameObject);  // �� ��ȯ �� ������Ʈ ����

        }
        else
        {

            Destroy(gameObject);    // ���� �ν��Ͻ��� ������ ����

        }

        if (pauseMenuUI != null)
        {

            pauseMenuUI.SetActive(false);   // ���� ���� �� UI ��Ȱ��ȭ

        }
        
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))       // ESC Ű�� Pause/Resume ����
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

        pauseMenuUI.SetActive(true);    // Pause Menu Ȱ��ȭ
        Time.timeScale = 0f;    // ���� ����
        isPaused = true;

    }

    public void ResumeGame()
    {

        pauseMenuUI.SetActive(false);   // Pause Menu ��Ȱ��ȭ
        Time.timeScale = 1f;    // ���� �簳
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

        Time.timeScale = 1f;    // TimeScale �ʱ�ȭ
        Debug.Log("Game Quit!");

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

}
