using UnityEngine;
using TMPro;

namespace StormPig {
    public class MainUI : MonoBehaviour {
        [SerializeField] private GameObject _interactionPanel;
        [SerializeField] private TextMeshProUGUI _interactionText;
        [SerializeField] private string[] _interactionKeys;


        private void Awake() {
            Global.Events.SelectInteractable += InteractableSelected;
            Global.Events.CleanupInteractionPanel += Interacted;
        }

        private void OnDestroy() {
            Global.Events.SelectInteractable -= InteractableSelected;
            Global.Events.CleanupInteractionPanel -= Interacted;
        }

        private void InteractableSelected(int ind, string name) {
            _interactionPanel.SetActive(true);
            _interactionText.text = _interactionKeys[ind] + " " + name;
        }

        private void Interacted() {
            if (_interactionPanel.activeInHierarchy) {
                _interactionPanel.SetActive(false);
            }
        }
    }
}