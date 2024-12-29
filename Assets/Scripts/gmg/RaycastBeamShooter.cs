using UnityEngine;

public class RaycastBeamShooter : MonoBehaviour
{

    [SerializeField] private LineRenderer beamLinePrefab;
    [SerializeField] private Transform firepoint; // 빔 발사 시작점
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private LayerMask hitLayers;

    public void ShootBeam()
    {
        //(1)레이케스트로 충돌 지점 확인 
        // 발사 방향 조절 필요
        RaycastHit2D hit = Physics2D.Raycast(firepoint.position, transform.right, maxDistance, hitLayers);

        //(2)LineRender 만들기
        LineRenderer beamLine = Instantiate(beamLinePrefab, Vector3.zero, Quaternion.identity);

        Vector2 endPos;
        if(hit.collider != null)
        {
            endPos = hit.point; //레이케스트가 맞은 지점
            if(hit.collider.CompareTag("Destroyable"))
            {
                Destroy(hit.collider.gameObject);
            }
        }
        else
        {
            // 충돌이 없으면 maxDistance 까지 선 그리기
            endPos = (Vector2)firepoint.position + (Vector2)transform.right*maxDistance;
        }

        // 4) LineRenderer 의 시작점/ 끝점 설정
        beamLine.positionCount = 2;
        beamLine.SetPosition(0, firepoint.position);
        beamLine.SetPosition(1, endPos);

        //5) 빔을 잠시 보여준 뒤 제거
        Destroy(beamLine.gameObject, 0.5f);
        

    }


}
