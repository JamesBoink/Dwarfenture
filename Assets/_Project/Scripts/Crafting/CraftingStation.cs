using UnityEngine;

namespace StormPig.Crafting {
    public class CraftingStation : MonoBehaviour {
        [SerializeField] public CraftingStationData Data;
        [SerializeField] public Items.Item[] CraftedItems;
        public System.Action InteractWithStation { get; set; }

        public void Interact() {
            InteractWithStation?.Invoke();
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.CompareTag("Pickup")) {
                Interact();
            }
        }
    }
}