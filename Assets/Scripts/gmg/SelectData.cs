using JetBrains.Annotations;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectData : MonoBehaviour
{
    //비어있는 슬롯을 눌렀을 때 뜨는 창
    public GameObject createViewPort;
    public Text[] slotText;             //데이터를 저장할 때 슬롯의 문구가 바뀌어야 하기 때문에 슬롯의 텍스트를 변수로 가지고 있어야 함
    public Text newPlayerName;           // 새로운 저장 데이터를 만들 때 입력받을 플레이어 이름

    bool[] savefile = new bool[3];

    void Start()
    {
        // 슬롯 별로 저장된 데이터가 존재하는지 판단
        // 저장됬던 경로를 확인해야 함.
        for(int i =0; i< 3; i++)
        {
            //0~2까지 돌면서 파일이 존재하는지 아닌지 bool값으로 반환한다.
            if(File.Exists(DataManager.Instance.path + $"{i}"))
            {
                savefile[i] = true;
                DataManager.Instance.nowSlot = i;   // 데이터가 존재한다면, 현재 슬롯에 해당 슬롯의 번호를 입력
                DataManager.Instance.LoadData();    // 해당 슬롯에 저장된 데이터를 불러와 DataManager의 nowPlayer에 할당
                slotText[i].text = DataManager.Instance.nowPlayer.name; // 불러온 데이터의 플레이어 이름을 슬롯의 텍스트에 적어놓는다.
            }
            else // 파일이 존재하지 않는다면,
            {
                slotText[i].text = "비어 있음";
            }

        }
        //DataManager.Instance.DataClear(); // 실제로 데이터를 반영하려는 것이 아닌 슬롯에 표시만 하려는 것이기 때문에, 전부 불러온 다음엔 초기화해준다.


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //슬롯에 들어갈 함수
    //슬롯이 3개인데 어떻게 알맞게 불러오는가
    //슬롯 버튼 클릭시 호출되는 메서드, 클릭된 슬롯의 번호(number)를 매개변수로 받음
    public void Slot(int number)
    {
        DataManager.Instance.nowSlot = number; // 함수를 실행할 때 입력된 숫자가 나우슬롯에 들어감

        if (savefile[number])
        {
            // 2. 저장된 데이터가 있을 때 => 불러오기 해서 게임씬으로 넘어감
            DataManager.Instance.LoadData();
            GoGame(); // 세이브데이터가 있을 때만 게임 씬으로 바로 넘어간다.

        }
        else  // 1. 저장된 데이터가 없을 때
        {
            createNewPlayer(); // 새 플레이어 생성을 위한 UI 창을 활성화하는 메서드 (빈 슬롯 클릭 시 새 데이터 생성)

        }


    }

    //빈 슬롯을 누르면 캐릭터를 새로 생성시킨다.
    public void createNewPlayer()
    {
        createViewPort.gameObject.SetActive(true);  //플레이어 생성 창을 활성화하여 화면에 표시.
    }


    public void GoGame()
    {
        //게임 씬으로 이동하기 전에 입력을 받으면 이미 저장되어있던 경우 빈 값을 입력하는 것이나 마찬가지이기 때문에 이름이 없어지는 버그가 생긴다.
        if (!savefile[DataManager.Instance.nowSlot]) // 현재 슬롯에 데이터가 저장되어 있지 않다면,
        {
            DataManager.Instance.nowPlayer.name = newPlayerName.text; //슬롯을 만들 때 입력했던 이름을 현재데이터의 이름에 기록한다.
            DataManager.Instance.SaveData();                            // 새 데이터를 파일에 저장
            SceneManager.LoadScene(1); // 인덱스 1에 해당하는 씬(게임 씬)으로 전환
        }
        SceneManager.LoadScene(DataManager.Instance.nowPlayer.nowScene); // 데이터에 저장되어있던 Scene으로 이동

    }



}
