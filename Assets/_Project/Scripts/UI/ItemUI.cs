using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace StormPig {
    public class ItemUI : MonoBehaviour {
        [SerializeField] public RectTransform Rect;
        [SerializeField] public Image Icon;
        [SerializeField] public TextMeshProUGUI Text;
        [SerializeField] public Vector2Int[] GridPositions;
    }
}