using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; //1. 정적메모리에 담기위한 instance를 변수로 선언

    [Header("#BGM")] //3.
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;

    [Header("#SFX")] //3.
    [Header("#SFX")] //3.
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;//4. 채널 개수 변수 선언 for 다량의 효과음들
    AudioSource[] sfxPlayers;
    int channelIndex;

    public enum Sfx { attack3, attacksuccess, Cat,ChargeattackCharging, ChargeAttackRelease1_1, dash1, Falling, glide, Iamattattacked, jump1,Mosquito,RockCrush,Snail,SpiderNo4,Zombie2}

    void Awake() //5. 채널 인덱스 선언-채널이 몇 번인가?
    {
        instance = this; //2
        Init();
    }

    void Init()
    {

        //6. 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false; //7. 처음 키자마자 나오면 곤란해
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;

        //효과음 플레이어 초기화+
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].volume = sfxVolume;
        }
    }
    public void PlayBgm(bool isPlay)
    {
        if (isPlay)
        {
            bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }
    public void PlaySfx(Sfx sfx)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
}
//배경음은 하나 , 효과음은 채널이 여러개



// Fin 여기다가 원하는 곳의 메니저? 파일들에 다음의 코드를 넣기

//Fin1 우선,  효과음의 경우 gamemanager 같은 script의 메소드? 에다가
//AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);//여기서 Select는 음원이름이다 //24분 근처로 보기
//Fin1-1 몹들같은 거의 사망 사운드는 게임 종료 시에는 나지 않도록 조건 추가하기
//Fin1-2 두개 이상 소리 랜덤 동작은 생략

//Fin2. AudioManager.instance.PlayBgm(true); 시작부근에다가
//Fin2. AudioManager.instance.PlayBgm(false);끝나는 부근에다가
//Fin2-1. highpass filter이런거나 ui있을때 bgm멈추거나 그러는거 일단 pass //33분 부근?
