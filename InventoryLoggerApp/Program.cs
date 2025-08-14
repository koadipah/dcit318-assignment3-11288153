// File: Program.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InventoryRecordsApp
{
    // ---------- Marker Interface ----------
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    // ---------- Immutable Inventory Record ----------
    // Use a positional record to keep InventoryItem immutable.
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    // ---------- Generic Inventory Logger ----------
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private readonly List<T> _log = new();
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            // Ensure DateTime is serialized in ISO format (default) — explicit converters not required here.
        };

        public InventoryLogger(string filePath)
        {
            _filePath = string.IsNullOrWhiteSpace(filePath) ? "inventory.json" : filePath;
        }

        public void Add(T item)
        {
            _log.Add(item);
        }

        public List<T> GetAll()
        {
            // return a copy to preserve internal list immutability from caller modifications
            return new List<T>(_log);
        }

        public void SaveToFile()
        {
            try
            {
                // Using StreamWriter inside a using block for safe disposal.
                var json = JsonSerializer.Serialize(_log, _jsonOptions);
                using var writer = new StreamWriter(_filePath, false);
                writer.Write(json);
                Console.WriteLine($"[Info] Saved {_log.Count} item(s) to '{Path.GetFullPath(_filePath)}'.");
            }
            catch (UnauthorizedAccessException uaEx)
            {
                Console.WriteLine($"[Error] Access denied when writing file '{_filePath}': {uaEx.Message}");
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"[Error] IO error when writing file '{_filePath}': {ioEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Unexpected error while saving to file: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine($"[Warning] File '{_filePath}' not found — starting with an empty log.");
                    _log.Clear();
                    return;
                }

                using var reader = new StreamReader(_filePath);
                var json = reader.ReadToEnd();

                if (string.IsNullOrWhiteSpace(json))
                {
                    Console.WriteLine($"[Info] File '{_filePath}' is empty — loaded 0 items.");
                    _log.Clear();
                    return;
                }

                var items = JsonSerializer.Deserialize<List<T>>(json, _jsonOptions);
                if (items is null)
                {
                    Console.WriteLine($"[Warning] No items could be deserialized from '{_filePath}'.");
                    _log.Clear();
                    return;
                }

                _log.Clear();
                _log.AddRange(items);
                Console.WriteLine($"[Info] Loaded {_log.Count} item(s) from '{Path.GetFullPath(_filePath)}'.");
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"[Error] Failed to parse JSON in '{_filePath}': {jsonEx.Message}");
            }
            catch (UnauthorizedAccessException uaEx)
            {
                Console.WriteLine($"[Error] Access denied when reading file '{_filePath}': {uaEx.Message}");
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"[Error] IO error when reading file '{_filePath}': {ioEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Unexpected error while loading from file: {ex.Message}");
            }
        }
    }

    // ---------- Integration Layer ----------
    public class InventoryApp
    {
        private readonly InventoryLogger<InventoryItem> _logger;

        public InventoryApp(string filePath = "inventory.json")
        {
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }

        // Add sample items (DateAdded uses UTC Now for consistency)
        public void SeedSampleData()
        {
            // Using different Ids and DateAdded to demonstrate immutability of records.
            _logger.Add(new InventoryItem(1, "USB-C Cable", 120, DateTime.UtcNow));
            _logger.Add(new InventoryItem(2, "Wireless Mouse", 45, DateTime.UtcNow.AddMinutes(-30)));
            _logger.Add(new InventoryItem(3, "LED Monitor 24\"", 12, DateTime.UtcNow.AddHours(-5)));
            _logger.Add(new InventoryItem(4, "Keyboard Mechanical", 20, DateTime.UtcNow.AddDays(-1)));
        }

        public void SaveData()
        {
            _logger.SaveToFile();
        }

        public void LoadData()
        {
            _logger.LoadFromFile();
        }

        public void PrintAllItems()
        {
            var items = _logger.GetAll();
            if (items.Count == 0)
            {
                Console.WriteLine("[Info] No inventory items to display.");
                return;
            }

            Console.WriteLine("Inventory items:");
            foreach (var item in items)
            {
                // Format DateAdded for readability.
                Console.WriteLine($"- ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, DateAdded: {item.DateAdded:u}");
            }
        }

        // Expose GetAll for additional usage/testing
        public List<InventoryItem> GetAllItems() => _logger.GetAll();
    }

    // ---------- Program Entry ----------
    class Program
    {
        static void Main(string[] args)
        {
            // Use provided path or default file name
            string filePath = args.Length >= 1 ? args[0] : "inventory.json";

            try
            {
                // Create first "session": seed and save
                var app = new InventoryApp(filePath);
                Console.WriteLine("[Step] Seeding sample data...");
                app.SeedSampleData();
                Console.WriteLine("[Step] Saving data to file...");
                app.SaveData();

                // Simulate a fresh process (clear memory) by creating a new InventoryApp instance
                Console.WriteLine("\n[Step] Simulating new session and loading data from file...");
                var newApp = new InventoryApp(filePath);
                newApp.LoadData();

                Console.WriteLine("\n[Step] Printing loaded items:");
                newApp.PrintAllItems();
            }
            catch (Exception ex)
            {
                // Top-level safety net for any unexpected error
                Console.WriteLine($"[Fatal] Unhandled exception: {ex.Message}");
            }
        }
    }
}
