using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CateringMvc.Models;

public class PriceSetting
{
    public int Id { get; set; }

    // ===== Fixed prices by size (8 / 10 / 12 / 14 kg rice) =====
    // Biryani
    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal BiryaniPrice8Kg { get; set; } = 4640;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal BiryaniPrice10Kg { get; set; } = 5800;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal BiryaniPrice12Kg { get; set; } = 6960;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal BiryaniPrice14Kg { get; set; } = 8120;

    // Zarda
    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ZardaPrice8Kg { get; set; } = 4400;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ZardaPrice10Kg { get; set; } = 5500;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ZardaPrice12Kg { get; set; } = 6600;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ZardaPrice14Kg { get; set; } = 7700;

    // ChanaDaig
    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ChanaDaigPrice8Kg { get; set; } = 6240;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ChanaDaigPrice10Kg { get; set; } = 7800;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ChanaDaigPrice12Kg { get; set; } = 9360;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ChanaDaigPrice14Kg { get; set; } = 10920;

    // WhiteRice
    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal WhiteRicePrice8Kg { get; set; } = 2800;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal WhiteRicePrice10Kg { get; set; } = 3500;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal WhiteRicePrice12Kg { get; set; } = 4200;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal WhiteRicePrice14Kg { get; set; } = 4900;

    // SimpleRice
    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal SimpleRicePrice8Kg { get; set; } = 2400;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal SimpleRicePrice10Kg { get; set; } = 3000;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal SimpleRicePrice12Kg { get; set; } = 3600;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal SimpleRicePrice14Kg { get; set; } = 4200;

    // GurChawal
    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal GurChawalPrice8Kg { get; set; } = 4400;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal GurChawalPrice10Kg { get; set; } = 5500;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal GurChawalPrice12Kg { get; set; } = 6600;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal GurChawalPrice14Kg { get; set; } = 7700;

    // ===== Biryani meat (per kg meat) =====
    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal BiryaniMeatPerKg { get; set; } = 1800;

    // ===== Cooking charges (per item, can be changed in an order) =====
    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal BiryaniCookingCharge { get; set; } = 2000;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ZardaCookingCharge { get; set; } = 1000;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ChanaDaigCookingCharge { get; set; } = 1000;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal WhiteRiceCookingCharge { get; set; } = 500;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal SimpleRiceCookingCharge { get; set; } = 500;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal GurChawalCookingCharge { get; set; } = 1000;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal HaleemCookingCharge { get; set; } = 1000;

    // ===== Haleem =====
    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal HaleemPerKg { get; set; } = 1200;

    // ===== Korma / Beef Korma: meat and cooking per kg meat =====
    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal KormaMeatPerKg { get; set; } = 1800;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal KormaCookingPerKg { get; set; } = 250;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal BeefKormaMeatPerKg { get; set; } = 2200;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal BeefKormaCookingPerKg { get; set; } = 250;

    // ===== Roast: meat and cooking per kg meat =====
    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal RoastMeatPerKg { get; set; } = 1800;

    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal RoastCookingPerKg { get; set; } = 250;

    // ===== Raita (optional with Biryani, Chana Wala Chawal, Chinese Rice, Korma) =====
    [Range(0, 10000000, ErrorMessage = "Enter a valid price.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal RaitaPerKg { get; set; } = 400;

    /// <summary>Fixed price for a given size (kg of rice). Returns 0 if the type has no size price.</summary>
    public decimal TierPrice(OrderType type, decimal kg)
    {
        var k = (int)kg;
        return type switch
        {
            OrderType.Biryani => k switch { 8 => BiryaniPrice8Kg, 10 => BiryaniPrice10Kg, 12 => BiryaniPrice12Kg, 14 => BiryaniPrice14Kg, _ => 0 },
            OrderType.Zarda => k switch { 8 => ZardaPrice8Kg, 10 => ZardaPrice10Kg, 12 => ZardaPrice12Kg, 14 => ZardaPrice14Kg, _ => 0 },
            OrderType.ChanaDaig => k switch { 8 => ChanaDaigPrice8Kg, 10 => ChanaDaigPrice10Kg, 12 => ChanaDaigPrice12Kg, 14 => ChanaDaigPrice14Kg, _ => 0 },
            OrderType.WhiteRice => k switch { 8 => WhiteRicePrice8Kg, 10 => WhiteRicePrice10Kg, 12 => WhiteRicePrice12Kg, 14 => WhiteRicePrice14Kg, _ => 0 },
            OrderType.SimpleRice => k switch { 8 => SimpleRicePrice8Kg, 10 => SimpleRicePrice10Kg, 12 => SimpleRicePrice12Kg, 14 => SimpleRicePrice14Kg, _ => 0 },
            OrderType.GurChawal => k switch { 8 => GurChawalPrice8Kg, 10 => GurChawalPrice10Kg, 12 => GurChawalPrice12Kg, 14 => GurChawalPrice14Kg, _ => 0 },
            _ => 0
        };
    }

    public static readonly OrderType[] TierTypes =
    {
        OrderType.Biryani, OrderType.Zarda, OrderType.ChanaDaig, OrderType.WhiteRice, OrderType.SimpleRice, OrderType.GurChawal
    };
}
