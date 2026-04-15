using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

namespace StormPig.UI {
    public class InventoryUI : MonoBehaviour {
        [Header("Functionality")]
        [SerializeField] private HoverPanel _hoverPanel;
        [SerializeField] private SplitStackPanel _splitStackPanel;
        [SerializeField] private GraphicRaycaster raycaster;
        [SerializeField] private EventSystem eventSystem;
        [Space(5)]

        [Header("Inventory")]
        [SerializeField] private Inventories.Inventory _playerInventory;
        [SerializeField] private Transform itemContainter;
        [SerializeField] private GameObject panelInventory;
        [SerializeField] private GameObject _panelLoot;
        [Space(2)]
        [SerializeField] private Image[] cellImages;
        [SerializeField] private InventoryCell[] cells;
        [SerializeField] private ItemUI[] itemPrefabs; 
        [SerializeField] private ItemUI[] itemPreviewPrefabs;
        [Space(5)]

        [Header("Parameters")]
        [SerializeField] private Color freeSpaceColor;
        [SerializeField] private Color takenSpaceColor;
        [Space(2)]
        [SerializeField] private int gridX;
        [SerializeField] private int gridY;
        [Space(2)]
        [SerializeField] private float _hoverXOffset;
        [SerializeField] private float _hoverYOffset;

        [SerializeField] private List<ItemUI> items = new List<ItemUI>();
        private readonly List<RaycastResult> itemHits = new();

        private ItemUI _currentSelectedStack = null;
        private ItemUI _currentMovingItem = null;
        private ItemUI _currentHoveredOnItem = null;
        private ItemUI _previewInstance = null;

        private Vector3 lastItemPosition;

        private bool _splittingStack = false;
        private int _stackAmmount;

        private Inventories.Inventory _inv;

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
            _playerInventory.CreateSpace(gridX, gridY);
            _playerInventory.VisualizeItem += VisualiseItem;
            _playerInventory.VisualizeStack += VisualiseStack;
            _playerInventory.FuseAndRemove += FuseRemove;
            _splitStackPanel.Initialize(SplitStack, CancelSplitStack);
        }


