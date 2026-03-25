using UnityEngine;
using System.Collections.Generic;
using StormPig.Items;

namespace StormPig.Inventories {
    public class Inventory : MonoBehaviour {
        private bool[][] SpaceTaken;

        [field: SerializeField] public List<Item> Items { get; private set; } = new List<Item>();
        [SerializeField] private List<ItemStack> stacks = new List<ItemStack>();

        public System.Action<Vector2Int[], Sprite, int> VisualizeItem;
        public System.Action<Vector2Int[], int> VisualizeStack;
        public System.Action<int> AcceptedAmmount;
        public void CreateSpace(int x, int y) {
            SpaceTaken = new bool[x][];

            for (int i = 0; i < SpaceTaken.Length; i++) {
                SpaceTaken[i] = new bool[y];
            }
            Global.Log.Trace("Inventory:  " + name + "  has had new space created: " + x + "x" + y);
        }

        /// <summary>
        /// Tries to add item to inventory, firstly checking if we can simply add it to a stack, if not tries to find space for item and add it
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Wheter we succedeed in item adding</returns>
        public bool TryAddItem(Item item, int ammount) {
            List<Vector2Int> freeCoords;
            Vector2Int[] foundPos;

            // First check if we can stack item            
            if (item.Data.MaxStack > 1) {
                // Look for active stack with space for our item, if we find it
                // just increase stack ammount
                for(int i = 0; i < stacks.Count; i++) {
                    if(item.Data.Name == stacks[i].Original.Data.Name && stacks[i].Ammount < item.Data.MaxStack) {
                        if(stacks[i].Ammount + ammount <= item.Data.MaxStack) {
                            stacks[i].Ammount+= ammount;
                            Global.Log.Trace("Increased stack ammount for stack:  <color=green>" + stacks[i].Original.Data.name + "</color> to: " + stacks[i].Ammount);
                            VisualizeStack?.Invoke(stacks[i].Original.InventoryPosition, stacks[i].Ammount);
                            return true;
                        } else {
                            int acceptedAmmount = item.Data.MaxStack - ammount;
                            stacks[i].Ammount += acceptedAmmount;
                            Global.Log.Trace("Increased stack ammount for stack:  <color=green>" + stacks[i].Original.Data.name + "</color> to: " + stacks[i].Ammount);
                            VisualizeStack?.Invoke(stacks[i].Original.InventoryPosition, stacks[i].Ammount);


                            if (!CheckFreeSpace(item, out freeCoords)) {
                                AcceptedAmmount?.Invoke(acceptedAmmount); //If we recieve a stack but cant take entire thing, callbak to say how much we took
                                return false;
                            }

                            if (FindSpaceForItem(item.Data.InventorySpaceTaken, freeCoords, out foundPos)) {
                                stacks.Add(new ItemStack(item, ammount- acceptedAmmount));
                                item.InventoryPosition = foundPos;
                                Items.Add(item);
                                Global.Log.Trace("Added item:  <color=green>" + item.Data.name + "</color>  to inventory:  " + name + " as a stack");
                                VisualizeItem?.Invoke(foundPos, item.Data.UIIcon, stacks[stacks.Count - 1].Ammount);
                                return true;
                            }
                            AcceptedAmmount?.Invoke(acceptedAmmount);  //If we recieve a stack but cant take entire thing, callbak to say how much we took
                        }
                    }
                }

                // If not create new stack at position
                // and add as item
                if (!CheckFreeSpace(item, out freeCoords)) {
                    return false;
                }

                if (FindSpaceForItem(item.Data.InventorySpaceTaken, freeCoords, out foundPos)) {
                    stacks.Add(new ItemStack(item, ammount));
                    item.InventoryPosition = foundPos;
                    Items.Add(item);
                    Global.Log.Trace("Added item:  <color=green>" + item.Data.name + "</color>  to inventory:  " + name + " as a stack");
                    VisualizeItem?.Invoke(foundPos, item.Data.UIIcon, stacks[stacks.Count - 1].Ammount);
                    return true;
                }
            } else {
                //Single, non-stackable item
                if (!CheckFreeSpace(item, out freeCoords)) {
                    return false;
                }


                if (FindSpaceForItem(item.Data.InventorySpaceTaken, freeCoords, out foundPos)) {
                    item.InventoryPosition = foundPos;
                    Items.Add(item);
                    Global.Log.Trace("Added item:  <color=green>" + item.Data.name + "</color>  to inventory:  " + name);
                    VisualizeItem?.Invoke(foundPos, item.Data.UIIcon, -1);
                    return true;
                }              
            }

            return false;
        }

