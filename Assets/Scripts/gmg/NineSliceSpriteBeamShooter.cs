using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class NineSliceSpriteBeamShooter : MonoBehaviour
{

    public float minbeamLength = 2f; //차지 0% 일때 길이
    public float maxBeamLength = 8f; // 차지 100% 일때의 길이
    public float minBeamHeight = 0.2f; //차지 0% 일 때 높이 (두께)
    public float maxBeamHeight = 0.5f; //차지 100% 일 때 높이


    public float shortBeamDuration = 0.2f; // 발사 순간 잠깐 표시할 시간

    public LayerMask hitlayer; // 충돌 판정용

    private SpriteRenderer spriteR;
    private Coroutine beamRoutine;


    private void Awake()
    {
        spriteR = GetComponent<SpriteRenderer>();

        //스프라이트의 드로우 모드는 미리 인스팩터에서 sliced로 해두거나 여기서 설정

        spriteR.enabled = false; // 시작 시에는 빔이 보이지 않게
    }


    public void ShootBeam(float chargeRatio)
    {
        //혹시 이전 코루틴이 진행중이면 중단
        if(beamRoutine != null)
        {
            StopCoroutine(beamRoutine);
        }

        //차지 비율에 맞춘 길이/ 두께 계산
        float beamLength = Mathf.Clamp(minbeamLength, maxBeamLength, chargeRatio);
        float beamHeight = Mathf.Clamp(minBeamHeight, maxBeamHeight, chargeRatio);
    }


    private IEnumerator DoBeamRoutine(float length, float height)
    {
        // 1) 일단 스프라이트 활성화
        spriteR.enabled = true;


        // 2) 레이케스트로 충돌지점 확인 
        Vector2 startPos = transform.position;
        Vector2 dir = transform.right;


        RaycastHit2D hit = Physics2D.Raycast(startPos, dir, length, hitlayer);
        Vector2 endPos;
        if(hit.collider != null)
        {
            endPos = hit.point;
            // 맞은 대상이 파괴 가능이라면 파괴
            if(hit.collider.CompareTag("Destroyable"))
            {
                Destroy(hit.collider.gameObject);
            }
        }
        else
        {
            endPos = startPos + dir * length;
        }

        // 3) 9-slice 스프라이트의 "size" 조정
        float actualDistance = Vector2.Distance(startPos, endPos);

        spriteR.size = new Vector2(actualDistance, height);

        // 4) 방향에 따라 스케일 or flip처리필요

        // 5) shortBeamDuration 만큼 빔을 보여주고 사라지기
        yield return new WaitForSeconds(shortBeamDuration);

        // 6) 빔 비활성
        spriteR.enabled = false;
    }
}
