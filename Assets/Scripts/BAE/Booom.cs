
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
