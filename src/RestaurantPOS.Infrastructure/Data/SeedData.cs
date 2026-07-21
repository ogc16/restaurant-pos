using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Enums;

namespace RestaurantPOS.Infrastructure.Data;

public static class SeedData
{
    public static void Seed(ModelBuilder builder)
    {
        builder.Entity<Table>().HasData(
            new Table { Id = 1, TableNumber = "T1", Capacity = 2, Seats = 2, Section = "Indoor", Status = TableStatus.Available },
            new Table { Id = 2, TableNumber = "T2", Capacity = 4, Seats = 4, Section = "Indoor", Status = TableStatus.Available },
            new Table { Id = 3, TableNumber = "T3", Capacity = 6, Seats = 6, Section = "Indoor", Status = TableStatus.Available },
            new Table { Id = 4, TableNumber = "T4", Capacity = 4, Seats = 4, Section = "Outdoor", Status = TableStatus.Available },
            new Table { Id = 5, TableNumber = "T5", Capacity = 8, Seats = 8, Section = "Outdoor", Status = TableStatus.Available },
            new Table { Id = 6, TableNumber = "T6", Capacity = 2, Seats = 2, Section = "Bar", Status = TableStatus.Available },
            new Table { Id = 7, TableNumber = "T7", Capacity = 4, Seats = 4, Section = "VIP", Status = TableStatus.Available },
            new Table { Id = 8, TableNumber = "T8", Capacity = 6, Seats = 6, Section = "VIP", Status = TableStatus.Available }
        );

        var starters = new List<MenuItem>
        {
            new() { Id = 1, Name = "Bruschetta", Description = "Toasted bread with tomatoes, garlic, and basil", Price = 8.99m, Category = MenuItemCategory.Starter, PrepTimeMinutes = 10, IsAvailable = true },
            new() { Id = 2, Name = "Caesar Salad", Description = "Romaine lettuce with Caesar dressing and parmesan", Price = 9.49m, Category = MenuItemCategory.Starter, PrepTimeMinutes = 8, IsAvailable = true },
            new() { Id = 3, Name = "Spring Rolls", Description = "Crispy vegetable spring rolls with sweet chili sauce", Price = 7.99m, Category = MenuItemCategory.Starter, PrepTimeMinutes = 12, IsAvailable = true },
            new() { Id = 4, Name = "Soup of the Day", Description = "Chef's seasonal soup selection", Price = 6.99m, Category = MenuItemCategory.Starter, PrepTimeMinutes = 5, IsAvailable = true }
        };

        var mains = new List<MenuItem>
        {
            new() { Id = 5, Name = "Grilled Salmon", Description = "Atlantic salmon with lemon butter sauce", Price = 22.99m, Category = MenuItemCategory.Main, PrepTimeMinutes = 20, IsAvailable = true },
            new() { Id = 6, Name = "Ribeye Steak", Description = "12oz ribeye with garlic mashed potatoes", Price = 28.99m, Category = MenuItemCategory.Main, PrepTimeMinutes = 25, IsAvailable = true },
            new() { Id = 7, Name = "Chicken Parmesan", Description = "Breaded chicken with marinara and mozzarella", Price = 17.99m, Category = MenuItemCategory.Main, PrepTimeMinutes = 18, IsAvailable = true },
            new() { Id = 8, Name = "Mushroom Risotto", Description = "Creamy arborio rice with wild mushrooms", Price = 15.99m, Category = MenuItemCategory.Main, PrepTimeMinutes = 22, IsAvailable = true },
            new() { Id = 9, Name = "Fish & Chips", Description = "Beer-battered cod with tartar sauce and fries", Price = 16.99m, Category = MenuItemCategory.Main, PrepTimeMinutes = 15, IsAvailable = true }
        };

        var desserts = new List<MenuItem>
        {
            new() { Id = 10, Name = "Tiramisu", Description = "Classic Italian coffee-flavored dessert", Price = 9.99m, Category = MenuItemCategory.Dessert, PrepTimeMinutes = 5, IsAvailable = true },
            new() { Id = 11, Name = "Chocolate Lava Cake", Description = "Warm chocolate cake with molten center", Price = 11.49m, Category = MenuItemCategory.Dessert, PrepTimeMinutes = 15, IsAvailable = true },
            new() { Id = 12, Name = "Cheesecake", Description = "New York style with berry compote", Price = 8.99m, Category = MenuItemCategory.Dessert, PrepTimeMinutes = 5, IsAvailable = true }
        };

        var drinks = new List<MenuItem>
        {
            new() { Id = 13, Name = "Coca-Cola", Description = "Classic soft drink", Price = 2.99m, Category = MenuItemCategory.Drink, PrepTimeMinutes = 1, IsAvailable = true },
            new() { Id = 14, Name = "Fresh Lemonade", Description = "House-made lemonade", Price = 4.49m, Category = MenuItemCategory.Drink, PrepTimeMinutes = 3, IsAvailable = true },
            new() { Id = 15, Name = "House Red Wine", Description = "Glass of house red", Price = 7.99m, Category = MenuItemCategory.Drink, PrepTimeMinutes = 1, IsAvailable = true },
            new() { Id = 16, Name = "Espresso", Description = "Double shot espresso", Price = 3.99m, Category = MenuItemCategory.Drink, PrepTimeMinutes = 3, IsAvailable = true }
        };

        builder.Entity<MenuItem>().HasData(starters.Concat(mains).Concat(desserts).Concat(drinks));

        builder.Entity<InventoryItem>().HasData(
            new InventoryItem { Id = 1, Name = "Salmon Fillet", Quantity = 50, Unit = "pieces", LowStockThreshold = 10, CostPerUnit = 8.50m, SupplierName = "SeaFresh Co" },
            new InventoryItem { Id = 2, Name = "Ribeye Steak", Quantity = 30, Unit = "pieces", LowStockThreshold = 8, CostPerUnit = 12.00m, SupplierName = "Prime Meats" },
            new InventoryItem { Id = 3, Name = "Chicken Breast", Quantity = 40, Unit = "pieces", LowStockThreshold = 10, CostPerUnit = 4.50m, SupplierName = "Prime Meats" },
            new InventoryItem { Id = 4, Name = "Pasta", Quantity = 100, Unit = "portions", LowStockThreshold = 20, CostPerUnit = 0.80m, SupplierName = "Italian Imports" },
            new InventoryItem { Id = 5, Name = "Rice", Quantity = 200, Unit = "kg", LowStockThreshold = 25, CostPerUnit = 2.00m, SupplierName = "Grain Supply" },
            new InventoryItem { Id = 6, Name = "Bread", Quantity = 60, Unit = "loaves", LowStockThreshold = 15, CostPerUnit = 1.50m, SupplierName = "Bakery Direct" },
            new InventoryItem { Id = 7, Name = "Olive Oil", Quantity = 30, Unit = "liters", LowStockThreshold = 5, CostPerUnit = 8.00m, SupplierName = "Italian Imports" },
            new InventoryItem { Id = 8, Name = "Tomatoes", Quantity = 80, Unit = "kg", LowStockThreshold = 15, CostPerUnit = 3.00m, SupplierName = "Fresh Farms" },
            new InventoryItem { Id = 9, Name = "Mozzarella", Quantity = 25, Unit = "kg", LowStockThreshold = 5, CostPerUnit = 6.00m, SupplierName = "Dairy Best" },
            new InventoryItem { Id = 10, Name = "Eggs", Quantity = 200, Unit = "pieces", LowStockThreshold = 30, CostPerUnit = 0.30m, SupplierName = "Fresh Farms" }
        );

        builder.Entity<MenuItemInventory>().HasData(
            new MenuItemInventory { MenuItemId = 1, InventoryItemId = 6, QuantityRequired = 0.5m },
            new MenuItemInventory { MenuItemId = 1, InventoryItemId = 8, QuantityRequired = 0.2m },
            new MenuItemInventory { MenuItemId = 1, InventoryItemId = 9, QuantityRequired = 0.1m },
            new MenuItemInventory { MenuItemId = 2, InventoryItemId = 8, QuantityRequired = 0.1m },
            new MenuItemInventory { MenuItemId = 2, InventoryItemId = 9, QuantityRequired = 0.05m },
            new MenuItemInventory { MenuItemId = 3, InventoryItemId = 8, QuantityRequired = 0.15m },
            new MenuItemInventory { MenuItemId = 3, InventoryItemId = 10, QuantityRequired = 0.5m },
            new MenuItemInventory { MenuItemId = 4, InventoryItemId = 8, QuantityRequired = 0.3m },
            new MenuItemInventory { MenuItemId = 5, InventoryItemId = 1, QuantityRequired = 1m },
            new MenuItemInventory { MenuItemId = 5, InventoryItemId = 7, QuantityRequired = 0.05m },
            new MenuItemInventory { MenuItemId = 6, InventoryItemId = 2, QuantityRequired = 1m },
            new MenuItemInventory { MenuItemId = 6, InventoryItemId = 7, QuantityRequired = 0.05m },
            new MenuItemInventory { MenuItemId = 7, InventoryItemId = 3, QuantityRequired = 1m },
            new MenuItemInventory { MenuItemId = 7, InventoryItemId = 9, QuantityRequired = 0.15m },
            new MenuItemInventory { MenuItemId = 7, InventoryItemId = 8, QuantityRequired = 0.1m },
            new MenuItemInventory { MenuItemId = 8, InventoryItemId = 4, QuantityRequired = 0.3m },
            new MenuItemInventory { MenuItemId = 8, InventoryItemId = 5, QuantityRequired = 0.1m },
            new MenuItemInventory { MenuItemId = 8, InventoryItemId = 7, QuantityRequired = 0.03m },
            new MenuItemInventory { MenuItemId = 9, InventoryItemId = 3, QuantityRequired = 0.5m },
            new MenuItemInventory { MenuItemId = 9, InventoryItemId = 10, QuantityRequired = 0.2m },
            new MenuItemInventory { MenuItemId = 10, InventoryItemId = 10, QuantityRequired = 0.5m },
            new MenuItemInventory { MenuItemId = 11, InventoryItemId = 10, QuantityRequired = 1m },
            new MenuItemInventory { MenuItemId = 12, InventoryItemId = 10, QuantityRequired = 0.5m }
        );
    }

    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

        string[] roles = { "Admin", "Cashier", "Waiter" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<int>(role));
        }

        var adminEmail = "admin@restaurant.com";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Admin",
                Role = "Admin",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Admin@123456");
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
