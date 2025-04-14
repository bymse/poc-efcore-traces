using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

public class IndexModel() : PageModel
{
    public string GrafanaWebMonitoringUrl { get; } = "http://localhost:8080/grafana/d/eeiiievmiry80f/web-monitoring";
    public string GrafanaRedMetricsUrl { get; } = "http://localhost:8080/grafana/d/beienxwtq39c0a/r-e-d-metrics-cool";
    public string GrafanaTracesSearchUrl { get; } = "http://localhost:8080/grafana/explore";

    public void OnGet()
    {
        
    }
}