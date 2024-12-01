using InventoryManagement.Core;
using InventoryManagement.Storage;

namespace InventoryManagment.UI
{
    public static class ConsoleHelper
    {
        public static string Prompt(string message)
        {
            Console.Write(message);
            return Console.ReadLine() ?? string.Empty;
        }

        public static void ShowMenu()
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
        }

        public static void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

    }
    internal static class Program
    {
        static List<InventoryItem> inventory = new List<InventoryItem>();
        static string filePath = "inventory.csv"; // Path for saving/loading inventory
        public const string mainMenuMessage = "\nPress Enter to return to the main menu.";
        public const string defaultItemNameMessage = "Default name";
        public const string defaultCategory = "Default category";
        static void Main(string[] args)
        {
            FileHandler.LoadInventoryFromFile(filePath);
            
            while (true)
            {
                ConsoleHelper.ShowMenu();

                string? choice = ConsoleHelper.Prompt("Please select an option (1-7): ");

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
                        FileHandler.SaveInventoryToFile(inventory, filePath);
                        break;
                    case "6":
                        inventory = FileHandler.LoadInventoryFromFile(filePath);
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
            InventoryItem newItem = GetItemFromUser();
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

                InventoryItem updatedItem = GetItemFromUser();

                itemToEdit.ItemName = updatedItem.ItemName;
                itemToEdit.Category = updatedItem.Category;
                itemToEdit.Quantity = updatedItem.Quantity;
                itemToEdit.Price = updatedItem.Price;
                itemToEdit.MinStock = updatedItem.MinStock;
                itemToEdit.MaxStock = updatedItem.MaxStock;

                Console.WriteLine("The item has been updated successfully.");
            }
            else
            {
                Console.WriteLine("That item was not found.");
            }

            Console.WriteLine(mainMenuMessage);
            Console.ReadLine();
        }

        private static InventoryItem GetItemFromUser()
        {
            InventoryItem newItem = new InventoryItem()
            {
                ItemName = defaultItemNameMessage,
                Category = defaultCategory,
                Quantity = 1,
                Price = 0.00m,
                MinStock = null,
                MaxStock = null
            };

            newItem.ItemName = ConsoleHelper.Prompt("Enter Item Name: ");
            _ = string.IsNullOrWhiteSpace(newItem.ItemName) ? newItem.ItemName = "Default Name" : newItem.ItemName;
            newItem.Category = ConsoleHelper.Prompt("Enter Category: ");
            _ = string.IsNullOrWhiteSpace(newItem.Category) ? newItem.Category = "Default Category" : newItem.Category;
            newItem.Quantity = uint.TryParse(ConsoleHelper.Prompt("Enter Quantity: "), out uint quantity) ? quantity : 1;
            newItem.Price = decimal.TryParse(ConsoleHelper.Prompt("Enter Price: "), out decimal price) && price >= 0 ? price : 0.00m;
            newItem.MinStock = uint.TryParse(ConsoleHelper.Prompt("(Optional) Enter Minimum Stock: "), out uint minStock) ? (uint?)minStock : null;
            newItem.MaxStock = uint.TryParse(ConsoleHelper.Prompt("(Optional) Enter Maximum Stock: "), out uint maxStock) ? (uint?)maxStock : null;

            return newItem;
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