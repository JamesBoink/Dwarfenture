using UnityEngine;

namespace StormPig.Crafting {
    [CreateAssetMenu(menuName = "StormPig/CraftingStation")]
    public class CraftingStationData : ScriptableObject {
        [field: SerializeField] public string Name { get; private set; } = default;
        [field: SerializeField] public string Description { get; private set; } = default;
        [field: SerializeField] public RecipeRequirement StationRequirements { get; private set; } = default;

        [field: SerializeField] public ItemRecipe[] AvailableRecipes { get; private set; } = default;
        [field: SerializeField] public ItemRecipe[] HiddenRecipes { get; private set; } = default;
        [field: SerializeField] public int[] MaxItemsAtOnce { get; private set; } = default;
        [field: SerializeField] public int MaxItemsInStorage { get; private set; } = default;
    }
}