        /// <summary>
        /// Removes an item from inventory, firstly trying to see if we just remove one item from stack or just a single item
        /// </summary>
        /// <param name="sourcePos"></param>
        public void RemoveItem(Vector2Int[] sourcePos, int ammount) {
            Item item = null;
            for (int i =0; i < Items.Count; i++) {
                if(Items[i].InventoryPosition == sourcePos) {
                    item = Items[i];
                    break;
                }
            }
            if(item == null) {
                Global.Log.Critical("Item to remove not found!!! There is desynchronization in Inventory <-> UI  systems");
                return;
            }

            if (item.Data.MaxStack > 1) {
                // Look for active stack with space for our item, if we find
                // just decrease stack ammount
                for (int i = 0; i < stacks.Count; i++) {
                    if (stacks[i].Original == item) {
                        
                        stacks[i].Ammount -= ammount;

                        Global.Log.Trace("Decreased stack ammount for stack:  <color=green>" + stacks[i].Original.Data.name + "</color>   for inventory:  " + name);
                        if (stacks[i].Ammount <= 0) {

                            for (int j = 0; j < stacks[i].Original.InventoryPosition.Length; j++) {
                                SpaceTaken[stacks[i].Original.InventoryPosition[j].x][stacks[i].Original.InventoryPosition[j].y] = false;
                            }

                            Global.Log.Trace("Removed stack of items:  <color=green>" + stacks[i].Original.Data.name + "</color>  from inventory:  " + name);
                            Items.Remove(item);
                            stacks.RemoveAt(i);
                        }
                        break;
                    }
                }
            } else {
                for (int i = 0; i < item.InventoryPosition.Length; i++) {
                    SpaceTaken[item.InventoryPosition[i].x][item.InventoryPosition[i].y] = false;
                }
                Items.Remove(item);
                Global.Log.Trace("Removed item:  <color=green>" + item.Data.name + "</color>  from inventory:  " + name);
            }
        }

        /// <summary>
        /// Returns item info based on passed positions
        /// </summary>
        /// <param name="positions"></param>
        /// <returns></returns>
        public string[] GetItemInfo(Vector2Int[] positions) {
            for(int i =0; i < Items.Count; i++) {
                if(Items[i].InventoryPosition == positions) {

             
                    // Depending on the items additional parameters
                    // either we send typical info
                    // or also add additional info at the end of array
                    string[] info;
                    if (Items[i].Data.AdditionalParameters == null) {
                        info = new string[5];
                    } else {
                        info = new string[5 + (Items[i].Data.AdditionalParameters.Length*2)];
                        int currentIndex = 5;
                        for(int x =0; x < Items[i].Data.AdditionalParameters.Length; x++) {
                            info[currentIndex] = Items[i].Data.AdditionalParameters[x].Type.ToString();
                            currentIndex++;
                            info[currentIndex] = Items[i].Data.AdditionalParameters[x].Value.ToString();
                            currentIndex++;
                        }
                    }
                    info[0] = Items[i].Data.Name;
                    info[1] = Items[i].Data.Description;

                    //if item is singular just display its weight
                    if(Items[i].Data.MaxStack == 1) {
                        info[2] = Items[i].Data.Weight + " kg";
                    } else {
                        //if not find the stack and display total stack weight
                        for(int j =0; j < stacks.Count; j++) {
                            if (stacks[j].Original == Items[i]) {
                                float totalWeight = Items[i].Data.Weight * stacks[j].Ammount;
                                info[2] = totalWeight + " kg";
                                break;
                            }
                        }                        
                    }
                   
                    info[3] = Items[i].Data.Type.ToString();
                    info[4] = Items[i].Data.Quality.ToString();

                    Global.Log.Trace("Sending info to UI about: <color=green>" + Items[i].Data.name + "</color>  from inventory:  " + name);
                    Global.Log.Warning("Carfull, hardcoded values in this method. We assume 5 base informations, might be less or more right now");
                    return info;
                }
            }
            Global.Log.Critical("The item cannot be found in inventory: " + name + ". Something deleted the item without a cleanup and it persits in UI!");
            return null;
        }


