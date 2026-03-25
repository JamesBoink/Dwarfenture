using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace StormPig.UI {
    public class HoverPanel : MonoBehaviour {
        [field: SerializeField] public RectTransform Rect { get; private set; }
        [SerializeField] private TextMeshProUGUI[] _texts;
        [SerializeField] private TextMeshProUGUI[] _addParamsTexts;
        [SerializeField] private Image _icon;


        public void DisplayInfo(string[] info, Sprite ic) {
            for (int i = 0; i < _texts.Length; i++) {
                _texts[i].text = info[i];
            }
          
            // This checks if we got any additional params
            if(info.Length > 5) {
                int currentIndex = 5;
                for (int j = 0; j < _addParamsTexts.Length; j++) {
                    // this checks if we still have params to put (max 3)
                    // if not it leaves texts empty
                    if(currentIndex < info.Length) {
                        _addParamsTexts[j].text = info[currentIndex];
                        currentIndex++;
                        j++;
                        _addParamsTexts[j].text = info[currentIndex];

                        currentIndex++;
                    } else {
                        _addParamsTexts[j].text = "";
                        j++;
                        _addParamsTexts[j].text = "";
                    }                    
                }
            } else {
                for (int j = 0; j < _addParamsTexts.Length; j++) {
                    _addParamsTexts[j].text = "";
                }
            }
           

            _icon.sprite = ic;
        }
    }

}