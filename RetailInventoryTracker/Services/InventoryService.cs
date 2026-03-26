namespace RetailInventoryTracker.Services;

using RetailInventoryTracker.Models;

/// <summary>
/// In-memory inventory service seeded with realistic demo data.
/// In production this would talk to Azure SQL / an API.
/// </summary>
public class InventoryService
{
    private readonly List<Product> _products;

    public InventoryService()
    {
        _products = GenerateSeedData();
    }

    // ── Queries ──────────────────────────────────
    public List<Product> GetAll() => _products.OrderBy(p => p.Name).ToList();

    public Product? GetById(Guid id) => _products.FirstOrDefault(p => p.Id == id);

    public Product? GetByBarcode(string barcode) =>
        string.IsNullOrWhiteSpace(barcode) ? null :
        _products.FirstOrDefault(p => p.Barcode.Equals(barcode, StringComparison.OrdinalIgnoreCase));

    public List<Product> GetLowStock() =>
        _products.Where(p => p.IsLowStock).OrderBy(p => p.QuantityOnHand).ToList();

    public List<string> GetCategories() =>
        _products.Select(p => p.Category).Distinct().OrderBy(c => c).ToList();

    public List<Product> Search(string? query, string? category)
    {
        var results = _products.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(category) && category != "All")
            results = results.Where(p => p.Category == category);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var q = query.Trim().ToLowerInvariant();
            results = results.Where(p =>
                p.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                p.Sku.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                p.Barcode.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                p.Supplier.Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        return results.OrderBy(p => p.Name).ToList();
    }

    // ── Commands ─────────────────────────────────
    public void Add(Product product)
    {
        product.Id = Guid.NewGuid();
        product.DateAdded = DateTime.Now;
        product.LastUpdated = DateTime.Now;
        _products.Add(product);
    }

    public void Update(Product product)
    {
        var existing = GetById(product.Id);
        if (existing is null) return;

        existing.Name = product.Name;
        existing.Sku = product.Sku;
        existing.Category = product.Category;
        existing.CostPrice = product.CostPrice;
        existing.SellPrice = product.SellPrice;
        existing.QuantityOnHand = product.QuantityOnHand;
        existing.ReorderLevel = product.ReorderLevel;
        existing.Barcode = product.Barcode;
        existing.Supplier = product.Supplier;
        existing.Notes = product.Notes;
        existing.LastUpdated = DateTime.Now;
    }

    public void Delete(Guid id)
    {
        var product = GetById(id);
        if (product is not null)
            _products.Remove(product);
    }

    public void AdjustStock(Guid id, int adjustment)
    {
        var product = GetById(id);
        if (product is null) return;
        product.QuantityOnHand = Math.Max(0, product.QuantityOnHand + adjustment);
        product.LastUpdated = DateTime.Now;
    }

    // ── Dashboard stats ──────────────────────────
    public int TotalProducts => _products.Count;
    public int TotalUnits => _products.Sum(p => p.QuantityOnHand);
    public decimal TotalInventoryValue => _products.Sum(p => p.InventoryValue);
    public int LowStockCount => _products.Count(p => p.IsLowStock);
    public int OutOfStockCount => _products.Count(p => p.IsOutOfStock);

    public List<(string Category, int Count, decimal Value)> GetCategoryBreakdown() =>
        _products
            .GroupBy(p => p.Category)
            .Select(g => (g.Key, g.Count(), g.Sum(p => p.InventoryValue)))
            .OrderByDescending(x => x.Item3)
            .ToList();