        /// <summary>
        /// Splits stack based by original item by passed ammount and creates a new one that steals set ammount of items from previous stack
        /// </summary>
        /// <param name="sourcePos"></param>
        /// <param name="ammount"></param>
        /// <param name="newPosition"></param>
        public void SplitStack(Vector2Int[] sourcePos, int ammount, Vector2Int[] newPosition) {
            Item item = null;
            for (int i = 0; i < Items.Count; i++) {
                if (Items[i].InventoryPosition == sourcePos) {
                    item = Items[i];
                    break;
                }
            }
            if (item == null) {
                Global.Log.Critical("Item stack to split has not been found in inventory: " + name + ". Something deleted the item without a cleanup!");
                return;
            }
            for (int i = 0; i < stacks.Count; i++) {
                if (stacks[i].Original == item) {
                    Item newItem = new Item(item.Data);
                    newItem.InventoryPosition = newPosition;

                    Items.Add(newItem);
                    stacks.Add(new ItemStack(newItem, ammount));

                    stacks[i].Ammount -= ammount;

                    for (int z = 0; z < stacks[stacks.Count - 1].Original.InventoryPosition.Length; z++) {
                        SpaceTaken[stacks[stacks.Count - 1].Original.InventoryPosition[z].x][stacks[stacks.Count - 1].Original.InventoryPosition[z].y] = true;
                    }
                    Global.Log.Trace("Split off a new stack of items:  <color=green>" + stacks[stacks.Count - 1].Original.Data.name + "</color> from stack:  <color=green>" + stacks[i].Original.Data.name + "</color>  in inventory:  " + name);
                    break;
                }
            }
        }

        //public void FuseStacks(Vector2Int[] giverPos, int ammount, Vector2Int[] recieverPos) {
        //    Item item = null;
        //    for (int i = 0; i < Items.Count; i++) {
        //        if (Items[i].InventoryPosition == giverPos) {
        //            item = Items[i];
        //            break;
        //        }
        //    }
        //    if (item == null) {
        //        Global.Log.Critical("Item stack to split has not been found in inventory: " + name + ". Something deleted the item without a cleanup!");
        //        return;
        //    }

        //    for (int i = 0; i < stacks.Count; i++) {
        //        if (item.Data.Name == stacks[i].Original.Data.Name && stacks[i].Ammount < item.Data.MaxStack) {
        //            if (stacks[i].Ammount + ammount <= item.Data.MaxStack) {
        //                stacks[i].Ammount += ammount;
        //                Global.Log.Trace("Increased stack ammount for stack:  <color=green>" + stacks[i].Original.Data.name + "</color> to: " + stacks[i].Ammount);
        //                VisualizeStack?.Invoke(stacks[i].Original.InventoryPosition, stacks[i].Ammount);
        //                return true;
        //            } else {
        //                int acceptedAmmount = item.Data.MaxStack - ammount;
        //                stacks[i].Ammount += acceptedAmmount;
        //                Global.Log.Trace("Increased stack ammount for stack:  <color=green>" + stacks[i].Original.Data.name + "</color> to: " + stacks[i].Ammount);
        //                VisualizeStack?.Invoke(stacks[i].Original.InventoryPosition, stacks[i].Ammount);


