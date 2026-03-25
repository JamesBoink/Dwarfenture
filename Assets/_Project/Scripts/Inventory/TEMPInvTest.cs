using UnityEngine;

namespace StormPig.Inventory {
    public class TEMPInvTest : MonoBehaviour {
        [SerializeField] private Inventory inv;
        [SerializeField] private Items.Item[] its;
        [ContextMenu("Add")]
        public void Add() {
            Items.Item i = new Items.Item();
            i.Data = its[0].Data;
            inv.TryAddItem(i, 1);
        }

        [ContextMenu("Delete")]
        private void Delete() {
        //    inv.RemoveItem(its[its.Length - 1]);
        }

        [ContextMenu("RemoveStack")]
        private void Rs() {
     //       inv.RemoveStack(its[0]);
        }
    }
}