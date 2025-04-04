using DataLayer.Entities;
using DataLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

public class ProductDetailsModel : PageModel
{
    private readonly IProductRepository _productRepository;

    public ProductDetailsModel(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [BindProperty(SupportsGet = true)]
    public int ProductId { get; set; }

    public Product Product { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Product = await _productRepository.GetProductById(ProductId);

        if (Product == null)
        {
            return NotFound();
        }

        return Page();
    }
}