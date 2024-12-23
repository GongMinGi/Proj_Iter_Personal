using System.Collections;
using UnityEngine;

public class Booom : MonoBehaviour
{
    public GameObject explosionAreaGO;
    public LayerMask whatisPlatform;
    public CircleCollider2D circleCollider2D;

    void Start()
    {
        explosionAreaGO.SetActive(false);
    }

    // 충돌 시 폭발 처리
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 오브젝트가 지정된 레이어에 포함되어 있는지 확인
        if (((1 << collision.gameObject.layer) & whatisPlatform) != 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        // 폭발 영역 활성화 (선택)
        explosionAreaGO.SetActive(true);

        // 폭발 영역 내 타일 파괴
        DestroyArea();

        // 폭탄 오브젝트 삭제
        Destroy(this.gameObject, 0.05f);
    }

    void DestroyArea()
    {
        int radiusInt = Mathf.RoundToInt(circleCollider2D.radius);
        for (int i = -radiusInt; i <= radiusInt; i++)
        {
            for (int j = -radiusInt; j <= radiusInt; j++)
            {
                Vector3 checkCellPos = new Vector3(transform.position.x + i, transform.position.y + j, 0);
                float distance = Vector2.Distance(transform.position, checkCellPos) - 0.001f;

                if (distance <= radiusInt)
                {
                    Collider2D overCollider2D = Physics2D.OverlapCircle(checkCellPos, 0.01f, whatisPlatform);
                    if (overCollider2D != null)
                    {
                        overCollider2D.transform.GetComponent<Bricks>().MakeDot(checkCellPos);
                    }
                }
            }
        }
    }
}


/*
using System.Collections;
using UnityEngine;

public class Booom : MonoBehaviour
{
    public GameObject explosionAreaGO;
    public LayerMask whatisPlatform;
    public CircleCollider2D circleCollider2D;

    void Start()
    {
        explosionAreaGO.SetActive(false);
        StartCoroutine(Boooom());
    }

    IEnumerator Boooom()
    {
        yield return new WaitForSeconds(1f);
        explosionAreaGO.SetActive(true);

        DestroyArea();
        Destroy(this.gameObject, 0.05f);
    }

    void DestroyArea()
    {
        int radiusInt = Mathf.RoundToInt(circleCollider2D.radius);
        for (int i = -radiusInt; i <= radiusInt; i++)
        {
            for (int j = -radiusInt; j <= radiusInt; j++)
            {
                Vector3 checkCellPos = new Vector3(transform.position.x + i, transform.position.y + j, 0);
                float distance = Vector2.Distance(transform.position, checkCellPos) - 0.001f;

                if (distance <= radiusInt)
                {
                    Collider2D overCollider2D = Physics2D.OverlapCircle(checkCellPos, 0.01f, whatisPlatform);
                    if (overCollider2D != null)
                    {
                        overCollider2D.transform.GetComponent<Bricks>().MakeDot(checkCellPos);
                    }
                }
            }
        }
    }
}
*/