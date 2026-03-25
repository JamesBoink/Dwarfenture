using UnityEngine;
using StormPig.Inventories;
using StormPig.Items;
using StormPig.UI;

namespace StormPig.Managers {
    public class InventoriesManager : MonoBehaviour {
        public static InventoriesManager Instance;
        [SerializeField] private Inventory _playerInventory;
        [SerializeField] private Inventory[] InventoriesInLevel;
        [SerializeField] private ItemObject[] _itemsInScene;

        public System.Action PickedUp;

        private void Awake() {
            Singleton();
            for(int i =0; i < _itemsInScene.Length; i++) {
                _itemsInScene[i].PickupA += PlayerPickup;
            }
        }

        private void Singleton() {
            if(Instance == null) {
                Instance = this;
                DontDestroyOnLoad(this);
                return;
            }
            Destroy(this);
        }

        public void PlayerPickup(ItemData d, int a, System.Action success) {
            Item it = new Item(d);
            bool s = _playerInventory.TryAddItem(it, a);
            if (s) {
                success.Invoke();
            } else {
                Global.Log.Trace("No space for item: <color=green>" + it.Data.Name + " in player inventory");
            }
        }
    }
}