namespace InventoryManagement.Core
{
    public class InventoryItem
    {
        public Guid ItemId { get; private set; }
        public string ItemName { get; set; }

        public Guid CategoryId { get; set; }

        public uint Quantity { get; set; }

        public decimal Price { get; set; }

        public uint? MinStock { get; set; }

        public uint? MaxStock { get; set; }

        public InventoryItem(string itemName, Guid categoryId, uint quantity, decimal price, uint? minStock = null, uint? maxStock = null)
        {
            ItemId = Guid.NewGuid();
            ItemName = itemName;
            CategoryId = categoryId;
            Quantity = quantity;
            Price = price;
            MinStock = minStock;
            MaxStock = maxStock;
        }

        /*This method is necessary for loading the inventory from a file (down in the LoadInventoryFromFile method). I want the ItemId
        to be tightly controlled so it can only be set at item creation OR when this method is EXPLICITLY called. */
        public void SetItemId(Guid id)
        {
            ItemId = id;
        }
        public override string ToString()
        {
            string minStockDisplay = MinStock.HasValue ? MinStock.Value.ToString() : "Not Set";
            string maxStockDisplay = MaxStock.HasValue ? MaxStock.Value.ToString() : "Not Set";

            return $"ID: {ItemId}, Name: {ItemName}, Category: {CategoryId}, " + $"Quantity: {Quantity}, Price: {Price:C}, MinStock: {minStockDisplay}, MaxStock: {maxStockDisplay}";
        }
    }
    public class Category
    {
        public Guid CategoryId { get; private set; }
        public string CategoryName { get; set; }

        public Category(string categoryName)
        {
            CategoryId = Guid.NewGuid();
            CategoryName = categoryName;
        }

        public override string ToString()
        {
            return CategoryName;
        }
    }   
}