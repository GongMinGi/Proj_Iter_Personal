using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float lifetime;
    [SerializeField] private LayerMask whatisPlatform;
    [SerializeField] private float explosionRadius;

    [System.Obsolete]
    void Start()
    {
        rb.velocity = transform.right * speed;
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



/*using System.Collections.Generic; //bullet prefab에 들어가는 스크립트입니다.https://www.youtube.com/watch?v=ZDkMhiNQOwo&t=5s 참고
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float lifetime;

    [System.Obsolete]
    void Start()
    {
        rb.velocity = transform.right * speed;
        Invoke("DestroyProjectile", lifetime); // 일정 시간 후 파괴
    }

    void DestroyProjectile()
    {
        Destroy(gameObject); // 현재 객체 삭제
    }
}

*/