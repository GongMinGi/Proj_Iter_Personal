using UnityEngine;

public class SavePoint : MonoBehaviour
{

    private Vector3 savePoint;

    void Start()
    {
          
        savePoint = transform.position;

    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("SavePoint"))
        {

            savePoint = transform.position;
            Debug.Log("SavePoint updated");

        }

    }

}
