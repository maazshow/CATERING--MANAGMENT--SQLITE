using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CateringMvc.Models;

/// <summary>Order ka ek item (jaise Biryani 10 kg, ya Zarda 8 kg). Ek order mein kai items ho sakte hain.</summary>
public class OrderLine
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order? Order { get; set; }

    public OrderType ItemType { get; set; } = OrderType.Biryani;

    [StringLength(100)]
    public string Description { get; set; } = "";

    [StringLength(200)]
    public string Detail { get; set; } = "";

    [Column(TypeName = "decimal(18,2)")]
    public decimal RiceKg { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal MeatKg { get; set; }

    /// <summary>Haleem aur Chana Daig ki quantity (kg).</summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal QuantityKg { get; set; }

    /// <summary>Is item ki cooking charges (Price Settings se aati hain, order mein badli ja sakti hain).</summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal CookingCharge { get; set; }

    /// <summary>Optional raita (kg) taken with this item.</summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal RaitaKg { get; set; }

    /// <summary>Price of that raita (already included in Amount).</summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal RaitaAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    /// <summary>Sirf quantity (cooking ki price ke baghair). Receipt mein yahi dikhta hai.</summary>
    [NotMapped]
    public string QuantityOnly => ItemType switch
    {
        OrderType.Biryani => $"{RiceKg:0.##} kg rice + {MeatKg:0.##} kg meat",
        OrderType.Zarda or OrderType.WhiteRice or OrderType.SimpleRice or OrderType.GurChawal => $"{RiceKg:0.##} kg rice",
        OrderType.Korma or OrderType.BeefKorma or OrderType.Roast => $"{MeatKg:0.##} kg meat",
        OrderType.Haleem => $"{QuantityKg:0.##} kg",
        OrderType.ChanaDaig => RiceKg > 0 ? $"{RiceKg:0.##} kg rice" : $"{QuantityKg:0.##} kg",
        _ => Detail
    };
}
