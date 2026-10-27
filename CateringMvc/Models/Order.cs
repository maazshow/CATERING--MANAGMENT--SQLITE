using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CateringMvc.Models;

public enum OrderType
{
    Biryani,
    Korma,
    Zarda,
    Haleem,
    ChanaDaig,
    BeefKorma,
    Roast,
    WhiteRice,
    SimpleRice,
    GurChawal
}

public enum OrderStatus
{
    Pending,
    Completed
}

public class Order
{
    public int Id { get; set; }

    /// <summary>Purane (single item) orders ke liye. Naye orders mein pehle item ki type.</summary>
    public OrderType OrderType { get; set; }

    [Required, StringLength(100)]
    public string CustomerName { get; set; } = "";

    [Required, StringLength(20)]
    public string Phone1 { get; set; } = "";

    [StringLength(20)]
    public string? Phone2 { get; set; }

    [Required, StringLength(300)]
    public string Address { get; set; } = "";

    /// <summary>Saare items ka total rice kg.</summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal RiceKg { get; set; }

    /// <summary>Saare items ka total meat kg.</summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal MeatKg { get; set; }

    /// <summary>Saare items ka total.</summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    /// <summary>Customer ne ab tak kitni payment di (advance + baad mein li hui).</summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal AdvancePaid { get; set; }

    /// <summary>Order kis din aur kis time ke liye hai (date + time).</summary>
    public DateTime DeliveryDate { get; set; } = DateTime.Today;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    /// <summary>Order kab likha gaya (date + time).</summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<OrderLine> Lines { get; set; } = new();

    // ----- Sirf screen ke liye (database mein save nahi hote) -----

    /// <summary>Kitni payment baqi hai.</summary>
    [NotMapped]
    public decimal Remaining => Math.Max(0, TotalPrice - AdvancePaid);

    /// <summary>Row ka rang: poori payment = green, kuch baqi = yellow, bilkul nahi di = red.</summary>
    [NotMapped]
    public string PayClass => Remaining <= 0 ? "pay-green" : AdvancePaid > 0 ? "pay-yellow" : "pay-red";

    /// <summary>Table row ka rang (green / yellow / red).</summary>
    [NotMapped]
    public string RowClass => PayClass;

    [NotMapped]
    public string PayLabel => Remaining <= 0 ? "Paid" : AdvancePaid > 0 ? "Partial" : "Unpaid";

    [NotMapped]
    public string ItemName => Lines.Count > 0
        ? string.Join(", ", Lines.Select(l => l.Description))
        : OrderType switch
        {
            OrderType.Korma => "Korma",
            OrderType.Zarda => "Zarda",
            OrderType.Haleem => "Haleem",
            OrderType.ChanaDaig => "Chana Wala Chawal",
            OrderType.BeefKorma => "Beef Korma",
            OrderType.Roast => "Roast",
            OrderType.WhiteRice => "White Rice",
            OrderType.SimpleRice => "Chinese Rice",
            OrderType.GurChawal => "Gor Wala Chawal",
            _ => "Biryani Daig"
        };

    [NotMapped]
    public string QuantityText => Lines.Count > 0
        ? string.Join(" | ", Lines.Select(l => l.Detail))
        : OrderType switch
        {
            OrderType.Korma or OrderType.BeefKorma or OrderType.Roast => $"{MeatKg:0.##} kg meat",
            OrderType.Zarda or OrderType.WhiteRice or OrderType.SimpleRice or OrderType.GurChawal => $"{RiceKg:0.##} kg rice",
            OrderType.Haleem or OrderType.ChanaDaig => "",
            _ => $"{RiceKg:0.##} kg rice + {MeatKg:0.##} kg meat"
        };
}
