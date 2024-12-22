using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/Item")] 
public class Item : ScriptableObject
{

    public string itemName;
    public ItemType itemType;
    public Sprite itemImage;
    public GameObject itemPrefab;

    public enum ItemType { Parts, MonsterDropped, ETC }

}
