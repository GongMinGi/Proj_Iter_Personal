using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ShootBeamWithSprite : MonoBehaviour
{
    private SpriteRenderer sr;


    private void Awake()
    {
        // 반드시 SpriteRenderer를 가져온다
        sr = GetComponent<SpriteRenderer>();

        // 혹시 Inspector에서 안 바꿨다면, 여기서도 Sliced 모드로 설정 가능
        sr.drawMode = SpriteDrawMode.Sliced;
    }

    /// <summary>
    /// 빔의 길이를 동적으로 변경하는 함수
    /// </summary>
    /// <param name="length">늘리고 싶은 길이</param>
    public void SetBeamLength(float length)
    {
        // 1) Sliced가 아닌 경우 예외처리
        if (sr.drawMode != SpriteDrawMode.Sliced)
        {
            Debug.LogWarning("SpriteRenderer.drawMode가 Sliced가 아닙니다.");
            return;
        }

        // 2) 기존 size에서 x값(길이)만 변경
        Vector2 newSize = sr.size;
        newSize.x = length;
        sr.size = newSize;
    }
}
