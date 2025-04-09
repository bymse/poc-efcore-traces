using Microsoft.Playwright;
using System.Collections.Concurrent;

namespace LoadGenerator;

public abstract class ScenarioBase(IPage page, string baseUrl)
{
    protected readonly string BaseUrl = baseUrl;
    protected readonly Random Random = new();
    protected readonly IPage Page = page;

    public abstract Task RunAsync(CancellationToken cancellationToken);

    protected async Task<int[]> ExtractIdsFromTable(string rowSelector)
    {
        var rows = await Page.QuerySelectorAllAsync(rowSelector);
        var ids = new List<int>();

        foreach (var row in rows)
        {
            var idAttribute = await row.GetAttributeAsync("data-customer-id") ??
                             await row.GetAttributeAsync("data-product-id") ??
                             await row.GetAttributeAsync("data-order-id");

            if (!string.IsNullOrEmpty(idAttribute) && int.TryParse(idAttribute, out var id))
            {
                ids.Add(id);
            }
        }

        return [.. ids];
    }

    protected int[] GetRandomItems(int[] items, int count)
    {
        int skip = Random.Next(4);
        return [.. items.Skip(skip).Take(Math.Min(count, items.Length - skip))];
    }
}