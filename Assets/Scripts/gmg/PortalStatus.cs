using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PortalStatus : MonoBehaviour
{
    [SerializeField]
    private string nextSceneName;

    private Rigidbody2D playerRigidbody;    //�÷��̾ ������ rigidbody�� ����
    private Transform cameraTransform;      //���� ī�޶��� trasnform ����
    private Vector3 cameraOriginalPosition; //ī�޶��� ������ġ ����(�� ��ȯ �� ī�޶� ��ġ������ ����)

    [Header("Fade Settings")]
    public Image fadeScreen;                // ���̵� ȿ���� ����� �̹���
    public float fadeSpeed = 1.0f;          //���̵� ȿ���� �ӵ��� �����ϴ� ����

    [Header("Scene Settings")]
    public string sceneToLoad;               //��ȯ�� ���� �̸�

    private bool isTransitioning = false;       //�� ��ȯ�� ������������ ��Ÿ���� ����(�ߺ� ���� ����)

    private void Awake()
    {
        cameraTransform = Camera.main.transform;

        // ���̵� ȭ�� �ʱ�ȭ
        if (fadeScreen == null)
        {
            Debug.LogError("FadeScreen�� �������� �ʾҽ��ϴ�. Inspector���� FadeScreen�� �������ּ���.");
        }
        else
        {
            fadeScreen.gameObject.SetActive(false);     //���۽� fadescreen�� ��Ȱ��ȭ�ؼ� ȭ�鿡 ������ �ʵ��� ��
            Color initialColor = fadeScreen.color;      //fadescreen�� �ʱ� ������ ������... �Ⱦ�
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !isTransitioning)
        {
            playerRigidbody = other.GetComponent<Rigidbody2D>();

            playerRigidbody.linearVelocity = Vector2.zero; // �÷��̾� ������ ����

            // ī�޶� ���� ��ġ ���� (���̵� ȿ������ ī�޶� �����ϱ� ����)
            cameraOriginalPosition = cameraTransform.position;

            // ī�޶�� �÷��̾��� Parent ���� ����
            if (cameraTransform.parent == transform)
            {
                cameraTransform.SetParent(null, true); // �θ� ���� ���� (���� ��ǥ ����)
            }

            //�� ��ȯ�� ���� �ڷ�ƾ ����
            StartCoroutine(TransitionScene());
        }
    }



    // ������ ȿ���� �� ��ȯ�� ���������� ����
    IEnumerator TransitionScene()
    {
        // �� ��ȯ ������ ǥ���Ͽ� �ߺ� ���� ����
        isTransitioning = true;

        // ī�޶� ��ġ ���� (fade ȿ�� ���� ī�޶� ��ġ�� �����ϴ� �� ���)
        Vector3 fixedCameraPosition = cameraTransform.position;

        // ��ȯ ���� ���� 0.8�� ���
        yield return new WaitForSeconds(0.8f);

        // ���� ȭ�� ���̵� �� ȿ��
        yield return StartCoroutine(FadeOutEffect());

        //���̵� ȿ�� ���� ��� ����
        yield return new WaitForSeconds(0.5f);

        // �� ��ȯ
        SceneManager.LoadScene(sceneToLoad);
    }



    // ���̵�ƿ�(���� ȭ���� ��Ÿ����) ȿ�� ����
    private IEnumerator FadeOutEffect()
    {
        //fadescreen�� rectTranform�� ������( ȭ�� �� ��ġ �� ũ�⸦ �����ϱ� ����)
        RectTransform fadeScreenRect = fadeScreen.rectTransform;

        // �ʱ� ��ġ: ȭ�� �Ʒ�
        Vector3 startPosition = new Vector3(0, -Screen.height, 0);
        Vector3 endPosition = Vector3.zero; // ȭ�� �߾����� �̵�

        fadeScreenRect.localPosition = startPosition;   // fadescreen�� �ʱ� ��ġ ����
        fadeScreen.gameObject.SetActive(true);          //fadescreen ������Ʈ�� Ȱ��ȭ�Ͽ� ȭ�鿡 ǥ��

        float elapsedTime = 0f;              // ����ð��� ������ ���� �ʱ�ȭ

        while (elapsedTime < 1f / fadeSpeed) // ���̵� �ӵ��� ���� �ð� ��� 1/fadespeed�� ���� ����
        {
            float t = elapsedTime / (1f / fadeSpeed); // t ���� 0�� 1 ������ �����( ���� ���� ����)

            fadeScreenRect.localPosition = Vector3.Lerp(startPosition, endPosition, t);

            // ī�޶� ��ġ�� ��� ���� (���̵� ȿ�� ���� ī�޶��� ��ġ�� ��� ���� ��ġ�� ����)
            cameraTransform.position = cameraOriginalPosition;

            // ��� �ð� ���� ( �����Ӵ� ��� �ð�)
            elapsedTime += Time.deltaTime;

            // ���� �����ӱ��� ���
            yield return null;
        }

        // �ݺ��� ���� �� fadeScreen�� ��ġ�� ��Ȯ�� ���� ��ġ�� ����
        fadeScreenRect.localPosition = endPosition; // ���� ��ġ ����
    }
}
