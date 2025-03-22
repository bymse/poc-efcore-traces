using NBomber.CSharp;

var httpClient = new HttpClient();

var scenario = Scenario.Create("scenario", async context =>
    {
        var result = await httpClient.GetAsync("http://localhost:5010/orders?take=5000");
        if (!result.IsSuccessStatusCode)
        {
            return Response.Fail(result.StatusCode);
        }

        return Response.Ok();
    })
    .WithoutWarmUp()
    .WithLoadSimulations(
        Simulation.KeepConstant(10, TimeSpan.FromMinutes(10))
    );

NBomberRunner
    .RegisterScenarios(scenario)
    .Run();