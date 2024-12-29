using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundManager : MonoBehaviour
{
    [Header("배경 타입")]
    public GameObject backgroundCity;
    public GameObject backgroundLab;

    [Header("Scene to Background Mapping")]
    public Dictionary<string, string> sceneBackgroundMap = new Dictionary<string, string>
    {
        { "LabABasementScene", "Lab" },
        { "CharacterAndTile", "City" },
        { "LabARooftopScene", "City" },
    };

    private void Awake()
    {
        // 씬 전환 시 오브젝트 유지
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        SetBackgroundForCurrentScene();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");
        SetBackgroundForCurrentScene();
    }

    private void SetBackgroundForCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (sceneBackgroundMap.TryGetValue(currentScene, out string backgroundType))
        {
            if (backgroundType == "City")
            {
                backgroundCity.SetActive(true);
                backgroundLab.SetActive(false);
            }
            else if (backgroundType == "Lab")
            {
                backgroundCity.SetActive(false);
                backgroundLab.SetActive(true);
            }
        }
        else
        {
            Debug.LogWarning($"No background mapping found for scene: {currentScene}");
            backgroundCity.SetActive(false);
            backgroundLab.SetActive(false);
        }
    }
}