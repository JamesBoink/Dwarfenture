using UnityEngine;

namespace StormPig.Inventory {
    public class TEMPInvTest : MonoBehaviour {
        [SerializeField] private Inventory inv;
        [SerializeField] private Items.Item[] its;
     //   [SerializeField] private Resources.ResourceNode r;
        [ContextMenu("Add")]
        public void Add() {
            //Resources.Resource r = new Resources.Resource();
            //r.Data = this.r.resource.Data;
            //r.ResourceData = this.r.resource.ResourceData;
            //inv.TryAddItem(r);
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