using UnityEngine;

namespace StormPig.Items {
    public class ItemObject : MonoBehaviour {
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

        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Pickup")) {
                Pickup();
            }
        }
    }
}
