using UnityEngine;

public class SavePoint : MonoBehaviour
{

    private Vector3 savePoint;  // 저장할 위치 변수

    void Start()
    {
        
        savePoint = transform.position;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("SavePoint"))
        {
            
            savePoint = collision.transform.position;       // SavePoint의 위치를 저장

            Debug.Log("SavePoint updated: " + savePoint);

        }

    }

    public void Respawn(Transform player)
    {
        
        player.position = savePoint;        // 플레이어의 위치를 마지막 저장된 SavePoint 위치로 이동

        Debug.Log("Player respawned at: " + savePoint);

    }

}