        //                if (!CheckFreeSpace(item, out freeCoords)) {
        //                    AcceptedAmmount?.Invoke(acceptedAmmount); //If we recieve a stack but cant take entire thing, callbak to say how much we took
        //                    return false;
        //                }

        //                if (FindSpaceForItem(item.Data.InventorySpaceTaken, freeCoords, out foundPos)) {
        //                    stacks.Add(new ItemStack(item, ammount - acceptedAmmount));
        //                    item.InventoryPosition = foundPos;
        //                    Items.Add(item);
        //                    Global.Log.Trace("Added item:  <color=green>" + item.Data.name + "</color>  to inventory:  " + name + " as a stack");
        //                    VisualizeItem?.Invoke(foundPos, item.Data.UIIcon, stacks[stacks.Count - 1].Ammount);
        //                    return true;
        //                }
        //                AcceptedAmmount?.Invoke(acceptedAmmount);  //If we recieve a stack but cant take entire thing, callbak to say how much we took
        //            }
        //        }
        //    }

        //    // If not create new stack at position
        //    // and add as item
        //    if (!CheckFreeSpace(item, out freeCoords)) {
        //        return false;
        //    }

        //    if (FindSpaceForItem(item.Data.InventorySpaceTaken, freeCoords, out foundPos)) {
        //        stacks.Add(new ItemStack(item, ammount));
        //        item.InventoryPosition = foundPos;
        //        Items.Add(item);
        //        Global.Log.Trace("Added item:  <color=green>" + item.Data.name + "</color>  to inventory:  " + name + " as a stack");
        //        VisualizeItem?.Invoke(foundPos, item.Data.UIIcon, stacks[stacks.Count - 1].Ammount);
        //        return true;
        //    }
        //}

        /// <summary>
        /// Allows changing existing items position as well as adding it manually to inventory by dragging it
        /// </summary>
        /// <param name="source"></param>
        /// <param name="oldCoords"></param>
        /// <param name="newCoords"></param>
        public void ChangeItemPosition(Inventory source, int stackAmmount, Vector2Int[] oldCoords, Vector2Int[] newCoords) {

            Item item = null;
            if (source == null) {
                for (int i = 0; i < Items.Count; i++) {
                    if (Items[i].InventoryPosition == oldCoords) {
                        item = Items[i];
                        break;
                    }
                }
                if (item == null) {
                    Global.Log.Critical("Error! The move-in-inventory item has not been found in inventory: " + name + ". Something deleted the item without a cleanup!");
                    return;
                }
            } else {
                for (int i = 0; i < source.Items.Count; i++) {
                    if (source.Items[i].InventoryPosition == oldCoords) {
                        item = source.Items[i];
                        source.RemoveItem(oldCoords, 10000);
                        break;
                    }
                }
                if(item == null) {
                    Global.Log.Critical("Error! The pass-from-inventory-to-inventory item has not been found in source: " + source.name + ". Either the source is wrongly provided or the item ceased to exist!");
                    return;
                }

                Items.Add(item);

                Global.Log.Trace("Item:  <color=green>" + item.Data.name + "</color>  seems to have been dragged manually to inventory: " + name + " as new item");
                if (item.Data.MaxStack > 1) {
                    stacks.Add(new ItemStack(item, stackAmmount));
                    Global.Log.Trace("Freshly dragged item:  <color=green>" + item.Data.name + "</color>  has created a stack in inventory: " + name);
                }

            }

            for (int i = 0; i < item.InventoryPosition.Length; i++) {
                SpaceTaken[item.InventoryPosition[i].x][item.InventoryPosition[i].y] = false;
            }

            item.InventoryPosition = newCoords;

            for (int i = 0; i < item.InventoryPosition.Length; i++) {
                SpaceTaken[item.InventoryPosition[i].x][item.InventoryPosition[i].y] = true;
            }

            Global.Log.Trace("Changed position of item:  <color=green>" + item.Data.name + "</color>  in inventory:  " + name);
        }

