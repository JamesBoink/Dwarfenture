using UnityEngine;
using UnityEngine.UI;

namespace StormPig.UI {
    public class BackpackHandler : MonoBehaviour {
        [SerializeField] private Image[] inventoryCells;
        [SerializeField] private Color inactive;
        [SerializeField] private Color active;

        private void OnDisable() {
            SelectCell(-1);
        }

        public void SelectCell(int ind) {
            for(int i = 0; i < inventoryCells.Length; i++) {
                if(i == ind) {
                    inventoryCells[i].color = active;
                } else {
                    if (inventoryCells[i].color != inactive) {
                        inventoryCells[i].color = inactive;
                    }
                }
            }
        }
    }
}