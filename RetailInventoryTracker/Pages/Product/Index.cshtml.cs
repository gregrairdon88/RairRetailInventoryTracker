using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetailInventoryTracker.Models;
using RetailInventoryTracker.Services;

namespace RetailInventoryTracker.Pages.Product;

public class IndexModel : PageModel
{
    private readonly InventoryService _svc;

    public IndexModel(InventoryService svc) => _svc = svc;

    [BindProperty]
    public Models.Product Product { get; set; } = new();

    public List<string> ExistingCategories { get; set; } = [];

    // Route parameter — null or "new" means create mode; a valid guid string means edit mode
    [FromRoute]
    public string? Id { get; set; }

    public bool IsNew => Id == null || string.Equals(Id, "new", StringComparison.OrdinalIgnoreCase);

    public IActionResult OnGet()
    {
        ExistingCategories = _svc.GetCategories();

        if (!IsNew)
        {
            if (!Guid.TryParse(Id, out var guid))
                return RedirectToPage("/Inventory");

            var existing = _svc.GetById(guid);
            if (existing is null)
                return RedirectToPage("/Inventory");

            Product = new Models.Product
            {
                Id = existing.Id,
                Name = existing.Name,
                Sku = existing.Sku,
                Category = existing.Category,
                CostPrice = existing.CostPrice,
                SellPrice = existing.SellPrice,
                QuantityOnHand = existing.QuantityOnHand,
                ReorderLevel = existing.ReorderLevel,
                Barcode = existing.Barcode,
                Supplier = existing.Supplier,
                Notes = existing.Notes,
            };
        }

        return Page();
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(Product.Name) ||
            string.IsNullOrWhiteSpace(Product.Sku) ||
            string.IsNullOrWhiteSpace(Product.Category))
        {
            ExistingCategories = _svc.GetCategories();
            return Page();
        }

        if (IsNew)
            _svc.Add(Product);
        else
            _svc.Update(Product);

        return RedirectToPage("/Inventory");
    }

    public IActionResult OnPostDelete()
    {
        if (!IsNew && Guid.TryParse(Id, out var guid))
            _svc.Delete(guid);

        return RedirectToPage("/Inventory");
    }
}
