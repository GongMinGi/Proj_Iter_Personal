using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossIntroController : MonoBehaviour
{
    [Header("ī�޶� �� ���")]
    public Camera mainCamera;
    public Transform bossTransform;
    public Transform playerTransform;
    private Vector3 bossPos;

    [Header("ī�޶� �̵� ����")]
    public float cameraMoveDuration = 2f; //ī�޶� �̵� �ð�

    [Header("���� UI")]
    public Text bossNameText;               // ���� �̸��� ���� �ؽ�Ʈ ����
    public float bossNameDisplayTime = 2f;  // ���� �̸��� ��Ÿ�� �ð�
    public Slider bossHealthBar;            // ���� ü�¹�( slilder)
    public float healthBarFillDuration = 2f; // ü�¹ٸ� ä��� �ð�

    [Header("���� �ִϸ�����")]
    public Animator bossAnimator;

    [Header("�÷��̾� ��Ʈ��")]
    public MonoBehaviour playerController;  // �÷��̾� ��Ʈ�� ��ũ��Ʈ
    public Rigidbody2D playerRigidbody;

    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;



    void Start()
    {
        // ���� UI�� �ʱ���´� false�� ���� (������ �� Ȱ��ȭ)
        if(bossNameText != null)
        {
            bossNameText.gameObject.SetActive(false);
        }
        if(bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(false); 
        }
    }

    //���� �濡 ���� �� �Լ��� ȣ���Ͽ� ���� ����
    public void StartBossIntro()
    {
        StartCoroutine(BossIntroSequence());
    }

    IEnumerator BossIntroSequence()
    {

        //1. �÷��̾� ���� ��Ȱ��ȭ
        if(playerController != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
            playerController.enabled = false;
        }


        //2. ���� ī�޶� ��ġ�� ȸ�� ����( ���Ϳ�)
        originalCameraPosition = mainCamera.transform.position;
        originalCameraRotation = mainCamera.transform.rotation;

        //���� ��ġ �߽��� �عٴ����� �������־ ���� ������ ī�޶� ������ �߽��� ����
        bossPos = bossTransform.position;
        bossPos.y += 2f;

        //3. ī�޶� ���� ��ġ�� �̵�
        yield return StartCoroutine(MoveCamera(bossPos, bossTransform.rotation, cameraMoveDuration));


        //4. ���� �̸� �ؽ�Ʈ ǥ��
        if(bossNameText != null)
        {
            bossNameText.text = "Project_ZEUS";
            bossNameText.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(bossNameDisplayTime);
        if (bossNameText != null)
            bossNameText.gameObject.SetActive(false);

        
        // 5.ü�¹� UI Ȱ��ȭ �� ä��� �ִϸ��̼�
        if(bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(true);
            float elapsedTime = 0f;

            //�ʱ� ü�¹ٸ� 0���� �����ϰ� 0~100%���� ���� �������� ��������
            while( elapsedTime < healthBarFillDuration)
            {
                bossHealthBar.value = Mathf.Lerp(0, bossHealthBar.maxValue, elapsedTime/healthBarFillDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            bossHealthBar.value = bossHealthBar.value;
        }

        // 6. ī�޶� �÷��̾�� ����
        yield return StartCoroutine(MoveCamera(originalCameraPosition, originalCameraRotation, cameraMoveDuration));

        // 7. �÷��̾� ����� ��Ȱ��ȭ
        if(playerController != null)
        {
            playerController.enabled = true;
        }

        // 8. ���� �ִϸ����� Ȱ��ȭ -> ���� ����
        if(bossAnimator != null)
        {
            bossAnimator.enabled = true ;
        }
        

    }

    // ī�޶� �̵��� ���� �ڷ�ƾ( ���� ����)
    IEnumerator MoveCamera(Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;

        targetPosition.z = startPos.z; // ī�޶�� ������Ʈ�� z���� �ٸ��� �����Ǿ�����, ī�޶�z���� �ٲ�� Ȯ�������� �ٲ�Ƿ� �ٲ����ʰ� ����


        float elapsed = 0f;  // Time.deltaTime �� ���缭 ����
        while (elapsed < duration)
        {
            float t = elapsed / duration; // deltatime/ 2�� ���ǹǷ� ���� targetpositon���ٰ���
            mainCamera.transform.position = Vector3.Lerp(startPos, targetPosition, t);
            mainCamera.transform.rotation = Quaternion.Lerp(startRot, targetRotation, t);
            elapsed += Time.deltaTime;
            yield return null;

        }
        // 2�ʰ� ������ �ݺ����� �������� targetPosition���� ��ġ�� ��������
        mainCamera.transform.position = targetPosition;
        mainCamera.transform.rotation = targetRotation;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