        /// <summary>
        /// Checks if there is enough cells in inventory for item and returns a list of free spaces
        /// </summary>
        /// <param name="item"></param>
        /// <param name="freeCoords"></param>
        /// <returns>Whether enough space has been found</returns>
        private bool CheckFreeSpace(Item item, out List<Vector2Int> freeCoords) {
            int freeSpace = 0;
            freeCoords = new List<Vector2Int>();
            for (int i = 0; i < SpaceTaken.Length; i++) {
                for (int j = 0; j < SpaceTaken[i].Length; j++) {
                    if (!SpaceTaken[i][j]) {
                        freeSpace++;
                        freeCoords.Add(new Vector2Int(i, j));
                    }
                }
            }

            if (freeSpace < (item.Data.InventorySpaceTaken.x * item.Data.InventorySpaceTaken.y)) {
                Global.Log.Trace("Not enough space in inventory:  " + name + "  for item:  <color=green>" + item.Data.name + "</color>");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Finds exact cell space for new item
        /// </summary>
        /// <param name="spaceTaken"></param>
        /// <param name="freeCoords"></param>
        /// <param name="foundPositions"></param>
        /// <returns>Whether specific space for item has been found</returns>
        private bool FindSpaceForItem(Vector2Int spaceTaken, List<Vector2Int> freeCoords, out Vector2Int[] foundPositions) {
            // Try original orientation
            if (TryFindSpace(spaceTaken, freeCoords, out foundPositions)) {
                return true;
            }

            // Try rotated orientation if we don't have the same sT
            if(spaceTaken.x != spaceTaken.y) {
                Vector2Int rotated = new Vector2Int(spaceTaken.y, spaceTaken.x);
                if (TryFindSpace(rotated, freeCoords, out foundPositions)) {
                    return true;
                }
            }            

            Global.Log.Trace("Not enough space found for item in any orientation.");
            return false;
        }

        /// <summary>
        /// Tries to find space in inventory based on passed coordinates
        /// </summary>
        /// <param name="size"></param>
        /// <param name="freeCoords"></param>
        /// <param name="foundPositions"></param>
        /// <returns></returns>
        private bool TryFindSpace(Vector2Int size, List<Vector2Int> freeCoords, out Vector2Int[] foundPositions) {
            for (int i = 0; i < freeCoords.Count; i++) {
                Vector2Int start = freeCoords[i];
                bool fits = true;

                // Check if item would go out of inventory bounds
                if (start.x + size.x > SpaceTaken.Length || start.y + size.y > SpaceTaken[0].Length) {
                    continue;
                }

                // Check all tiles in the intended rectangle area
                for (int x = 0; x < size.x; x++) {
                    for (int y = 0; y < size.y; y++) {
                        if (SpaceTaken[start.x + x][start.y + y]) {
                            fits = false;
                            break;
                        }
                    }
                    if (!fits)
                        break;
                }

                if (fits) {
                    if(size.x > size.y) {
                        foundPositions = new Vector2Int[size.x];
                    } else {
                        foundPositions = new Vector2Int[size.y];
                    }
                    int indexer = 0;
                    for (int x = 0; x < size.x; x++) {
                        for (int y = 0; y < size.y; y++) {
                            Vector2Int pos = new Vector2Int(start.x + x, start.y + y);
                            foundPositions[indexer] = pos;
                            SpaceTaken[pos.x][pos.y] = true;
                            indexer++;
                            Global.Log.Trace("Found space for item in inventory at: " + pos.x + "." + pos.y);
                        }
                    }

                    return true;
                }
            }

            foundPositions = null;
            return false;
        }
    }
}