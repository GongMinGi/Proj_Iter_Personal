using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundManager : MonoBehaviour
{
    [Header("배경 타입")]
    public GameObject backgroundCity; // 배경 이미지 타입 A
    public GameObject backgroundLab; // 배경 이미지 타입 B

    [Header("Scene to Background Mapping")]
    public Dictionary<string, string> sceneBackgroundMap = new Dictionary<string, string>
    {
        { "LabABasementScene", "Lab" },
        { "CharacterAndTile", "City" },
        { "LabARooftopScene", "City" },
    };

    private void Start()
    {
        SetBackgroundForCurrentScene();
    }

    private void SetBackgroundForCurrentScene()
    {
        // 현재 씬 이름 가져오기
        string currentScene = SceneManager.GetActiveScene().name;

        // 씬에 대한 배경 이미지 타입 확인
        if (sceneBackgroundMap.TryGetValue(currentScene, out string backgroundType))
        {
            // 배경 타입에 따라 활성화/비활성화 설정
            if (backgroundType == "TypeA")
            {
                backgroundCity.SetActive(true);
                backgroundLab.SetActive(false);
            }
            else if (backgroundType == "TypeB")
            {
                backgroundCity.SetActive(false);
                backgroundLab.SetActive(true);
            }
        }
        else
        {
            Debug.LogWarning($"No background mapping found for scene: {currentScene}");
            // 기본값으로 모든 배경 비활성화
            backgroundCity.SetActive(false);
            backgroundLab.SetActive(false);
        }
    }
}
