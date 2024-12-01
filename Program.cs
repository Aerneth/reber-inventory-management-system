using InventoryManagement.Core;
using InventoryManagement.Storage;

namespace InventoryManagment.UI
{
    using System;
    using System.Collections.Generic;

    internal static class Program
    {
        static List<InventoryManagement.Core.InventoryItem> inventory = new List<InventoryManagement.Core.InventoryItem>();
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