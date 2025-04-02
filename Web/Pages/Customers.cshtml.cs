using DataLayer.Entities;
using DataLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

public class CustomersModel(ICustomerRepository customerRepository) : PageModel
{
    private const int PageSize = 30;

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    public int TotalPages { get; private set; }
    
    public Customer[] Customers { get; private set; }

    public async Task OnGetAsync()
    {
        var totalCount = await customerRepository.GetCustomersCount();
        TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

        if (CurrentPage < 1) CurrentPage = 1;
        if (CurrentPage > TotalPages && TotalPages > 0) CurrentPage = TotalPages;

        var skip = (CurrentPage - 1) * PageSize;
        Customers = await customerRepository.GetCustomers(skip, PageSize);
    }
}