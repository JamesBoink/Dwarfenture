using UnityEngine;

namespace StormPig.Items {
    [CreateAssetMenu(menuName = "StormPig/Item")]
    public class ItemData : ScriptableObject {
        [field: SerializeField] public string Name { get; private set; } = default;
        [field: SerializeField] public string Description { get; private set; } = default;
        [field: SerializeField] public float Weight { get; private set; } = default;
        [field: SerializeField] public ItemType Type { get; private set; } = ItemType.Alcohol;
        [field: SerializeField] public ItemQuality Quality { get; private set; } = ItemQuality.Poor;
        [field: SerializeField] public int MaxStack{ get; private set; } = 1;
        [field: SerializeField] public Sprite UIIcon { get; private set; } = default;
        [field: SerializeField] public Vector2Int InventorySpaceTaken { get; private set; } = new Vector2Int(1, 1);
        [field: SerializeField] public ItemParameter[] AdditionalParameters { get; private set; } = default;
    }

    [System.Serializable]
    public class ItemParameter {
        [field: SerializeField] public ItemParameterType Type { get; private set; } = ItemParameterType.Damage;
        [field: SerializeField] public int Value { get; private set; } =0;
    }

    public enum ItemParameterType {
        Damage,
        Armor,
        HealthRegen,

    }
}