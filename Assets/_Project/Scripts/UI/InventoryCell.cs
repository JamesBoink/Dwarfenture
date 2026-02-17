using UnityEngine;

namespace StormPig.UI {
    [System.Serializable]
    public class InventoryCell {
        [SerializeField] public Vector2Int Position;

        public InventoryCell(Vector2Int p) {
            Position = p;
        }
    }
}