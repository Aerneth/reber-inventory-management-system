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
                    writer.WriteLine($"{item.ItemId},{item.ItemName},{item.Category},{item.Quantity},{item.Price},{item.MinStock},{item.MaxStock}");
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
                    InventoryItem item = new InventoryItem
                    {
                        ItemName = columns[1],
                        Category = columns[2],
                        Quantity = uint.Parse(columns[3]),
                        Price = decimal.Parse(columns[4])
                    };

                    //This is separated out due to the need for tight control over when the UUID of the item can be edited.
                    item.SetItemId(columns[0]);

                    inventory.Add(item);
                }
            }
            return inventory;
        }
    }
}