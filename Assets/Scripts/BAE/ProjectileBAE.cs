/*using UnityEngine;

public class ProjectileBAE : MonoBehaviour
{
    [SerializeField] private float speed; // 발사 속도
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private float lifetime; // 수명
    [SerializeField] private float explosionRadius; // 폭발 반경
    [SerializeField] private LayerMask whatisPlatform; // 타일맵 파괴 대상
    [SerializeField] private GameObject explosionEffectPrefab; // 폭발 이펙트 (선택)

    public bool isCharged = false; // 차지 어택 여부

    [System.Obsolete]
    void Start()
    {
        rigid.velocity = transform.right * speed; // 총알 이동
        Destroy(gameObject, lifetime); // 일정 시간 후 삭제
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCharged)
        {
            Explode(); // 차지 어택일 경우 폭발
        }
        Destroy(gameObject); // 충돌 후 제거
    }

    private void Explode()
    {
        // 폭발 반경 내 Collider 탐색
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, whatisPlatform);

        foreach (var collider in colliders)
        {
            // 타일맵 파괴
            Bricks brick = collider.GetComponent<Bricks>();
            if (brick != null)
            {
                brick.MakeDot(collider.transform.position);
            }
        }

        // 폭발 이펙트 생성 (선택 사항)
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (explosionRadius > 0)
        {
            Gizmos.color = isCharged ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, explosionRadius); // 폭발 반경 시각화
        }
    }
}




*/

using UnityEngine;

public class ProjectileBAE : MonoBehaviour
{
    [SerializeField] private float speed; // 발사 속도
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float lifetime; // 수명
    [SerializeField] private float explosionRadius; // 폭발 반경
    [SerializeField] private LayerMask whatisPlatform; // 타일맵 파괴 대상
    [SerializeField] private GameObject explosionEffectPrefab; // 폭발 이펙트 (선택)

    public bool isCharged = false; // 차지 어택 여부

    [System.Obsolete]
    void Start()
    {
        rb.velocity = transform.right * speed; // 총알 이동
        Destroy(gameObject, lifetime); // 일정 시간 후 삭제
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCharged)
        {
            Explode(); // 차지 어택일 경우 폭발
        }
        Destroy(gameObject); // 충돌 후 제거
    }

    private void Explode()
    {
        // 폭발 반경 내 Collider 탐색
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, whatisPlatform);

        foreach (var collider in colliders)
        {
            // 타일맵 파괴
            Bricks brick = collider.GetComponent<Bricks>();
            if (brick != null)
            {
                brick.MakeDot(collider.transform.position);
            }
        }

        // 폭발 이펙트 생성 (선택 사항)
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (explosionRadius > 0)
        {
            Gizmos.color = isCharged ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, explosionRadius); // 폭발 반경 시각화
        }
    }
}





/*using System;
using UnityEngine;

public class ProjectileBAE : MonoBehaviour
{
    [SerializeField] private float speed; // 발사 속도
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private float lifetime; // 수명
    [SerializeField] private float explosionRadius; // 폭발 반경
    [SerializeField] private LayerMask whatisPlatform; // 타일맵 및 파괴 대상
    [SerializeField] private GameObject explosionEffectPrefab; // 폭발 이펙트 (선택)

    public bool isCharged = false; // 차지 어택 여부

    [System.Obsolete]
    void Start()
    {
        rigid.velocity = transform.right * speed; // 총알 이동
        Destroy(gameObject, lifetime); // 일정 시간 후 삭제
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCharged)
        {
            Explode(); // 차지 어택일 경우 폭발
        }
        Destroy(gameObject); // 충돌 후 제거
    }

    private void Explode()
    {
        // 폭발 반경 내 Collider 탐색
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, whatisPlatform);

        foreach (var collider in colliders)
        {
            // 타일맵 파괴
            Bricks brick = collider.GetComponent<Bricks>();
            if (brick != null)
            {
                brick.MakeDot(collider.transform.position);
            }
        }

        // 폭발 이펙트 생성 (선택 사항)
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 폭발 반경 시각화
        Gizmos.color = isCharged ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    internal void Initialize(bool v)
    {
        throw new NotImplementedException();
    }
}
*/


/*using UnityEngine;

public class ProjectileBAE : MonoBehaviour

{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private float lifetime;
    [SerializeField] private LayerMask whatisPlatform;
    [SerializeField] private float explosionRadius;

    [System.Obsolete]
    void Start()
    {
        rigid.velocity = transform.right * speed;
        Invoke("DestroyProjectile", lifetime); // 일정 시간 후 삭제
    }

    void DestroyProjectile()
    {
        Explode(); // 범위 공격 처리
        Destroy(gameObject);
    }

    void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, whatisPlatform);

        foreach (var collider in colliders)
        {
            Bricks brick = collider.GetComponent<Bricks>();
            if (brick != null)
            {
                brick.MakeDot(collider.transform.position);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius); // 폭발 범위 표시
    }
}

*/

