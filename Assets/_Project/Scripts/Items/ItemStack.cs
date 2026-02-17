namespace StormPig.Items {
    [System.Serializable]
    public class ItemStack {
        public Item Original;
        public int Ammount;
        
        public ItemStack(Item o, int a) {
            Original = o;
            Ammount = a;
        }
    }
}