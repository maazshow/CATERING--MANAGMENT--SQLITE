using CateringMvc.Models;

namespace CateringMvc.ViewModels;

public class RemainingViewModel
{
    public List<Order> Orders { get; set; } = new();
    public string? Q { get; set; }
}
