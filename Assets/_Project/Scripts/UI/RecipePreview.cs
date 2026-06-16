using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StormPig.UI {
    public class RecipePreview : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private Image _icon;

        public void OnCreate(Crafting.ItemRecipe i) {
            _title.text = i.Produce.Name;
            _icon.sprite = i.Produce.UIIcon;
        }


      // hrer make it so that this will print and populate
      // crafting requirements for thhis recipe
        public void OnSelect() {

        }
    }
}