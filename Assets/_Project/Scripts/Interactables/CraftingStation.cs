using UnityEngine;
using StormPig.Crafting;

namespace StormPig.Interactables {
    public class CraftingStation : MonoBehaviour, IInteractable {
        public void Interact() {
            Managers.CraftingStationsManager.Instance.OpenStation(DataRef);
        }

        public void Selected() {
            Global.Events.SelectInteractable?.Invoke(1, DataRef.Name);
        }


        [field: SerializeField] public CraftingStationData DataRef { get; private set; }
       
    }
}
