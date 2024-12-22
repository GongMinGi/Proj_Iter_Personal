using UnityEngine;
using UnityEngine.Tilemaps;
public class Bricks : MonoBehaviour
{
    public Tilemap tilemap;
    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void MakeDot(Vector3 Pos)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(Pos);

        tilemap.SetTile(cellPosition, null);
    }
}
