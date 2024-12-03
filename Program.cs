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
            Console.WriteLine("5. Add Supplier");
            Console.WriteLine("6. View All Suppliers");
            Console.WriteLine("7. Associate Supplier with Item");
            Console.WriteLine("8. View Items by Supplier");
            Console.WriteLine("9. Exit");
            Console.Write("Please select an option (1-9): ");
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

                string? choice = ConsoleHelper.Prompt("Please select an option (1-9): ");

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
                        AddSupplier();
                        break;
                    case "6":
                        ViewAllSuppliers();
                        break;
                    case "7":
                        AssociateSupplierWithItem();
                        break;
                    case "8":
                        ViewItemsBySupplier();
                        break;
                    case "9":
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

            bool isChildCategory = ConsoleHelper.Prompt("Is this a child category? (y/n): ").Trim().ToLower() == "y";
            Guid parentCategoryId = Guid.Empty;

            if (isChildCategory)
            {
                string parentCategoryName = ConsoleHelper.Prompt("Enter Parent Category Name: ").Trim();
                var parentCategory = FileHandler.GetCategoryByName(parentCategoryName);

                if (parentCategory != null)
                {
                    parentCategoryId = parentCategory.CategoryId;
                    Console.WriteLine($"Parent category '{parentCategoryName}' found with ID: {parentCategoryId}");
                }
                else
                {
                    Console.WriteLine("Parent category not found.");

                    bool createParent = ConsoleHelper.Prompt("Would you like to create a parent category? (y/n): ").Trim().ToLower() == "y";
                    if (createParent)
                    {
                        parentCategoryId = CreateNewParentCategory(parentCategoryName);
                        Console.WriteLine($"New parent category '{parentCategoryName}' created with ID: {parentCategoryId}");
                    }
                    else
                    {
                        Console.WriteLine("No parent category created. The item will be added without a parent.");
                    }
                }
            }

            Guid categoryId = GetOrCreateCategoryId(categoryName, parentCategoryId);

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
        public static Guid GetOrCreateCategoryId(string categoryName, Guid? parentCategoryId = null)
        {
            var existingCategory = FileHandler.GetCategoryByName(categoryName);

            if (existingCategory != null)
            {
                Console.WriteLine($"Category '{categoryName}' already exists with ID: {existingCategory.CategoryId}");
                return existingCategory.CategoryId;
            }
            else
            {
                Guid newCategoryId = Guid.NewGuid();
                FileHandler.InsertCategory(newCategoryId, categoryName, parentCategoryId);
                Console.WriteLine($"New category '{categoryName}' created with ID: {newCategoryId}");

                return newCategoryId;
            }
        }

        private static Guid CreateNewParentCategory(string parentCategoryName)
        {
            Category newParentCategory = new Category(parentCategoryName);
            FileHandler.InsertCategory(newParentCategory.CategoryId, newParentCategory.CategoryName, null);
            return newParentCategory.CategoryId;
        }

        private static void AddSupplier()
        {
            Console.Clear();
            Console.WriteLine("Add New Supplier");

            string supplierName = ConsoleHelper.Prompt("Enter Supplier Name: ").Trim();
            string? website = ConsoleHelper.Prompt("Enter Website (optional): ").Trim();
            string? phone = ConsoleHelper.Prompt("Enter Phone Number (optional): ").Trim();
            string? email = ConsoleHelper.Prompt("Enter Email (optional): ").Trim();

            if (string.IsNullOrWhiteSpace(supplierName))
            {
                Console.WriteLine("Supplier name is required.");
                return;
            }

            Supplier newSupplier = new Supplier(supplierName, website, phone, email);

            FileHandler.InsertSupplier(newSupplier);

            Console.WriteLine($"Supplier '{newSupplier.SupplierName}' added successfully!");
            Console.WriteLine("\nPress Enter to return to the main menu.");
            Console.ReadLine();
        }
        private static void ViewAllSuppliers()
        {
            Console.Clear();
            Console.WriteLine("All Suppliers:\n");

            // Retrieve suppliers from the database
            var suppliers = FileHandler.GetAllSuppliers();

            if (suppliers.Count == 0)
            {
                Console.WriteLine("No suppliers found.");
            }
            else
            {
                foreach (var supplier in suppliers)
                {
                    Console.WriteLine(supplier);
                }
            }

            Console.WriteLine("\nPress Enter to return to the main menu.");
            Console.ReadLine();
        }
        private static void AssociateSupplierWithItem()
        {
            Console.Clear();
            Console.WriteLine("Associate Supplier with Item\n");

            var items = FileHandler.GetAllInventoryItems();
            if (items.Count == 0)
            {
                Console.WriteLine("No items found. Please add items first.");
                return;
            }

            Console.WriteLine("Available Items:");
            foreach (var item in items)
            {
                Console.WriteLine($"- {item.ItemId}: {item.ItemName}");
            }

            Guid itemId = GetValidGuid("Enter the ID of the item to associate with a supplier: ");

            var suppliers = FileHandler.GetAllSuppliers();
            if (suppliers.Count == 0)
            {
                Console.WriteLine("No suppliers found. Please add suppliers first.");
                return;
            }

            Console.WriteLine("\nAvailable Suppliers:");
            foreach (var supplier in suppliers)
            {
                Console.WriteLine($"- {supplier.SupplierId}: {supplier.SupplierName}");
            }

            Guid supplierId = GetValidGuid("Enter the ID of the supplier to associate with the selected item: ");

            FileHandler.AssociateItemWithSupplier(itemId, supplierId);

            Console.WriteLine($"Supplier successfully associated with item.");
            Console.WriteLine("\nPress Enter to return to the main menu.");
            Console.ReadLine();
        }
        private static void ViewItemsBySupplier()
        {
            Console.Clear();
            Console.WriteLine("View Items by Supplier\n");

            // Get all suppliers
            var suppliers = FileHandler.GetAllSuppliers();
            if (suppliers.Count == 0)
            {
                Console.WriteLine("No suppliers found. Please add suppliers first.");
                Console.WriteLine("\nPress Enter to return to the main menu.");
                Console.ReadLine();
                return;
            }

            // Display suppliers
            Console.WriteLine("Available Suppliers:");
            foreach (var supplier in suppliers)
            {
                Console.WriteLine($"- {supplier.SupplierId}: {supplier.SupplierName}");
            }

            // Prompt user to select a supplier
            Guid supplierId = GetValidGuid("Enter the ID of the supplier to view associated items: ");

            // Get items associated with the supplier
            var items = FileHandler.GetItemsBySupplier(supplierId);
            if (items.Count == 0)
            {
                Console.WriteLine("\nNo items are associated with this supplier.");
            }
            else
            {
                Console.WriteLine("\nItems Associated with Supplier:");
                foreach (var item in items)
                {
                    Console.WriteLine($"- {item}");
                }
            }

            Console.WriteLine("\nPress Enter to return to the main menu.");
            Console.ReadLine();
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