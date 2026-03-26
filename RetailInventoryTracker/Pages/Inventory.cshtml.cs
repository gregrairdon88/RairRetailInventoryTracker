using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetailInventoryTracker.Models;
using RetailInventoryTracker.Services;

namespace RetailInventoryTracker.Pages;

public class InventoryModel : PageModel
{
    private readonly InventoryService _svc;

    public InventoryModel(InventoryService svc) => _svc = svc;

    public string Q { get; set; } = "";
    public string Cat { get; set; } = "All";
    public string Stock { get; set; } = "all";
    public List<string> Categories { get; set; } = [];
    public List<Models.Product> FilteredProducts { get; set; } = [];

    public void OnGet(string? q, string? cat, string? stock)
    {
        Q = q ?? "";
        Cat = cat ?? "All";
        Stock = stock ?? "all";
        LoadData();
    }

    public IActionResult OnPostAdjust(Guid id, int amount, string? q, string? cat, string? stock)
    {
        _svc.AdjustStock(id, amount);
        return RedirectToPage(new { q, cat, stock });
    }

    public IActionResult OnGetScanRedirect(string barcode)
    {
        var product = _svc.GetByBarcode(barcode);
        if (product is not null)
            return RedirectToPage("/Product/Index", new { id = product.Id });

        // Fall back to text search so the barcode value appears in the results
        return RedirectToPage(new { q = barcode });
    }

    private void LoadData()
    {
        Categories = _svc.GetCategories();
        var results = _svc.Search(Q, Cat == "All" ? null : Cat);
        FilteredProducts = (Stock switch
        {
            "low" => results.Where(p => p.IsLowStock).ToList(),
            "out" => results.Where(p => p.IsOutOfStock).ToList(),
            "ok"  => results.Where(p => !p.IsLowStock).ToList(),
            _     => results
        }).ToList();
    }
}
