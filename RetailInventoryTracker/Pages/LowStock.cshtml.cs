using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetailInventoryTracker.Models;
using RetailInventoryTracker.Services;

namespace RetailInventoryTracker.Pages;

public class LowStockModel : PageModel
{
    private readonly InventoryService _svc;

    public LowStockModel(InventoryService svc) => _svc = svc;

    public List<Models.Product> Items { get; set; } = [];

    public void OnGet()
    {
        Items = _svc.GetLowStock();
    }

    public IActionResult OnPostQuickAdd(Guid id, int amount)
    {
        _svc.AdjustStock(id, amount);
        return RedirectToPage();
    }
}
