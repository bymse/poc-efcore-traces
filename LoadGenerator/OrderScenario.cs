using Microsoft.Playwright;

namespace LoadGenerator;

public class OrderScenario(IPage page, string baseUrl) : ScenarioBase(page, baseUrl)
{
    private const int CustomersToVisit = 10;

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        await Page.GotoAsync($"{BaseUrl}/Customers", new PageGotoOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle
        });

        var customerIds = await ExtractIdsFromTable("[data-test-id='customer-row']");
        if (customerIds.Length == 0)
        {
            return;
        }

        var randomCustomerIds = GetRandomItems(customerIds, CustomersToVisit);

        foreach (var id in randomCustomerIds)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            await Page.GotoAsync($"{BaseUrl}/CustomerDetails/{id}", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            var orderRows = await Page.QuerySelectorAllAsync("[data-test-id='order-row']");
            if (orderRows.Count > 0)
            {
                var randomIndex = Random.Next(0, orderRows.Count);
                var orderRow = orderRows[randomIndex];

                await orderRow.ScrollIntoViewIfNeededAsync();
                await Task.Delay(Random.Next(300, 800), cancellationToken);
            }

            await Task.Delay(Random.Next(500, 1500), cancellationToken);
        }
    }
}