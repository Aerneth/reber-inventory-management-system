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
                Console.WriteLine("5. Exit");
                Console.Write("Please select an option (1-5): ");
        }

        public static void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

    }
    internal static class Program
    {
        static List<InventoryItem> inventory = new List<InventoryItem>();
        //static string filePath = "inventory.csv"; // Path for saving/loading inventory
        public const string mainMenuMessage = "\nPress Enter to return to the main menu.";
        public const string defaultItemNameMessage = "Default name";
        public const string defaultCategory = "Default category";
        public const string invalidInput = "Invalid input; Setting to default value.";
        static void Main(string[] args)
        {
            FileHandler.InitializeDatabase();
            
            while (true)
            {
                ConsoleHelper.ShowMenu();

                string? choice = ConsoleHelper.Prompt("Please select an option (1-5): ");

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

            var inventoryFromDb = FileHandler.GetAllInventoryItems();

            if (inventoryFromDb.Count == 0)
            {
                Console.WriteLine("There are not currently any items in the inventory.");
            }
            else
            {
                foreach (var item in inventoryFromDb)
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
            FileHandler.InsertInventoryItem(newItem);
            Console.WriteLine(mainMenuMessage);
            Console.ReadLine();
        }
        static void EditItem()
        {
            Console.Clear();
            Console.WriteLine("Edit Item");

            Guid id = GetValidGuid("Enter the ID of the item you wish to edit: ");

            InventoryItem? itemToEdit = FileHandler.GetInventoryItemById(id);

            if (itemToEdit != null)
            {
                Console.WriteLine($"Editing Item: {itemToEdit.ItemName}");

                Console.WriteLine("Leave fields blank to keep the current value.");

                string itemName = ConsoleHelper.Prompt($"Enter new name (Current: {itemToEdit.ItemName}): ").Trim();
                string categoryName = ConsoleHelper.Prompt($"Enter new category name (Current: {itemToEdit.CategoryId}): ").Trim();
                uint? quantity = GetOptionalUInt($"Enter new quantity (Current: {itemToEdit.Quantity}): ");
                decimal? price = GetValidDecimal($"Enter new price (Current: {itemToEdit.Price}): ");
                uint? minStock = GetOptionalUInt($"Enter new minimum stock (Current: {itemToEdit.MinStock ?? 0}): ");
                uint? maxStock = GetOptionalUInt($"Enter new maximum stock (Current: {itemToEdit.MaxStock ?? 0}): ");

                if (!string.IsNullOrWhiteSpace(itemName)) itemToEdit.ItemName = itemName;
                if (!string.IsNullOrWhiteSpace(categoryName))
                    itemToEdit.CategoryId = GetOrCreateCategoryId(categoryName);
                if (quantity.HasValue) itemToEdit.Quantity = quantity.Value;
                if (price.HasValue) itemToEdit.Price = price.Value;
                if (minStock.HasValue) itemToEdit.MinStock = minStock;
                if (maxStock.HasValue) itemToEdit.MaxStock = maxStock;

                FileHandler.UpdateInventoryItem(itemToEdit);

                Console.WriteLine("Item updated successfully.");
            }
            else
            {
                Console.WriteLine("That item was not found.");
            }

            Console.WriteLine("\nPress Enter to return to the main menu.");
            Console.ReadLine();
        }

        static void RemoveItem()
        {
            Console.Clear();
            Console.WriteLine("Remove Item");

            Guid? id = GetValidGuid("Enter the ID of the item you wish to remove: ");

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

        private static InventoryItem GetItemFromUser()
        {

            string itemName = ConsoleHelper.Prompt("Enter Item Name: ").Trim();
            string categoryName = ConsoleHelper.Prompt("Enter Category Name: ").Trim();
            Guid categoryId = GetOrCreateCategoryId(categoryName);
            uint quantity = GetValidUInt("Enter Quantity: ");
            decimal price = GetValidDecimal("Enter Price: ");
            uint? minStock = GetOptionalUInt("(Optional) Enter Minimum Stock: ");
            uint? maxStock = GetOptionalUInt("(Optional) Enter Maximum Stock: ");

            if (minStock.HasValue && maxStock.HasValue && minStock.Value > maxStock.Value)
            {
                Console.WriteLine("Error: Minimum stock cannot be greater than maximum stock.");

                minStock = GetOptionalUInt("(Optional) Enter Minimum Stock: ");
                maxStock = GetOptionalUInt("(Optional) Enter Maximum Stock: ");
            }

            InventoryItem newItem = new InventoryItem(itemName, categoryId, quantity, price, minStock, maxStock);

            return newItem;
        }
        
        private static Guid GetOrCreateCategoryId(string categoryName)
        {
            // (replace this with actual category data)
            var categories = new Dictionary<string, Guid>();

            if (!categories.ContainsKey(categoryName))
            {
                // If category doesn't exist, create a new GUID for it and add it to the dictionary
                var categoryId = Guid.NewGuid();
                categories.Add(categoryName, categoryId);
                Console.WriteLine($"New category '{categoryName}' created with ID: {categoryId}");
            }
            else
            {
                Console.WriteLine($"Category '{categoryName}' already exists.");
            }

            return categories[categoryName];
        }
        private static Guid GetValidGuid(string prompt)
        {
            while (true)
            {
                if (Guid.TryParse(ConsoleHelper.Prompt(prompt), out Guid value))
                {
                    return value;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }
        }

        private static uint GetValidUInt(string prompt)
        {
            while (true)
            {
                if (uint.TryParse(ConsoleHelper.Prompt(prompt), out uint value))
                {
                    return value;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }
        }

        private static decimal GetValidDecimal(string prompt)
        {
            while (true)
            {
                if (decimal.TryParse(ConsoleHelper.Prompt(prompt), out decimal value) && value >= 0)
                {
                    return value;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }
        }

        private static uint? GetOptionalUInt(string prompt)
        {
            while (true)
            {
                string input = ConsoleHelper.Prompt(prompt).Trim();

                if (string.IsNullOrWhiteSpace(input))
                {
                    return null;
                }


                if (uint.TryParse(input, out uint result) && result >= 0)
                {
                    return result;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid non-negative number.");
                }
            }
        }
    }
}