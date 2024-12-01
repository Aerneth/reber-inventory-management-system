using System.Formats.Asn1;

namespace InventoryManagement
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

namespace InventoryManagment.UI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using InventoryManagement;

    internal static class Program
    {
        static List<InventoryManagement.InventoryItem> inventory = new List<InventoryManagement.InventoryItem>();
        static string filePath = "inventory.csv"; // Path for saving/loading inventory
        public const string mainMenuMessage = "\nPress Enter to return to the main menu.";
        public const string defaultItemNameMessage = "Item was not named at time of creation. Edit the item to add the correct name.";
        public const string defaultCategory = "Default category";
        static void Main(string[] args)
        {
            InventoryManagement.Storage.FileHandler.LoadInventoryFromFile(filePath);
            
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Welcome to RIMS!");
                Console.WriteLine("1. View Inventory");
                Console.WriteLine("2. Add Item");
                Console.WriteLine("3. Edit Item");
                Console.WriteLine("4. Remove Item");
                Console.WriteLine("5. Save Inventory");
                Console.WriteLine("6. Load Inventory from File");
                Console.WriteLine("7. Exit");
                Console.Write("Please select an option (1-7): ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ViewInventory();
                        break;
                    case "2":
                        AddItem();
                        break;
                    case "3":
                        EditItem();
                        break;
                    case "4":
                        RemoveItem();
                        break;
                    case "5":
                        InventoryManagement.Storage.FileHandler.SaveInventoryToFile(inventory, filePath);
                        break;
                    case "6":
                        inventory = InventoryManagement.Storage.FileHandler.LoadInventoryFromFile(filePath);
                        break;
                    case "7":
                        Console.WriteLine("Exiting RIMS...");
                        return;
                    default:
                        Console.WriteLine("Invalid selection. Please try again.");
                        break;
                }
            }
        }
    

        static void ViewInventory()
        {
            Console.Clear();
            Console.WriteLine("Current Inventory:");

            if (inventory.Count == 0)
            {
                Console.WriteLine("There are not currently any items in the inventory.");
            }
            else
            {
                foreach (var item in inventory)
                {
                    Console.WriteLine(item);
                }
            }

            Console.WriteLine(mainMenuMessage);
            Console.ReadLine();
        }

        private static void AddItem()
        {
            Console.Clear();
            Console.WriteLine("Add New Item");

            InventoryItem newItem = new InventoryItem()
            {
                ItemName = defaultItemNameMessage,
                Category = defaultCategory,
                Quantity = 1,
                Price = 0.00m,
                MinStock = null,
                MaxStock = null
            };

            Console.Write("Enter Item Name: ");
            string? nameInput = Console.ReadLine();
            newItem.ItemName = string.IsNullOrWhiteSpace(nameInput) ? defaultItemNameMessage : nameInput;

            Console.Write("Enter Category: ");
            string? categoryInput = Console.ReadLine();
            newItem.Category = string.IsNullOrWhiteSpace(categoryInput) ? defaultCategory : categoryInput;

            Console.Write("Enter Quantity: ");
            string? quantityInput = Console.ReadLine();
            if(!uint.TryParse(quantityInput, out uint quantity))
            {
                newItem.Quantity = 1;
                Console.WriteLine("Invalid input for Quantity. Defaulting to a value of 1.");
            }
            else
            {
                newItem.Quantity = quantity;
            }

            Console.Write("Enter Price: ");
            string? priceInput = Console.ReadLine();
            if(!decimal.TryParse(priceInput, out decimal price) || price < 0)
            {
                newItem.Price = 0.00m;
                Console.WriteLine("Invalid input for Price. Defaulting to a value of 0.00");
            }
            else
            {
                newItem.Price = price;
            }

            Console.Write("(Optional) Enter Minimum Stock: ");
            string? minStockInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(minStockInput))
            {
                newItem.MinStock = null;
            }
            else
            {
                if (uint.TryParse(minStockInput, out uint minStock))
                {
                    newItem.MinStock = minStock;
                }
                else
                {
                    Console.WriteLine("Invalid input for Minimum Stock. Please enter a valid number.");
                }
            }

            Console.Write("(Optional) Enter Maximum Stock: ");
            string? maxStockInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(maxStockInput))
            {
                newItem.MaxStock = null;
            }
            else
            {
                if (uint.TryParse(maxStockInput, out uint maxStock))
                {
                    newItem.MaxStock = maxStock;
                }
                else
                {
                    Console.WriteLine("You've entered an invalid input for Maximum Stock. Please enter a valid number.");
                }
            }

            inventory.Add(newItem);
            Console.WriteLine("Item added successfully!");
            Console.WriteLine(mainMenuMessage);
            Console.ReadLine();
        }

        static void EditItem()
        {
            Console.Clear();
            Console.WriteLine("Edit Item");

            Console.Write("Enter the ID of the item you wish to edit: ");
            string? id = Console.ReadLine();

            var itemToEdit = inventory.Find(item => item.ItemId == id);
            if (itemToEdit != null)
            {
                Console.WriteLine($"Editing Item: {itemToEdit.ItemName}");
                Console.Write("Enter new Name (leave empty to keep the current name): ");
                string? name = Console.ReadLine();
                if (!string.IsNullOrEmpty(name)) itemToEdit.ItemName = name;

                Console.Write("Enter new Category (leave empty to keep current category): ");
                string? category = Console.ReadLine();
                if (!string.IsNullOrEmpty(category)) itemToEdit.Category = category;

                Console.Write("Enter new Quantity (leave empty to keep current quantity): ");
                string? quantity = Console.ReadLine();
                if (!string.IsNullOrEmpty(quantity)) itemToEdit.Quantity = uint.Parse(quantity);

                Console.Write("Enter new Price (leave empty to keep current price): ");
                string? price = Console.ReadLine();
                if (!string.IsNullOrEmpty(price)) itemToEdit.Price = decimal.Parse(price);

                Console.WriteLine("The item has been updated successfully.");
            }
            else
            {
                Console.WriteLine("That item was not found.");
            }

            Console.WriteLine(mainMenuMessage);
            Console.ReadLine();
        }
        static void RemoveItem()
        {
            Console.Clear();
            Console.WriteLine("Remove Item");

            Console.Write("Enter the ID of the item you wish to remove: ");
            string? id = Console.ReadLine();

            var itemToRemove = inventory.Find(item => item.ItemId == id);
            if (itemToRemove != null)
            {
                inventory.Remove(itemToRemove);
                Console.WriteLine("The item has been removed successfully.");
            }
            else
            {
                Console.WriteLine("That item was not found.");
            }

            Console.WriteLine(mainMenuMessage);
            Console.ReadLine();
        }
    }
}

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