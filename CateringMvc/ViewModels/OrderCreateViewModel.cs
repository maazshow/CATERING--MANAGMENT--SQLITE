using System.ComponentModel.DataAnnotations;
using CateringMvc.Models;

namespace CateringMvc.ViewModels;

/// <summary>Order form ka ek item (form se aata hai).</summary>
public class OrderItemInput
{
    public OrderType ItemType { get; set; } = OrderType.Biryani;
    public decimal? RiceKg { get; set; }
    public decimal? MeatKg { get; set; }
    public decimal? QuantityKg { get; set; }
    /// <summary>Tier items (Biryani, Zarda, rice items): "Other charges". Korma/Roast/Haleem: unused.</summary>
    public decimal? CookingCharge { get; set; }

    /// <summary>Extra "Other charges" for Korma, Beef Korma, Roast and Haleem.</summary>
    public decimal? OtherCharge { get; set; }

    /// <summary>Optional raita in kg (Biryani, Chana Wala Chawal, Chinese Rice, Korma).</summary>
    public decimal? RaitaKg { get; set; }
}

public class OrderCreateViewModel
{
    public static readonly decimal[] RiceOptions = { 8, 10, 12, 14 };

    public List<OrderItemInput> Items { get; set; } = new();

    [Display(Name = "Order date")]
    public DateTime DeliveryDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Order ka time chunen.")]
    [Display(Name = "Order time")]
    public TimeSpan? DeliveryTime { get; set; } = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);

    /// <summary>Customer ne advance mein kitni payment di.</summary>
    [Display(Name = "Advance payment")]
    public decimal? AdvancePaid { get; set; }

    [Required(ErrorMessage = "Customer ka naam likhein.")]
    [StringLength(100)]
    [Display(Name = "Customer name")]
    public string CustomerName { get; set; } = "";

    [Required(ErrorMessage = "Phone number 1 likhein.")]
    [RegularExpression(@"^[0-9+\-\s]{10,15}$", ErrorMessage = "Sahi phone number likhein.")]
    [Display(Name = "Phone number 1")]
    public string Phone1 { get; set; } = "";

    [RegularExpression(@"^[0-9+\-\s]{10,15}$", ErrorMessage = "Sahi phone number likhein.")]
    [Display(Name = "Phone number 2")]
    public string? Phone2 { get; set; }

    [Required(ErrorMessage = "Address likhein.")]
    [StringLength(300)]
    public string Address { get; set; } = "";

    // Sirf screen par total dikhane ke liye (form se bind nahi hota).
    public PriceSetting Prices { get; set; } = new();
}
