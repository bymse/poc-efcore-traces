using Microsoft.Playwright;

namespace LoadGenerator;

public class ProductsScenario(IPage page, string baseUrl) : ScenarioBase(page, baseUrl)
{
    private const int PRODUCTS_TO_VISIT = 10;

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        await Page.GotoAsync($"{BaseUrl}/Products", new PageGotoOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle
        });

        var productIds = await ExtractIdsFromTable("[data-test-id='product-row']");
        if (productIds.Length == 0)
        {
            return;
        }

        var randomProductIds = GetRandomItems(productIds, PRODUCTS_TO_VISIT);

        foreach (var id in randomProductIds)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            await Page.GotoAsync($"{BaseUrl}/ProductDetails/{id}", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            await Task.Delay(Random.Next(500, 1500), cancellationToken);
        }
    }
}