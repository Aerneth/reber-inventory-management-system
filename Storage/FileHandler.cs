using InventoryManagement.Core;

namespace InventoryManagement.Storage
{
    public static class FileHandler
    {
        public static void SaveInventoryToFile(List<InventoryItem> inventory, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("ItemId,ItemName,Category,Quantity,Price,MinStock,MaxStock");
                foreach (var item in inventory)
                {
                    writer.WriteLine($"{item.ItemId},{item.ItemName},{item.CategoryId},{item.Quantity},{item.Price},{item.MinStock},{item.MaxStock}");
                }
            }

            Console.WriteLine("Your inventory has been saved successfully.");
            Console.WriteLine("\n Press Enter to return to the main menu.");
            Console.ReadLine();
        }

        public static List<InventoryItem> LoadInventoryFromFile(string filePath)
        {
            List<InventoryItem> inventory = new List<InventoryItem>();
            if (File.Exists(filePath))
            {
                string[] rows = File.ReadAllLines(filePath);
                for (int i = 1; i < rows.Length; i++)
                {
                    var columns = rows[i].Split(',');

                    Guid itemId = Guid.Parse(columns[0]); // ItemId
                    string itemName = columns[1]; // ItemName
                    Guid categoryId = Guid.Parse(columns[2]); // CategoryId
                    uint quantity = uint.Parse(columns[3]); // Quantity
                    decimal price = decimal.Parse(columns[4]); // Price
                    uint? minStock = string.IsNullOrEmpty(columns[5]) ? (uint?)null : uint.Parse(columns[5]); // MinStock
                    uint? maxStock = string.IsNullOrEmpty(columns[6]) ? (uint?)null : uint.Parse(columns[6]); // MaxStock

                    InventoryItem item = new InventoryItem(itemName, categoryId, quantity, price, minStock, maxStock);
                    
                    //This is separated out due to the need for tight control over when the UUID of the item can be edited.
                    item.SetItemId(itemId);

                    inventory.Add(item);
                }
            }
            return inventory;
        }
    }
}