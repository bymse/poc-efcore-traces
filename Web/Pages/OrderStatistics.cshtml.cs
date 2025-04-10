using System.Threading.Tasks;
using DataLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Models;

namespace Web.Pages;

public class OrderStatisticsModel(IOrderStatisticsRepository orderStatisticsRepository) : PageModel
{
    public OrderStatistics[] Statistics { get; private set; }
    public ProductPopularity MostPopularProduct { get; private set; }
    public CustomerOrderCount TopCustomer { get; private set; }
    public OrderAveragesByCategory CategoryStatistics { get; private set; }
    public PaginationModel PaginationModel { get; private set; }

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    private const int PageSize = 10;

    public async Task OnGetAsync()
    {
        int totalItems = await orderStatisticsRepository.GetOrderStatisticsCount();
        int totalPages = (totalItems + PageSize - 1) / PageSize;

        if (CurrentPage < 1) CurrentPage = 1;
        if (CurrentPage > totalPages && totalPages > 0) CurrentPage = totalPages;
        int skip = (CurrentPage - 1) * PageSize;

        Statistics = await orderStatisticsRepository.GetPaginatedOrderStatistics(skip, PageSize);
        MostPopularProduct = await orderStatisticsRepository.GetMostPopularProduct();
        TopCustomer = await orderStatisticsRepository.GetCustomerWithMostOrders();
        CategoryStatistics = await orderStatisticsRepository.GetOrderAveragesByProductCategory();

        PaginationModel = new PaginationModel
        {
            CurrentPage = CurrentPage,
            TotalPages = totalPages,
            PagePath = "/OrderStatistics"
        };
    }
}
