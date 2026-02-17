using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

namespace StormPig.UI {
    public class InventoryUI : MonoBehaviour {
        // [SerializeField] private GridLayoutGroup group;
        [SerializeField] private GraphicRaycaster raycaster;
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private Inventory.Inventory inv;
        [SerializeField] private Image[] cellImages;
        [SerializeField] private InventoryCell[] cells;
        [SerializeField] private ItemUI[] itemPrefabs;
        [SerializeField] private Transform itemContainter;
        [SerializeField] private Color freeSpaceColor;
        [SerializeField] private Color takenSpaceColor;
        [SerializeField] private int gridX;
        [SerializeField] private int gridY;
        [SerializeField] private TextMeshProUGUI[] texts;
        [SerializeField] private Image infoIcon;

        private List<ItemUI> items = new List<ItemUI>();
        private readonly List<RaycastResult> itemHits = new();

        private ItemUI currentMovingItem = null;
        [SerializeField] private ItemUI currentHoveredOnItem = null;
        private ItemUI previewInstance = null;

        private Vector3 lastItemPosition;

        private void Awake() {
            cells = new InventoryCell[cellImages.Length];

            int x = 0;
            int y = 0;
            for(int i =0; i < cells.Length; i++) {
                cells[i] = new InventoryCell(new Vector2Int(x, y));
                x++;
                if(x == gridX) {
                    x = 0;
                    y++;
                }
            }
            inv.CreateSpace(gridX, gridY);
            inv.VisualizeItem += VisualiseItem;
            inv.VisualizeStack += VisualiseStack;
        }


        private void Update() {
            DisplayInfo();

            if (Input.GetMouseButtonDown(0)) {
                DetectClickedUIItem();
            }

            if (Input.GetMouseButton(0) && currentMovingItem != null) {
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(currentMovingItem.Rect.parent as RectTransform,  Input.mousePosition, null,   out pos );

                currentMovingItem.Rect.anchoredPosition = pos;
            }

            if (Input.GetMouseButtonUp(0)) {
     
                if (previewInstance != null) {
                    TryPutItem();                 
                }
                currentMovingItem = null;
            }

            if(currentMovingItem != null && previewInstance != null) {
                ItemPreview();
            }
        }

        private void DetectClickedUIItem() {
            PointerEventData pointerData = new PointerEventData(eventSystem) {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            for(int i =0; i < results.Count; i++) {
                if(results[i].gameObject.TryGetComponent(out ItemUI it)) {
                    currentMovingItem = it;
                    lastItemPosition = currentMovingItem.Rect.position;

                    if (previewInstance != null) {
                        Destroy(previewInstance.gameObject);
                    }
                    previewInstance = Instantiate(it, itemContainter);

                    if (previewInstance.Text.gameObject.activeInHierarchy) {
                        previewInstance.Text.gameObject.SetActive(false);
                    }

                    previewInstance.transform.SetAsFirstSibling();

                    previewInstance.Icon.sprite = null;
                    previewInstance.Icon.color = freeSpaceColor;
                    previewInstance.Rect.anchorMin = new Vector2(0f, 1f);
                    previewInstance.Rect.anchorMax = previewInstance.Rect.anchorMin;
                    previewInstance.Rect.anchoredPosition = currentMovingItem.Rect.anchoredPosition;
                    Global.Log.Trace("Clicked on an ItemUI: " + currentMovingItem.name);
                    break;
                }
            }
        }

        private void ItemPreview() {            
            int closest = -1;
            float dist;
            float lastDist = 10000000f;
            for (int i = 0; i < cellImages.Length; i++) {
                dist = Mathf.Abs(Vector2.Distance(cellImages[i].rectTransform.position, currentMovingItem.Rect.position));
                if (dist < lastDist) {
                    closest = i;
                    lastDist = dist;
                }
            }

            if(closest != -1) {
                previewInstance.Rect.anchoredPosition = cellImages[closest].rectTransform.anchoredPosition;
                previewInstance.GridPositions = new Vector2Int[1];
                previewInstance.GridPositions[0] = cells[closest].Position;

                for(int i =0; i < items.Count; i++) {
                    for(int j =0; j < items[i].GridPositions.Length; j++) {
                        if (items[i] != currentMovingItem && items[i].GridPositions[j] == cells[closest].Position) {
                            previewInstance.Icon.color = takenSpaceColor;
                            return;
                        }
                    }
                }
                if(previewInstance.Icon.color != freeSpaceColor) {
                    previewInstance.Icon.color = freeSpaceColor;
                }
            }
        }

        private void TryPutItem() {
            if(previewInstance.Icon.color == takenSpaceColor) {
                currentMovingItem.Rect.position = lastItemPosition;
                Destroy(previewInstance.gameObject);
                return;
            }


            inv.ChangeItemPosition(null, -1, currentMovingItem.GridPositions, previewInstance.GridPositions);
            currentMovingItem.GridPositions = previewInstance.GridPositions;
            currentMovingItem.Rect.position = previewInstance.Rect.position;

            Destroy(previewInstance.gameObject);
        }

        private void VisualiseItem(Vector2Int[] positions, Sprite sprite, int ammount) {
            ItemUI newItem = Instantiate(itemPrefabs[0], itemContainter);            
            items.Add(newItem);

            int first = -1;
            int last = -1;
           
            for (int i = 0; i < cells.Length; i++) {
                if (first == -1 && positions[0] == cells[i].Position) {
                    first = i;
                }
                if (last == -1 && positions[positions.Length - 1] == cells[i].Position) {
                    last = i;
                    break;
                }
            }

            if (positions.Length == 1) {
                last = first;
            }
            Global.Log.Trace("Weird positions maybe? First: " + first + "  and last: " + last);

            newItem.Rect.position = (cellImages[first].rectTransform.position + cellImages[last].rectTransform.position) / 2f;
            newItem.GridPositions = positions;
            newItem.Icon.sprite = sprite;
            if(ammount != -1) {
                if (!newItem.Text.gameObject.activeInHierarchy) {
                    newItem.Text.gameObject.SetActive(true);
                }
                newItem.Text.SetText("{0}", ammount);
            } else {
                newItem.Text.gameObject.SetActive(false);
            }
        }

        private void VisualiseStack(Vector2Int[] positions, int ammount) {
            for(int i = 0; i < items.Count;i++) {
               

                if(items[i].GridPositions == positions) {
                    items[i].Text.SetText("{0}", ammount);
                    Global.Log.Trace("Increased UI stack ammount of: " + items[i].gameObject.name);
                    break;
                }
            }
        }

        private void DisplayInfo() {
            if (!EventSystem.current.IsPointerOverGameObject()) { return; }

            PointerEventData data = new PointerEventData(EventSystem.current) {
                position = Input.mousePosition
            };

            raycaster.Raycast(data, itemHits);

            ItemUI check = null;
            for (int i = 0; i < itemHits.Count; i++) {
                if(itemHits[i].gameObject.TryGetComponent(out ItemUI it)){
                    if(it == currentHoveredOnItem) {
                        itemHits.Clear();
                        return; 
                    }
                    check = it;
                    currentHoveredOnItem = it;
                }
            }

            itemHits.Clear();

            if (check == null) {               
                for (int i = 0; i < texts.Length; i++) {
                    texts[i].text = "";
                }
                currentHoveredOnItem = null;
                infoIcon.sprite = null;
                return;
            }
 
            string[] info = inv.GetItemInfo(currentHoveredOnItem.GridPositions);
            for(int i =0; i < info.Length; i++) {
                texts[i].text = info[i];
            }
            infoIcon.sprite = currentHoveredOnItem.Icon.sprite;
        }
    }
}