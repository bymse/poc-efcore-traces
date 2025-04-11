using Microsoft.Playwright;
using System.Collections.Concurrent;

namespace LoadGenerator;

public class CustomerScenario(IPage page, string baseUrl) : ScenarioBase(page, baseUrl)
{
    private const int CUSTOMER_DETAILS_TO_VISIT = 10;

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

        var randomCustomerIds = GetRandomItems(customerIds, CUSTOMER_DETAILS_TO_VISIT, 3);

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

            await Task.Delay(Random.Next(500, 1500), cancellationToken);
        }
    }
}