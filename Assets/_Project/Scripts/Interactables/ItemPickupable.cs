using UnityEngine;
using StormPig.Items;

namespace StormPig.Interactables {
    public class ItemPickupable : MonoBehaviour, IInteractable {
        [field: SerializeField] public ItemData DataRef { get; private set; }
        [SerializeField] private Vector2Int _ammounts;

        private int _ammount;

        public System.Action<ItemData, int, System.Action> PickupA;

        private void Awake() {
            _ammount = Random.Range(_ammounts.x, _ammounts.y);
        }

        private void Pickup() {
            PickupA.Invoke(DataRef, _ammount, PickupSuccesfull);
        }

        private void PickupSuccesfull() {
            Destroy(gameObject);
        }

        public void Interact() {
            Pickup();
        }

        public void Selected() {
            Global.Events.SelectInteractable?.Invoke(0,DataRef.Name);
        }
    }
}

