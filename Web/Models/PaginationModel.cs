namespace Web.Models;

public class PaginationModel
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string PagePath { get; set; }
}