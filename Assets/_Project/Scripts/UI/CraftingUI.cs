using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StormPig.UI {
    public class CraftingUI : MonoBehaviour {
        [SerializeField] private GameObject _craftingWindow;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Image _icon;


        public void OpenStation(Crafting.CraftingStationData c) {
            _craftingWindow.SetActive(true);
            _nameText.text = c.Name;
            _descriptionText.text = c.Description;
            _icon.sprite = c.Icon;
        }
        

    }
}