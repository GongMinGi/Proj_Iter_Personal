using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueController : MonoBehaviour
{

    public GameObject dialogueBox;   // 말풍선 이미지
    public Text dialogueText;   // 텍스트

    public string fullText;     // 출력할 전체 텍스트

    public float typingSpeed = 0.05f;    // 타이핑 속도

    private bool isTyping = false;  // 타이핑 중인지 확인
    private bool skipRequested = false;     // 스킵 요청 여부 확인
    public Rigidbody2D playerRigidbody;     // 플레이어 Rigidbody

    [HideInInspector]
    public bool isTalking = false;

    void Start()
    {

        dialogueBox.SetActive(false);    // 초기 상태에서 말풍선 숨기기
        dialogueText.text = "";     // 초기 상태에서 텍스트 숨기기

    }

    public void StartDialogue()
    {

        dialogueBox.SetActive(true); 

        isTalking = true;

        playerRigidbody.linearVelocity = Vector2.zero; 
        playerRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        playerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        StartCoroutine(TypeText());

    }

    void Update()
    {

        if (isTyping && Input.GetKeyDown(KeyCode.Q))
        {

            skipRequested = true;   // 스킵 요청 설정

        }

    }

    IEnumerator TypeText()
    {

        isTyping = true;
        skipRequested = false;
        dialogueText.text = ""; 

        foreach (char c in fullText)
        {

            if (skipRequested)
            {

                dialogueText.text = fullText;   // 전체 텍스트 바로 출력

                break;

            }

            dialogueText.text += c;     // 한 글자씩 출력

            yield return new WaitForSeconds(typingSpeed);

        }

        // 타이핑이 끝났으므로 종료
        isTyping = false;
        skipRequested = false;

        yield return new WaitForSeconds(1f);    // 타이핑 완료 후 대기

        EndDialogue();  // 대화 종료

    }

    public void EndDialogue()
    {

        dialogueBox.SetActive(false);   // 말풍선 숨기기
        dialogueText.text = "";     // 텍스트 숨기기

        isTalking = false;

        playerRigidbody.constraints = RigidbodyConstraints2D.None;  // 이동 제한 해제
        playerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;        // 안하면 캐릭터 뱅글뱅글 돌아감

    }

}
