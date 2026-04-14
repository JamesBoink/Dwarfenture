using UnityEngine;

namespace StormPig.UI {
    public class SplitStackPanel : MonoBehaviour {
        [SerializeField] private UnityEngine.UI.Image _icon;
        [SerializeField] private UnityEngine.UI.Slider _stackSlider;
        [SerializeField] private TMPro.TMP_InputField _stackIField;

        private System.Action<int> OnAccept;
        private System.Action OnDecline;

        private int _maxValue = 0;
        private int _currentValue = 0;

        public void Initialize(System.Action<int> onAccept, System.Action onDecline) {
            OnAccept += onAccept;
            OnDecline += onDecline;
        }

        public void DisplayInfo(ItemUI it) {
            _icon.sprite = it.Icon.sprite;

            int ammount = int.Parse(it.Text.text);
            _maxValue = (ammount - 1);
            _currentValue = (_maxValue / 2) + 1;
            _stackSlider.maxValue = _maxValue;
            SetVisuals();
        }

        public void OnSlider() {
            _currentValue = (int)_stackSlider.value;
            _stackIField.SetTextWithoutNotify(_currentValue.ToString());
        }

        public void OnTextField() {
            int fieldCurrent = int.Parse(_stackIField.text);
            if (fieldCurrent > _maxValue) {
                fieldCurrent = _maxValue;
            }
            if(fieldCurrent == 0) {
                fieldCurrent = 1;
            }
            _currentValue = fieldCurrent;
            SetVisuals();
        }        

        public void FilterTextField() {
            _stackIField.SetTextWithoutNotify(IFieldFilter.Filter(_stackIField.text));
        }

        public void Accepted() {
            OnAccept?.Invoke(_currentValue);
        }

        public void Cancelled() {
            OnDecline?.Invoke();
        }

        private void SetVisuals() {
            _stackSlider.value = _currentValue;
            _stackSlider.SetValueWithoutNotify(_currentValue);
            _stackIField.SetTextWithoutNotify(_currentValue.ToString());
        }
    }

}