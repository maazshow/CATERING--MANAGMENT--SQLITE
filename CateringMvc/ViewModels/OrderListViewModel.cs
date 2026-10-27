using CateringMvc.Models;

namespace CateringMvc.ViewModels;

public class OrderListViewModel
{
    public List<Order> Orders { get; set; } = new();
    public string? Q { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