        private void Update() {
            if (!panelInventory.activeInHierarchy) { return; }
            DisplayInfo();


            if(Input.GetMouseButtonDown(0) && _splittingStack && _currentMovingItem != null) {
                if (_previewInstance != null) {
                    TryPutStack();
                }
            }


            if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift)) {
                DetectStackItem();
                Global.Log.Trace("Split stack behaviour detected");
            } else if (Input.GetMouseButtonDown(0)) {
                DetectClickedUIItem();
                Global.Log.Trace("Click on item behaviour detected");             
            }

            if (_currentMovingItem != null) {
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_currentMovingItem.Rect.parent as RectTransform,  Input.mousePosition, null,   out pos );

                _currentMovingItem.Rect.anchoredPosition = pos;
            }

            if (Input.GetMouseButtonUp(0) && !_splittingStack) {
                
                if(_previewInstance != null) {
                    TryPutItem();
                }               
                _currentMovingItem = null;
            }

           

            if (_currentMovingItem != null && _previewInstance != null) {
                ItemPreview();
            }
        }

        private void DetectClickedUIItem() {
            if (_splittingStack) { return; }//disable object selection while splitting stacks
            PointerEventData pointerData = new PointerEventData(eventSystem) {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            for(int i =0; i < results.Count; i++) {
                if(results[i].gameObject.TryGetComponent(out ItemUI it)) {
                    _currentMovingItem = it;
                    lastItemPosition = _currentMovingItem.Rect.position;

                    if (_previewInstance != null) {
                        Destroy(_previewInstance.gameObject);
                    }
                    _previewInstance = Instantiate(itemPreviewPrefabs[0], itemContainter);

                    if (_previewInstance.Text.gameObject.activeInHierarchy) {
                        _previewInstance.Text.gameObject.SetActive(false);
                    }

                    _previewInstance.transform.SetAsFirstSibling();

                    _previewInstance.Icon.sprite = null;
                    _previewInstance.Icon.color = freeSpaceColor;
                    _previewInstance.Rect.anchorMin = new Vector2(0f, 1f);
                    _previewInstance.Rect.anchorMax = _previewInstance.Rect.anchorMin;
                    _previewInstance.Rect.anchoredPosition = _currentMovingItem.Rect.anchoredPosition;
                    Global.Log.Trace("Clicked on an ItemUI: " + _currentMovingItem.name);
                    if (_hoverPanel.gameObject.activeInHierarchy) {
                        _hoverPanel.gameObject.SetActive(false);
                    }
                    break;
                }
            }
        }
        private void DetectStackItem() {
            PointerEventData pointerData = new PointerEventData(eventSystem) {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            for (int i = 0; i < results.Count; i++) {
                if (results[i].gameObject.TryGetComponent(out ItemUI it)) {
                    if (_hoverPanel.gameObject.activeInHierarchy) {
                        _hoverPanel.gameObject.SetActive(false);
                    }
                    _splittingStack = true;
                    _splitStackPanel.gameObject.SetActive(true);
                    _splitStackPanel.DisplayInfo(it);
                    _currentSelectedStack = it;
                    break;
                }
            }
        }
       

        private void ItemPreview() {            
            int closest = -1;
            float dist;
            float lastDist = 10000000f;
            for (int i = 0; i < cellImages.Length; i++) {
                dist = Mathf.Abs(Vector2.Distance(cellImages[i].rectTransform.position, _currentMovingItem.Rect.position));
                if (dist < lastDist) {
                    closest = i;
                    lastDist = dist;
                }
            }

            if(closest != -1) {
                _previewInstance.Rect.anchoredPosition = cellImages[closest].rectTransform.anchoredPosition;
                _previewInstance.GridPositions = new Vector2Int[1];
                _previewInstance.GridPositions[0] = cells[closest].Position;

                for(int i =0; i < items.Count; i++) {
                    for(int j =0; j < items[i].GridPositions.Length; j++) {
                        if (items[i] != _currentMovingItem && items[i].GridPositions[j] == cells[closest].Position) {
                            _previewInstance.Icon.color = takenSpaceColor;
                            return;
                        }
                    }
                }
                if(_previewInstance.Icon.color != freeSpaceColor) {
                    _previewInstance.Icon.color = freeSpaceColor;
                }
            }
        }

        private void SplitStack(int val) {
            ItemUI it = Instantiate(_currentSelectedStack, itemContainter);
            _stackAmmount = val;

            _currentMovingItem = it;
            if (_previewInstance != null) {
                Destroy(_previewInstance.gameObject);
            }
            _previewInstance = Instantiate(itemPreviewPrefabs[0], itemContainter);

            if (_previewInstance.Text.gameObject.activeInHierarchy) {
                _previewInstance.Text.gameObject.SetActive(false);
            }

            _previewInstance.transform.SetAsFirstSibling();

            _previewInstance.Icon.sprite = null;
            _previewInstance.Icon.color = freeSpaceColor;
            _previewInstance.Rect.anchorMin = new Vector2(0f, 1f);
            _previewInstance.Rect.anchorMax = _previewInstance.Rect.anchorMin;
            _previewInstance.Rect.anchoredPosition = _currentMovingItem.Rect.anchoredPosition;
        }

        private void CancelSplitStack() {
            _splittingStack = false;
            Destroy(_currentMovingItem.gameObject);
        }

        private void TryPutStack() {
            if (_previewInstance.Icon.color == takenSpaceColor) {
               // inv.FuseStacks(_currentSelectedStack.GridPositions, _previewInstance.GridPositions);
                return;
            }

            _currentMovingItem.GridPositions = _previewInstance.GridPositions;
            _currentMovingItem.Rect.position = _previewInstance.Rect.position;

            _playerInventory.SplitStack(_currentSelectedStack.GridPositions, _stackAmmount, _currentMovingItem.GridPositions);


            Destroy(_previewInstance.gameObject);
            Destroy(_currentMovingItem.gameObject);
            _splittingStack = false;
        }

        private void FuseRemove(Vector2Int[] positions) {
            for(int i =0; i < items.Count; i++) {
                if (items[i].GridPositions.SequenceEqual(positions)) {
                    Destroy(items[i].gameObject);
                    items.RemoveAt(i);
                }
            }
             _currentMovingItem.Rect.position = lastItemPosition;
             Destroy(_previewInstance.gameObject);
        }

        private void TryPutItem() {
            if(_previewInstance.Icon.color == takenSpaceColor) {
                _playerInventory.FuseStacks(_currentMovingItem.GridPositions, _previewInstance.GridPositions);
                return;
            }


            _playerInventory.ChangeItemPosition(null, -1, _currentMovingItem.GridPositions, _previewInstance.GridPositions);
            _currentMovingItem.GridPositions = _previewInstance.GridPositions;
            _currentMovingItem.Rect.position = _previewInstance.Rect.position;

            Destroy(_previewInstance.gameObject);

            // After placing item, to make sure info is still there
            DisplayHoverPanel();
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
            Global.Log.Trace("Positions - first: " + first + "  and last: " + last);

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
                if(items[i].GridPositions.SequenceEqual(positions)) {
                    items[i].Text.SetText("{0}", ammount);
                    Global.Log.Trace("Increased UI stack ammount of: " + items[i].gameObject.name);
                    break;
                }
            }
        }

        /// <summary>
        /// Displays item information to hover panel
        /// </summary>
        private void DisplayInfo() {
            if (_splittingStack) { return; }
            if (!EventSystem.current.IsPointerOverGameObject()) { return; }

            PointerEventData data = new PointerEventData(EventSystem.current) {
                position = Input.mousePosition
            };

            raycaster.Raycast(data, itemHits);

            // below try to get item ui
            // if none is found return,
            // if we already got it return to save stuff
            ItemUI check = null;
            for (int i = 0; i < itemHits.Count; i++) {
                if(itemHits[i].gameObject.TryGetComponent(out ItemUI it)){
                    if(it == _currentHoveredOnItem) {
                        itemHits.Clear();
                        return; 
                    }
                    check = it;
                    _currentHoveredOnItem = it;
                }
            }

            itemHits.Clear();

            if (check == null) {               
                _currentHoveredOnItem = null;
                _hoverPanel.gameObject.SetActive(false);
                return;
            }
            DisplayHoverPanel();
        }

        private void DisplayHoverPanel() {
            _hoverPanel.gameObject.SetActive(true);

            _hoverPanel.Rect.position = new Vector3(
               _currentHoveredOnItem.Rect.position.x - (_currentHoveredOnItem.Rect.sizeDelta.x + _hoverPanel.Rect.sizeDelta.x / 2f + _hoverXOffset),
               _currentHoveredOnItem.Rect.position.y - (_hoverPanel.Rect.sizeDelta.y / 2f + _hoverYOffset),
               _currentHoveredOnItem.Rect.position.z);

            _hoverPanel.DisplayInfo(_playerInventory.GetItemInfo(_currentHoveredOnItem.GridPositions), _currentHoveredOnItem.Icon.sprite);
        }
    }
}