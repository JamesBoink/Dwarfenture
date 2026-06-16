using UnityEngine;

namespace StormPig.Items {
    [CreateAssetMenu(menuName = "StormPig/Item")]
    public class ItemData : ScriptableObject {
        [field: SerializeField] public string Name { get; private set; } = default;
        [field: SerializeField] public string Description { get; private set; } = default;
        [field: SerializeField] public float Weight { get; private set; } = default;
        [field: SerializeField] public ItemType Type { get; private set; } = ItemType.Drink;
        [field: SerializeField] public ItemQuality Quality { get; private set; } = ItemQuality.Poor;
        [field: SerializeField] public int MaxStack { get; private set; } = 1;
        [field: SerializeField] public Sprite UIIcon { get; private set; } = default;
        [SerializeField] private InvSpace _inventorySpace = InvSpace._1x1;
        [field: SerializeField] public ItemParameter[] AdditionalParameters { get; private set; } = default;
      

        public Vector2Int InventorySpaceTaken() {
            switch (_inventorySpace) {
                case InvSpace._1x1:
                    return _spaces[0];
                case InvSpace._1x2:
                    return _spaces[1];
                case InvSpace._1x3:
                    return _spaces[2];
                case InvSpace._2x1:
                    return _spaces[3];
                case InvSpace._2x2:
                    return _spaces[4];
                case InvSpace._3x3:
                    return _spaces[5];
                default:
                    return _spaces[0];
            }
        }

        private static readonly Vector2Int[] _spaces =  { 
            new Vector2Int(1, 1), 
            new Vector2Int(1, 2), 
            new Vector2Int(1, 3), 
            new Vector2Int(2, 1), 
            new Vector2Int(2, 2), 
            new Vector2Int(3, 3) 
        };
    }


    [System.Serializable]
    public class ItemParameter {
        [field: SerializeField] public ItemParameterType Type { get; private set; } = ItemParameterType.Damage;
        [field: SerializeField] public int Value { get; private set; } = 0;
    }

    public enum ItemParameterType {
        Damage,
        Armor,
        HealthRegen,
        Hydration,
        Satiation,
    }

    public enum InvSpace {
        _1x1,
        _1x2,
        _1x3,
        _2x1,
        _2x2,
        _3x3,
    }
}