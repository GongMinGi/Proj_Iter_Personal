using UnityEngine;

public class ParallaxBackground_test02 : MonoBehaviour
{
    [SerializeField]
    [Range(-1.0f, 1.0f)]
    private float moveSpeed = 0.1f;
    private Material material;



    void Awake()
    {
        material = GetComponent<Renderer>().material;

        
    }

    // Update is called once per frame
    void Update()
    {
        material.SetTextureOffset("_MainTex", Vector2.right *  moveSpeed *Time.time);

    }
}
