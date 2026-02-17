using UnityEngine;

namespace StormPig.Items {
    [System.Serializable]
    public class Item  {
        [SerializeField] public ItemData Data;
        [SerializeField] public Vector2Int[] InventoryPosition;
    }
}