    // ── Seed data ────────────────────────────────
    private static List<Product> GenerateSeedData() =>
    [
        // Hardware & Tools
        new() { Name = "Claw Hammer 16oz", Sku = "HW-1001", Category = "Hardware & Tools", CostPrice = 8.50m, SellPrice = 14.99m, QuantityOnHand = 24, ReorderLevel = 10, Supplier = "Midwest Supply Co" },
        new() { Name = "Phillips Screwdriver Set", Sku = "HW-1002", Category = "Hardware & Tools", CostPrice = 6.00m, SellPrice = 11.99m, QuantityOnHand = 18, ReorderLevel = 8, Supplier = "Midwest Supply Co" },
        new() { Name = "Tape Measure 25ft", Sku = "HW-1003", Category = "Hardware & Tools", CostPrice = 4.25m, SellPrice = 8.99m, QuantityOnHand = 3, ReorderLevel = 10, Supplier = "Midwest Supply Co" },
        new() { Name = "Adjustable Wrench 10\"", Sku = "HW-1004", Category = "Hardware & Tools", CostPrice = 7.00m, SellPrice = 12.99m, QuantityOnHand = 15, ReorderLevel = 6, Supplier = "Midwest Supply Co" },
        new() { Name = "Utility Knife", Sku = "HW-1005", Category = "Hardware & Tools", CostPrice = 3.50m, SellPrice = 6.99m, QuantityOnHand = 0, ReorderLevel = 10, Supplier = "Valley Hardware Dist." },

        // Paint & Supplies
        new() { Name = "Interior Latex Paint - White (1 gal)", Sku = "PT-2001", Category = "Paint & Supplies", CostPrice = 18.00m, SellPrice = 32.99m, QuantityOnHand = 12, ReorderLevel = 5, Supplier = "Valley Hardware Dist." },
        new() { Name = "Paint Roller Kit", Sku = "PT-2002", Category = "Paint & Supplies", CostPrice = 5.50m, SellPrice = 9.99m, QuantityOnHand = 22, ReorderLevel = 8, Supplier = "Valley Hardware Dist." },
        new() { Name = "Painter's Tape Blue 1.5\"", Sku = "PT-2003", Category = "Paint & Supplies", CostPrice = 2.75m, SellPrice = 5.49m, QuantityOnHand = 40, ReorderLevel = 15, Supplier = "Valley Hardware Dist." },
        new() { Name = "Drop Cloth 9x12", Sku = "PT-2004", Category = "Paint & Supplies", CostPrice = 4.00m, SellPrice = 7.99m, QuantityOnHand = 2, ReorderLevel = 5, Supplier = "Valley Hardware Dist." },

        // Electrical
        new() { Name = "LED Bulb 60W Equiv (4-pack)", Sku = "EL-3001", Category = "Electrical", CostPrice = 4.50m, SellPrice = 8.99m, QuantityOnHand = 30, ReorderLevel = 12, Supplier = "Ohio Electrical Supply" },
        new() { Name = "Extension Cord 25ft", Sku = "EL-3002", Category = "Electrical", CostPrice = 9.00m, SellPrice = 16.99m, QuantityOnHand = 8, ReorderLevel = 5, Supplier = "Ohio Electrical Supply" },
        new() { Name = "Outlet Cover Plate (10-pack)", Sku = "EL-3003", Category = "Electrical", CostPrice = 3.00m, SellPrice = 6.49m, QuantityOnHand = 4, ReorderLevel = 8, Supplier = "Ohio Electrical Supply" },
        new() { Name = "Wire Nuts Assorted (100-pack)", Sku = "EL-3004", Category = "Electrical", CostPrice = 5.25m, SellPrice = 9.99m, QuantityOnHand = 14, ReorderLevel = 6, Supplier = "Ohio Electrical Supply" },

        // Plumbing
        new() { Name = "PVC Pipe 1\" x 10ft", Sku = "PL-4001", Category = "Plumbing", CostPrice = 3.25m, SellPrice = 6.49m, QuantityOnHand = 20, ReorderLevel = 8, Supplier = "Ross County Plumbing" },
        new() { Name = "Pipe Wrench 14\"", Sku = "PL-4002", Category = "Plumbing", CostPrice = 12.00m, SellPrice = 21.99m, QuantityOnHand = 5, ReorderLevel = 3, Supplier = "Ross County Plumbing" },
        new() { Name = "Teflon Tape (5-pack)", Sku = "PL-4003", Category = "Plumbing", CostPrice = 2.00m, SellPrice = 4.99m, QuantityOnHand = 1, ReorderLevel = 10, Supplier = "Ross County Plumbing" },
        new() { Name = "Drain Snake 25ft", Sku = "PL-4004", Category = "Plumbing", CostPrice = 8.00m, SellPrice = 14.99m, QuantityOnHand = 7, ReorderLevel = 3, Supplier = "Ross County Plumbing" },

        // Garden & Outdoor
        new() { Name = "Garden Hose 50ft", Sku = "GD-5001", Category = "Garden & Outdoor", CostPrice = 14.00m, SellPrice = 24.99m, QuantityOnHand = 6, ReorderLevel = 4, Supplier = "Scioto Valley Outdoor" },
        new() { Name = "Potting Soil 40lb", Sku = "GD-5002", Category = "Garden & Outdoor", CostPrice = 5.00m, SellPrice = 9.99m, QuantityOnHand = 35, ReorderLevel = 10, Supplier = "Scioto Valley Outdoor" },
        new() { Name = "Hand Trowel", Sku = "GD-5003", Category = "Garden & Outdoor", CostPrice = 3.00m, SellPrice = 6.99m, QuantityOnHand = 0, ReorderLevel = 8, Supplier = "Scioto Valley Outdoor" },
        new() { Name = "Pruning Shears", Sku = "GD-5004", Category = "Garden & Outdoor", CostPrice = 7.50m, SellPrice = 13.99m, QuantityOnHand = 11, ReorderLevel = 5, Supplier = "Scioto Valley Outdoor" },
        new() { Name = "Sprinkler Head Adjustable", Sku = "GD-5005", Category = "Garden & Outdoor", CostPrice = 6.00m, SellPrice = 10.99m, QuantityOnHand = 2, ReorderLevel = 5, Supplier = "Scioto Valley Outdoor" },

        // Safety & Cleaning
        new() { Name = "Work Gloves Leather (pair)", Sku = "SF-6001", Category = "Safety & Cleaning", CostPrice = 5.50m, SellPrice = 10.99m, QuantityOnHand = 20, ReorderLevel = 8, Supplier = "Midwest Supply Co" },
        new() { Name = "Safety Glasses Clear", Sku = "SF-6002", Category = "Safety & Cleaning", CostPrice = 2.50m, SellPrice = 5.99m, QuantityOnHand = 16, ReorderLevel = 10, Supplier = "Midwest Supply Co" },
        new() { Name = "Shop Rags (50-pack)", Sku = "SF-6003", Category = "Safety & Cleaning", CostPrice = 6.00m, SellPrice = 11.99m, QuantityOnHand = 9, ReorderLevel = 4, Supplier = "Midwest Supply Co" },
    ];
}
