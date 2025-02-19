using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundManager : MonoBehaviour
{
    [Header("배경 타입")]
    public GameObject backgroundCity;
    public GameObject backgroundLab;


    //씬 이름에 따른 배경 타입을 매핑하는 Dictinary
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
        //씬이 로드될 대마다 OnsceneLoaded 함수를 호출하도록 이벤트에 구독
        //이벤트에 구독: 특정 이벤트가 실행될때 원하는 함수가 실행되도록 설정하는 것.
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

    // 씬이 로드 된 후에 출력되는 콜백 함수
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}"); // 로드된 씬의 이름을 출력
        SetBackgroundForCurrentScene(); // 새로 로드된 씬에 맞게 배경 갱신
    }


    //현재 활성화된 씬에 맞춰 배경을 활성/비활성화하는 함수
    private void SetBackgroundForCurrentScene()
    {
        //현재 활성화된 씬의 이름을 가져옴
        string currentScene = SceneManager.GetActiveScene().name;

        // sceneBackgroundMap 에서 현재 씬 이름에 해당하는 배경타입을 찾는다.
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