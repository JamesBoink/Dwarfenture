using UnityEngine;
using StormPig.Crafting;

namespace StormPig.Interactables {
    public class CraftingStation : MonoBehaviour, IInteractable {
        [field: SerializeField] public CraftingStationData DataRef { get; private set; }
        public void Interact() {

        }

        public void Selected() {
            Global.Events.SelectInteractable?.Invoke(1,DataRef.Name);
        }
    }
}
