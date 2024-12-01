using InventoryManagement.Core;
using Microsoft.Extensions.Configuration;
using System.Data.SQLite;

namespace InventoryManagement.Storage
{
    public static class FileHandler
    {
        public static string GetDatabaseConnectionString()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            var connectionString = config.GetConnectionString("DefaultDBConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string is missing in appsettings.json");
            }

            return connectionString;
        }
        public static void ConnectToDatabase()
        {
            try
            {
                var connectionString = GetDatabaseConnectionString();

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Connected to the database successfully!");
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Database connection failed: {ex.Message}");
            }
        }

        public static void InitializeDatabase()
        {
            var connectionString = GetDatabaseConnectionString();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Suppliers (
                        SupplierId	TEXT NOT NULL UNIQUE,
                        SupplierName	TEXT NOT NULL UNIQUE,
                        Website	TEXT,
                        Phone	TEXT,
                        Email	TEXT,
                        PRIMARY KEY(SupplierId)
                    );
                    CREATE TABLE IF NOT EXISTS ItemSuppliers (
                        ItemId	TEXT NOT NULL,
                        SupplierId	TEXT NOT NULL,
                        PRIMARY KEY(ItemId,SupplierId),
                        FOREIGN KEY(ItemId) REFERENCES InventoryItems(ItemId),
                        FOREIGN KEY(SupplierId) REFERENCES Suppliers(SupplierId)
                    );
                    CREATE TABLE IF NOT EXISTS InventoryItems (
                        ItemId	TEXT NOT NULL UNIQUE,
                        ItemName	TEXT NOT NULL,
                        CategoryId	TEXT NOT NULL,
                        Quantity	INTEGER DEFAULT 0,
                        Price	REAL DEFAULT 0.00,
                        MinStock	INTEGER,
                        MaxStock	INTEGER,
                        PRIMARY KEY(ItemId),
                        FOREIGN KEY(CategoryId) REFERENCES Categories(CategoryId)
                    );
                    CREATE TABLE IF NOT EXISTS Categories (
                        CategoryId	TEXT NOT NULL UNIQUE,
                        CategoryName	TEXT NOT NULL UNIQUE,
                        ParentId	TEXT,
                        FOREIGN KEY(ParentId) REFERENCES Categories(CategoryId)
                    );
                    CREATE INDEX IF NOT EXISTS idx_inventoryitems_categoryid ON InventoryItems (
                        CategoryId
                    );
                    CREATE INDEX IF NOT EXISTS idx_itemsuppliers_itemid ON ItemSuppliers (
                        ItemId
                    );
                    CREATE INDEX IF NOT EXISTS idx_itemsuppliers_supplierid ON ItemSuppliers (
                        SupplierId
                    );
                ";

                using (var cmd = new SQLiteCommand(createTableQuery, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            Console.WriteLine("Database initialized and tables created if they did not exist.");
        }
        public static void InsertInventoryItem(InventoryItem item)
        {
            var connectionString = GetDatabaseConnectionString();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string insertQuery = @"
                    INSERT INTO InventoryItems (ItemId, ItemName, CategoryId, Quantity, Price, MinStock, MaxStock)
                    VALUES (@ItemId, @ItemName, @CategoryId, @Quantity, @Price, @MinStock, @MaxStock);
                ";

                using (var cmd = new SQLiteCommand(insertQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@ItemId", item.ItemId.ToString());
                    cmd.Parameters.AddWithValue("@ItemName", item.ItemName);
                    cmd.Parameters.AddWithValue("@CategoryId", item.CategoryId.ToString());
                    cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    cmd.Parameters.AddWithValue("@Price", item.Price);
                    cmd.Parameters.AddWithValue("@MinStock", item.MinStock ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaxStock", item.MaxStock ?? (object)DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }

            Console.WriteLine($"Inventory item {item.ItemName} inserted into the database.");
        }
        public static List<InventoryItem> GetAllInventoryItems()
        {
            var inventoryItems = new List<InventoryItem>();
            var connectionString = GetDatabaseConnectionString();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string selectQuery = "SELECT * FROM InventoryItems";

                using (var cmd = new SQLiteCommand(selectQuery, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var itemName = reader["ItemName"] != DBNull.Value ? reader["ItemName"].ToString() : "Unknown";
                            var categoryId = reader["CategoryId"] != DBNull.Value ? Guid.Parse(reader["CategoryId"].ToString()) : Guid.Empty;
                            var quantity = reader["Quantity"] != DBNull.Value ? Convert.ToUInt32(reader["Quantity"]) : 0;
                            var price = reader["Price"] != DBNull.Value ? Convert.ToDecimal(reader["Price"]) : 0.00m;
                            var minStock = reader["MinStock"] != DBNull.Value ? Convert.ToUInt32(reader["MinStock"]) : 0;
                            var maxStock = reader["MaxStock"] != DBNull.Value ? Convert.ToUInt32(reader["MaxStock"]) : 0;

                            var item = new InventoryItem(itemName, categoryId, quantity, price, minStock, maxStock);

                            
                            if (reader["ItemId"] != DBNull.Value)
                            {
                                item.SetItemId(Guid.Parse(reader["ItemId"].ToString()));
                            }
                            inventoryItems.Add(item);
                        }
                    }
                }
            }

            return inventoryItems;
        }
        public static void UpdateInventoryItem(InventoryItem item)
        {
            var connectionString = GetDatabaseConnectionString();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string updateQuery = @"
                    UPDATE InventoryItems
                    SET ItemName = @ItemName,
                        CategoryId = @CategoryId,
                        Quantity = @Quantity,
                        Price = @Price,
                        MinStock = @MinStock,
                        MaxStock = @MaxStock
                    WHERE ItemId = @ItemId;
                ";

                using (var cmd = new SQLiteCommand(updateQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@ItemId", item.ItemId.ToString());
                    cmd.Parameters.AddWithValue("@ItemName", item.ItemName);
                    cmd.Parameters.AddWithValue("@CategoryId", item.CategoryId.ToString());
                    cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    cmd.Parameters.AddWithValue("@Price", item.Price);
                    cmd.Parameters.AddWithValue("@MinStock", item.MinStock ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaxStock", item.MaxStock ?? (object)DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }

            Console.WriteLine($"Inventory item {item.ItemName} updated in the database.");
        }
/*        public static void SaveInventoryToFile(List<InventoryItem> inventory, string filePath)
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
        }*/
    }
}