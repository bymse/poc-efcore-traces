using DataLayer.Entities;
using DataLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Models;

namespace Web.Pages;

public class ProductsModel(IProductRepository productRepository) : PageModel
{
    private const int PageSize = 30;

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    public Product[] Products { get; private set; }

    public PaginationModel PaginationModel { get; private set; }

    public async Task OnGetAsync()
    {
        var totalCount = await productRepository.GetProductsCount();
        var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

        if (CurrentPage < 1) CurrentPage = 1;
        if (CurrentPage > totalPages && totalPages > 0) CurrentPage = totalPages;

        var skip = (CurrentPage - 1) * PageSize;
        Products = await productRepository.GetProducts(skip, PageSize);

        PaginationModel = new PaginationModel
        {
            CurrentPage = CurrentPage,
            TotalPages = totalPages,
            PagePath = "./Products"
        };
    }
}
