using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Made by KMS
[System.Serializable]
public class SlideData
{
    public Sprite image;       // 슬라이드 이미지
    public float displayTime;  // 슬라이드 표시 시간
}

public class CutScene_Slide : MonoBehaviour
{
    public Image currentImage;       // 현재 이미지를 표시할 Image
    public Image nextImage;          // 다음 이미지를 표시할 Image
    public SlideData[] slides;       // 슬라이드 데이터 배열
    public float fadeDuration = 1f; // 페이드 인/아웃 시간

    private int currentSlideIndex = 0;

    private void Start()
    {
        StartCoroutine(PlaySlideShow());
    }

    private IEnumerator PlaySlideShow()
    {
        while (currentSlideIndex < slides.Length)
        {
            // 현재 슬라이드 데이터 설정
            SlideData currentSlide = slides[currentSlideIndex];

            // 다음 슬라이드 데이터 설정 (다음 슬라이드가 있는 경우만)
            Sprite nextSlideImage = (currentSlideIndex + 1 < slides.Length) ? slides[currentSlideIndex + 1].image : null;

            // 현재 이미지를 설정
            currentImage.sprite = currentSlide.image;
            currentImage.color = new Color(1, 1, 1, 1); // 완전한 불투명

            // 다음 이미지를 미리 설정 (투명 상태)
            if (nextSlideImage != null)
            {
                nextImage.sprite = nextSlideImage;
                nextImage.color = new Color(1, 1, 1, 0); // 완전한 투명
            }

            // 슬라이드 표시 시간 대기
            yield return new WaitForSeconds(currentSlide.displayTime);

            // 페이드 아웃/인 효과를 동시에 실행
            if (nextSlideImage != null)
            {
                yield return StartCoroutine(FadeTransition());
            }

            // 다음 슬라이드로 이동
            currentSlideIndex++;
        }

        // 컷신 종료
        Debug.Log("Cutscene Finished");
        SceneManager.LoadScene("LabABasementScene"); 
    }

    private IEnumerator FadeTransition()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float alpha = elapsedTime / fadeDuration;

            // 현재 이미지는 점점 투명해짐
            currentImage.color = new Color(1, 1, 1, 1 - alpha);

            // 다음 이미지는 점점 불투명해짐
            nextImage.color = new Color(1, 1, 1, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 페이드 완료 후, 상태 고정
        currentImage.color = new Color(1, 1, 1, 0); // 완전히 투명
        nextImage.color = new Color(1, 1, 1, 1);   // 완전히 불투명
    }
}
