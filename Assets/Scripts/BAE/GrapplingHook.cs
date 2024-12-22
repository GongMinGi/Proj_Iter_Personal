using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public LineRenderer line;
    public Transform hook;
    void Start()
    {


        line.positionCount = 2;
        line.endWidth = line.startWidth = 0.05f;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, hook.position);
        line.useWorldSpace = true;

    }
    void Update()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1,hook.position);
    }
}
