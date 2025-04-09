using DataLayer.Entities;
using DataLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

public class ProductCreateModel : PageModel
{
    private readonly IProductRepository _productRepository;

    public ProductCreateModel(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [BindProperty]
    public ProductInputModel ProductInput { get; set; } = new();

    public class ProductInputModel
    {
        public string Name { get; set; }
        public string Category { get; set; }
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var product = new Product
        {
            Name = ProductInput.Name,
            Category = ProductInput.Category,
            Orders = new List<Order>()
        };

        await _productRepository.CrateProduct(product);

        return RedirectToPage("./ProductDetails", new { productId = product.Id });
    }
}