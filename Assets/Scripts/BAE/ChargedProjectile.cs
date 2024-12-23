using UnityEngine;

public class ChargedProjectile : MonoBehaviour
{
    [SerializeField] private float speed; // 차지 어택 발사 속도
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float explosionRadius; // 폭발 반경
    [SerializeField] private LayerMask whatisPlatform; // 타일맵이나 폭발 대상을 위한 레이어
    [SerializeField] private GameObject explosionEffectPrefab; // 폭발 이펙트 프리팹 (선택)

    [System.Obsolete]
    void Start()
    {
        rb.velocity = transform.right * speed; // 오른쪽 방향으로 발사
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌 시 폭발 효과 실행
        Explode();
        Destroy(gameObject); // 충돌한 차지 어택 제거
    }

    private void Explode()
    {
        // 폭발 반경 내의 모든 Collider 탐색
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, whatisPlatform);

        foreach (var collider in colliders)
        {
            // 타일맵 파괴
            Bricks brick = collider.GetComponent<Bricks>();
            if (brick != null)
            {
                brick.MakeDot(collider.transform.position);
            }

            // 적 처리 로직 (선택)
            // 적 스크립트가 있다면 여기에 추가 가능
        }

        // 폭발 이펙트 생성 (선택 사항)
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 폭발 반경을 시각적으로 표시 (디버그용)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
