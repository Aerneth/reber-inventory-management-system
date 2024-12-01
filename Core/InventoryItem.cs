namespace InventoryManagement.Core
{
    public class InventoryItem
    {
        public string ItemId { get; private set; }
        public required string ItemName { get; set; }

        public string Category { get; set; }

        public uint Quantity { get; set; }

        public decimal Price { get; set; }

        public uint? MinStock { get; set; }

        public uint? MaxStock { get; set; }

        public InventoryItem()
        {
            ItemId = Guid.NewGuid().ToString();
        }

        /*This method is necessary for loading the inventory from a file (down in the LoadInventoryFromFile method). I want the ItemId
        to be tightly controlled so it can only be set at item creation OR when this method is EXPLICITLY called. */
        public void SetItemId(string id)
        {
            ItemId = id;
        }
        public override string ToString()
        {
            string minStockDisplay = MinStock.HasValue ? MinStock.Value.ToString() : "Not Set";
            string maxStockDisplay = MaxStock.HasValue ? MaxStock.Value.ToString() : "Not Set";

            return $"ID: {ItemId}, Name: {ItemName}, Category: {Category}, " + $"Quantity: {Quantity}, Price: {Price:C}, MinStock: {minStockDisplay}, MaxStock: {maxStockDisplay}";
        }
    }   
}