using UnityEngine;

public class LineTrajectory : MonoBehaviour
{
    public Transform startPoint;        // 시작 지점
    public Transform endPoint;          // 끝 지점
    public float lineDuration = 2.0f;    // 라인 지속 시간

    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        DrawStraightLine();
        lineRenderer.material = GetComponent<LineRenderer>().material;
    }

    private void DrawStraightLine()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);

        // 라인 지속 시간 후에 라인을 비활성화합니다.
        StartCoroutine(DisableLineAfterDelay());
    }

    private System.Collections.IEnumerator DisableLineAfterDelay()
    {
        yield return new WaitForSeconds(lineDuration);
        lineRenderer.positionCount = 0; // 라인을 비활성화합니다.
    }
}