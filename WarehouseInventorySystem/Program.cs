using WarehouseInventorySystem.App;
using WarehouseInventorySystem.Models;
using WarehouseInventorySystem.Exceptions;

var manager = new WareHouseManager();

manager.SeedData();

Console.WriteLine("\nGrocery Items:");
manager.PrintAllItems(manager.GetGroceriesRepo());

Console.WriteLine("\nElectronic Items:");
manager.PrintAllItems(manager.GetElectronicsRepo());

// Exception handling demonstrations
try
{
    // Duplicate item
    manager.GetElectronicsRepo().AddItem(new ElectronicItem(1, "Tablet", 15, "Apple", 12));
}
catch (DuplicateItemException ex)
{
    Console.WriteLine($"[Error] {ex.Message}");
}

try
{
    // Remove non-existent item
    manager.RemoveItemById(manager.GetGroceriesRepo(), 99);
}
catch (Exception ex)
{
    Console.WriteLine($"[Error] {ex.Message}");
}

try
{
    // Invalid quantity update
    manager.GetElectronicsRepo().UpdateQuantity(2, -5);
}
catch (InvalidQuantityException ex)
{
    Console.WriteLine($"[Error] {ex.Message}");
}
