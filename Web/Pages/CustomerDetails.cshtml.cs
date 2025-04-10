using DataLayer.Entities;
using DataLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Models;

namespace Web.Pages;

public class CustomerDetailsModel(ICustomerRepository customerRepository, IOrdersRepository ordersRepository, IOrderStatisticsRepository orderStatisticsRepository) : PageModel
{
    private const int PageSize = 10;

    [BindProperty(SupportsGet = true)]
    public int CustomerId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    public Customer? Customer { get; private set; }
    public Order[] Orders { get; private set; }
    public PaginationModel PaginationModel { get; private set; }
    public CategoryPopularity? MostPopularCategory { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Customer = await customerRepository.GetCustomerById(CustomerId);
        if (Customer == null)
        {
            return NotFound();
        }

        var totalCount = await ordersRepository.GetCustomerOrdersCount(CustomerId);
        var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

        if (CurrentPage < 1) CurrentPage = 1;
        if (CurrentPage > totalPages && totalPages > 0) CurrentPage = totalPages;

        var skip = (CurrentPage - 1) * PageSize;
        Orders = await ordersRepository.GetCustomerOrders(CustomerId, skip, PageSize);

        PaginationModel = new PaginationModel
        {
            CurrentPage = CurrentPage,
            TotalPages = totalPages,
            PagePath = "./CustomerDetails"
        };

        MostPopularCategory = await orderStatisticsRepository.GetMostPopularCategoryForCustomer(CustomerId);

        return Page();
    }
}