using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2 inputVec;

    Rigidbody2D rigid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        inputVec.x = Input.GetAxis("Horizontal");
        inputVec.y = Input.GetAxis("Vertical");

    }

    private void FixedUpdate()
    {
        
    }
}
