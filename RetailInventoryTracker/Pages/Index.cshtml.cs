using Microsoft.AspNetCore.Mvc.RazorPages;
using RetailInventoryTracker.Models;
using RetailInventoryTracker.Services;

namespace RetailInventoryTracker.Pages;

public class IndexModel : PageModel
{
    private readonly InventoryService _svc;

    public IndexModel(InventoryService svc) => _svc = svc;

    public int TotalProducts { get; set; }
    public int TotalUnits { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public int LowStockCount { get; set; }
    public List<Models.Product> LowStockItems { get; set; } = [];
    public List<Models.Product> RecentProducts { get; set; } = [];
    public List<(string Category, int Count, decimal Value)> CategoryBreakdown { get; set; } = [];

    public void OnGet()
    {
        TotalProducts = _svc.TotalProducts;
        TotalUnits = _svc.TotalUnits;
        TotalInventoryValue = _svc.TotalInventoryValue;
        LowStockCount = _svc.LowStockCount;
        LowStockItems = _svc.GetLowStock();
        RecentProducts = _svc.GetAll()
            .OrderByDescending(p => p.LastUpdated)
            .Take(8)
            .ToList();
        CategoryBreakdown = _svc.GetCategoryBreakdown();
    }
}
