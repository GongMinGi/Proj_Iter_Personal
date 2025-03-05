using Unity.VisualScripting;
using UnityEngine;
using System.IO; //어딘가로 넣거나 가져올때 쓰임 input /output

/* 저장 방법
 *  1. 저장할 데이터가 존재
 *  2. 데이털르 제이슨으로 변환
 *  3. 제이슨을 외부에 저장
 *  
 * 불러오기
 *  1. 외부에 저장할 제이슨을 가져옴
 *  2. 제이슨을 데이터 형태로 변환
 *  3. 불러온 데이터를 사용
 */


public class PlayerData
{
    // 이름, 레벨, 코인, 착용중인 무기
    public string nowScene;
    public string name;
    public int curHealth =15;   // 게임 시작 초기값을 설정
    public int maxHealth = 15;
    public Vector3 playerPos;

}


//데이터 관리 및 저장/불러오기 기능을 담당하는 Monobehaviour 클래스
public class DataManager : MonoBehaviour
{
    //DataManager의 싱글톤 인스턴스 (프로젝트 내에서 하나만 존재하게 함)
    public static DataManager Instance;
    
    // 현재 플레이어 데이터를 저장할 변수( playerData 타입의 인스턴스)
    public PlayerData nowPlayer = new PlayerData();

    public string path;                //저장파일의 경로를 저장할 변수
    //string filename = "save";   //저장파일의 이름을 지정 (확장자 없이 단순 이름), 현재는 아래의 path변수에 save가 직접 들어가있다
    public int nowSlot;         //파일 이름이 슬롯마다 같으려면, nowSlot 변수를 이용해서 현재 저장에 사용하는 슬롯 번호를 매겨준다.

    // 스크립트 인스턴스가 활성화 될 때 호출
    private void Awake()
    {
        //애플리케이션의 영구 데이터 저장 경로를 가져와 path 변수에 저장
        //persistentDataPath는 플랫폼에 맞는 경로를 제공 ( 유니티의 경우 프로젝트 폴더)
        path = Application.persistentDataPath + "/save"; // + "/"를 붙여야 jsonutility를 통해 파일이름을 설정할 수 있다.


        //싱글톤 인스턴스가 아직 할당되지 않앗다면 현재 인스턴스 할당
        if(Instance == null )
        {
            Instance = this;
        }
        //이미 다른 인스턴스가 존재한다면, 중복을 제거하기 위해 기존 인스턴스의 게임 오브젝트를 파괴
        else if(Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        DontDestroyOnLoad(Instance);
    }


    void Start()
    {

    }

    //현재 플레이어의 정보를 제이슨으로 바꾸고 저장
    public void SaveData()
    {
        string data = JsonUtility.ToJson(nowPlayer); // json은 string이기 때문에 문자열로 받을 수 있음 
        File.WriteAllText(path + nowSlot.ToString(), data);   //저장하고자 하는 경로에 +를 붙여서 파일의 이름을 설정할 수 있다.
        // + nowSlot을 햇기 때문에 이름이 같다고 하더라도 뒤에 0, 1, 2 가 붙기에 구분할 수 있다.
    }

    public void LoadData()
    {
        string data = File.ReadAllText(path + nowSlot.ToString()); // 경로와 불러올 파일의 이름을 매개로 데이터를 가져온다.
        nowPlayer = JsonUtility.FromJson<PlayerData>(data);
    }


    public void DataClear()
    {
        nowSlot = -1; // 슬롯 번호는 -가 될 수 없으므로 -로 초기화
        nowPlayer = new PlayerData(); //플레이어 정보 초기화
    }
}
