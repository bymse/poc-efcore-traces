using Microsoft.Playwright;

namespace LoadGenerator;

public class OrderStatisticsScenario(IPage page, string baseUrl) : ScenarioBase(page, baseUrl)
{
    private const int LinksToFollow = 10;

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        await Page.GotoAsync($"{BaseUrl}/OrderStatistics", new PageGotoOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle
        });

        var orderRows = await Page.QuerySelectorAllAsync("[data-test-id='order-row']");

        if (orderRows.Count == 0)
        {
            return;
        }

        var randomRowIndex = Random.Next(orderRows.Count);
        var randomRow = orderRows[randomRowIndex];
        await randomRow.ScrollIntoViewIfNeededAsync();
        await Task.Delay(Random.Next(300, 800), cancellationToken);

        var customerLinks = await Page.QuerySelectorAllAsync("[data-test-id='order-row'] td:nth-child(3) a");
        var productLinks = await Page.QuerySelectorAllAsync("[data-test-id='order-row'] td:nth-child(4) a");

        var allLinks = new List<IElementHandle>();
        allLinks.AddRange(customerLinks);
        allLinks.AddRange(productLinks);

        if (allLinks.Count == 0)
        {
            return;
        }

        var linksToVisit = Math.Min(LinksToFollow, allLinks.Count);
        var visitedIndices = new HashSet<int>();

        for (int i = 0; i < linksToVisit; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            int linkIndex;
            do
            {
                linkIndex = Random.Next(allLinks.Count);
            } while (visitedIndices.Contains(linkIndex) && visitedIndices.Count < allLinks.Count);

            if (visitedIndices.Count >= allLinks.Count)
            {
                break;
            }

            visitedIndices.Add(linkIndex);

            await allLinks[linkIndex].ScrollIntoViewIfNeededAsync();
            await allLinks[linkIndex].ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            await Task.Delay(Random.Next(500, 1500), cancellationToken);

            await Page.GoBackAsync(new PageGoBackOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });
        }
    }
}
