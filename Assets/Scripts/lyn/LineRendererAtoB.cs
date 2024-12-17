using UnityEngine;

public class LineRenderAtoB : MonoBehaviour
{
    public Transform startPoint; // 시작 지점
    public Transform endPoint;   // 끝 지점

    private LineRenderer lineRenderer;

    void Start()
    {
        // LineRenderer 컴포넌트 추가
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        // LineRenderer 초기 설정
        lineRenderer.positionCount = 2; // 두 개의 점 필요 (시작점, 끝점)
        lineRenderer.startWidth = 0.1f; // 선의 시작 두께
        lineRenderer.endWidth = 0.1f;   // 선의 끝 두께
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Material 설정
        lineRenderer.startColor = Color.red; // 선 색상
        lineRenderer.endColor = Color.red;

        // 초기 위치 업데이트
        UpdateLinePosition();
    }

    void Update()
    {
        // 매 프레임마다 선 위치 업데이트
        UpdateLinePosition();
    }

    private void UpdateLinePosition()
    {
        // 시작점과 끝점이 null이 아닌 경우에만 실행
        if (startPoint != null && endPoint != null)
        {
            lineRenderer.SetPosition(0, startPoint.position); // 시작점 위치
            lineRenderer.SetPosition(1, endPoint.position);   // 끝점 위치
        }
    }
}
