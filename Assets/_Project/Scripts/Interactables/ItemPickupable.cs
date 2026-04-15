using UnityEngine;
using StormPig.Items;

namespace StormPig.Interactables {
    public class ItemPickupable : MonoBehaviour, IInteractable {
        public void Interact() {
            Pickup();
        }

        public void Selected() {
            Global.Events.SelectInteractable?.Invoke(0, DataRef.Name);
        }


        [field: SerializeField] public ItemData DataRef { get; private set; }
        [SerializeField] private Vector2Int _ammounts;

        private int _ammount;

        private void Awake() {
            _ammount = Random.Range(_ammounts.x, _ammounts.y);
        }

        private void Pickup() {
            Managers.InventoriesManager.Instance.PlayerPickup(DataRef, _ammount, PickupSuccesfull);
        }

        private void PickupSuccesfull() {
            Destroy(gameObject);
        }

       
    }
}

