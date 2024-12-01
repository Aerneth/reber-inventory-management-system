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
        public const string invalidInput = "Invalid input; Setting to default value.";
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
                ItemName = ConsoleHelper.Prompt("Enter Item Name: ").Trim(),
                Category = ConsoleHelper.Prompt("Enter Category: ").Trim(),
                Quantity = GetValidUInt("Enter Quantity: ", 1),
                Price = GetValidDecimal("Enter Price: ", 0.00m),
                MinStock = GetOptionalUInt("(Optional) Enter Minimum Stock: "),
                MaxStock = GetOptionalUInt("(Optional) Enter Maximum Stock: ")
            };

            if (newItem.MinStock.HasValue && newItem.MaxStock.HasValue && newItem.MinStock.Value > newItem.MaxStock.Value)
            {
                Console.WriteLine("Error: Minimum stock cannot be greater than maximum stock.");

                newItem.MinStock = GetOptionalUInt("(Optional) Enter Minimum Stock: ");
                newItem.MaxStock = GetOptionalUInt("(Optional) Enter Maximum Stock: ");
            }

            newItem.ItemName = string.IsNullOrEmpty(newItem.ItemName) ? "Default Name" : newItem.ItemName;
            newItem.Category = string.IsNullOrEmpty(newItem.Category) ? "Default Category" : newItem.Category;

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

        private static uint GetValidUInt(string prompt, uint defaultValue)
        {

            if (uint.TryParse(ConsoleHelper.Prompt(prompt), out uint value))
            {
                return value;
            }
            else
            {
                Console.WriteLine(invalidInput);
                return defaultValue;
            }

        }

        private static decimal GetValidDecimal(string prompt, decimal defaultValue)
        {
            if (decimal.TryParse(ConsoleHelper.Prompt(prompt), out decimal value) && value >= 0)
            {
                return value;
            }
            else
            {
                Console.WriteLine(invalidInput);
                return defaultValue;
            }
        }

        private static uint? GetOptionalUInt(string prompt)
        {
            while (true)
            {
                string input = ConsoleHelper.Prompt(prompt).Trim();

                // If the input is empty or just whitespace, return null
                if (string.IsNullOrWhiteSpace(input))
                {
                    return null;
                }

                // Try parsing the input to a uint
                if (uint.TryParse(input, out uint result) && result >= 0)
                {
                    return result; // Return the valid uint
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid non-negative number.");
                }
            }
        }
    }
}