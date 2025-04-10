using LoadGenerator;
using Microsoft.Playwright;
using System.Collections.Concurrent;

const int parallelSessions = 10;
const string baseUrl = "http://localhost:5171";
const int durationMinutes = 60;

var cts = new CancellationTokenSource(TimeSpan.FromMinutes(durationMinutes));
var tasks = new List<Task>();

Console.WriteLine($"Starting {parallelSessions} parallel browser sessions for {durationMinutes} minutes...");

for (var i = 0; i < parallelSessions; i++)
{
    var sessionId = i;
    tasks.Add(Task.Run(() => RunBrowserSession(sessionId, cts.Token)));
}

try
{
    await Task.WhenAll(tasks);
}
catch (TaskCanceledException)
{
    Console.WriteLine("Load test duration completed.");
}
catch (Exception ex)
{
    Console.WriteLine($"Error in load test: {ex.Message}");
}

Console.WriteLine("Load test finished.");
return;

static async Task RunBrowserSession(int sessionId, CancellationToken cancellationToken)
{
    using var playwright = await Playwright.CreateAsync();
    await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
    {
        Headless = true
    });

    while (!cancellationToken.IsCancellationRequested)
    {
        await using var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        try
        {
            await LoadTestSession(page, sessionId, baseUrl, cancellationToken);
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine($"Session {sessionId} error: {ex.Message}");
            await Task.Delay(1000, cancellationToken);
        }
    }
}

static async Task LoadTestSession(IPage page, int sessionId, string baseUrl, CancellationToken cancellationToken)
{
    var random = new Random();
    var scenarioType = random.Next(5);

    ScenarioBase scenario = scenarioType switch
    {
        0 => new CustomerScenario(page, baseUrl),
        1 => new OrderScenario(page, baseUrl),
        2 => new ProductCreationScenario(page, baseUrl),
        3 => new ProductsScenario(page, baseUrl),
        4 => new OrderStatisticsScenario(page, baseUrl),
        _ => new CustomerScenario(page, baseUrl)
    };

    Console.WriteLine($"Session {sessionId} running {scenario.GetType().Name}");
    await scenario.RunAsync(cancellationToken);
}