using UnityEngine;

namespace StormPig.Crafting {
    [System.Serializable]
    public class RecipeRequirement {
        [field: SerializeField] public Items.ItemData[] Items{ get; private set; } = default;
        [field: SerializeField] public float[] Quantities{ get; private set; } = default;
    }

    [CreateAssetMenu(menuName =("StormPig/Item Recipe"))]
    public class ItemRecipe : ScriptableObject {
        [field: SerializeField] public RecipeRequirement Requirements { get; private set; } = default;
        [field: SerializeField] public float CraftingTime { get; private set; } = default;
        [field: SerializeField] public Items.ItemData Produce{ get; private set; } = default;
    }
}