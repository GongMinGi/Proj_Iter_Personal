using UnityEngine;

public class MouseInput : MonoBehaviour
{
    Vector3 MousePosition;
    public LayerMask whatisPlatform;
    public GameObject boomClone;
    private void OnDrawGizmos ()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere (MousePosition, 0.2f);
    }
    void Update()
    {
       if (Input.GetMouseButtonDown (1)) 
                    {
            MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Instantiate(boomClone, MousePosition, Quaternion.identity);

        }
    }
}