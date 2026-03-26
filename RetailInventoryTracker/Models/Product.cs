namespace RetailInventoryTracker.Models;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Sku { get; set; } = "";
    public string Category { get; set; } = "";
    public decimal CostPrice { get; set; }
    public decimal SellPrice { get; set; }
    public int QuantityOnHand { get; set; }
    public int ReorderLevel { get; set; } = 5;
    public string Barcode { get; set; } = "";
    public string Supplier { get; set; } = "";
    public string Notes { get; set; } = "";
    public DateTime DateAdded { get; set; } = DateTime.Now;
    public DateTime LastUpdated { get; set; } = DateTime.Now;

    public bool IsLowStock => QuantityOnHand <= ReorderLevel;
    public bool IsOutOfStock => QuantityOnHand == 0;
    public decimal InventoryValue => QuantityOnHand * CostPrice;
    public decimal Margin => SellPrice > 0 && CostPrice > 0
        ? Math.Round((SellPrice - CostPrice) / SellPrice * 100, 1)
        : 0;
}
