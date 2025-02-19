using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundManager : MonoBehaviour
{
    [Header("��� Ÿ��")]
    public GameObject backgroundCity;
    public GameObject backgroundLab;


    //�� �̸��� ���� ��� Ÿ���� �����ϴ� Dictinary
    [Header("Scene to Background Mapping")]
    public Dictionary<string, string> sceneBackgroundMap = new Dictionary<string, string>
    {
        { "LabABasementScene", "Lab" },
        { "CharacterAndTile", "City" },
        { "LabARooftopScene", "City" },
    };

    private void Awake()
    {
        // �� ��ȯ �� ������Ʈ ����
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        //���� �ε�� �븶�� OnsceneLoaded �Լ��� ȣ���ϵ��� �̺�Ʈ�� ����
        //�̺�Ʈ�� ����: Ư�� �̺�Ʈ�� ����ɶ� ���ϴ� �Լ��� ����ǵ��� �����ϴ� ��.
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

    // ���� �ε� �� �Ŀ� ��µǴ� �ݹ� �Լ�
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}"); // �ε�� ���� �̸��� ���
        SetBackgroundForCurrentScene(); // ���� �ε�� ���� �°� ��� ����
    }


    //���� Ȱ��ȭ�� ���� ���� ����� Ȱ��/��Ȱ��ȭ�ϴ� �Լ�
    private void SetBackgroundForCurrentScene()
    {
        //���� Ȱ��ȭ�� ���� �̸��� ������
        string currentScene = SceneManager.GetActiveScene().name;

        // sceneBackgroundMap ���� ���� �� �̸��� �ش��ϴ� ���Ÿ���� ã�´�.
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