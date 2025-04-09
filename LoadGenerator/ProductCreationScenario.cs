using Microsoft.Playwright;

namespace LoadGenerator;

public class ProductCreationScenario(IPage page, string baseUrl) : ScenarioBase(page, baseUrl)
{
    private readonly string[] ProductCategories = ["Electronics", "Books", "Clothing", "Food", "Toys", new('l', 140)];

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        await Page.GotoAsync($"{BaseUrl}/Products", new PageGotoOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle
        });

        await Page.ClickAsync("[data-test-id='create-product']");
        await Page.WaitForURLAsync($"{BaseUrl}/ProductCreate");

        string productName = $"Test Product {DateTime.Now:yyyyMMddHHmmss}-{Random.Next(10000)}";
        string category = ProductCategories[Random.Next(ProductCategories.Length)];

        await Page.FillAsync("[data-test-id='product-name-input']", productName);
        await Page.FillAsync("[data-test-id='product-category-input']", category);

        await Task.Delay(Random.Next(500, 1000), cancellationToken);

        await Page.ClickAsync("[data-test-id='create-product-submit']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var currentUrl = Page.Url;
        if (!currentUrl.Contains("ProductDetails"))
        {
            Console.WriteLine($"Product creation failed. Redirecting back to Products page.");
            await Page.GotoAsync($"{BaseUrl}/Products", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });
        }
    }
}