using DataLayer.Entities;
using DataLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

public class CustomerDetailsModel(ICustomerRepository customerRepository, IOrdersRepository ordersRepository) : PageModel
{

    [BindProperty(SupportsGet = true)]
    public int CustomerId { get; set; }

    public Customer? Customer { get; private set; }
    public Order[] Orders { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Customer = await customerRepository.GetCustomerById(CustomerId);
        if (Customer == null)
        {
            return NotFound();
        }

        Orders = await ordersRepository.GetCustomerOrders(CustomerId);
        return Page();
    }
}