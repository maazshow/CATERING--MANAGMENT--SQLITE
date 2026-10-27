using CateringMvc.Models;

namespace CateringMvc.Services;

public static class PriceCalculator
{
    /// <summary>
    /// Amount of one item:
    /// Biryani:                    fixed price of the size (8/10/12/14 kg) + meat kg x meat price + cooking
    /// Zarda / Chana / White /
    /// Chinese / Gor Wala Chawal:  fixed price of the size + cooking
    /// Korma / Beef Korma / Roast:       meat kg x meat price + cooking (cooking = meat kg x rate, computed by controller)
    /// Haleem:                     kg x haleem price + cooking
    /// </summary>
    public static decimal LineAmount(PriceSetting p, OrderType type, decimal riceKg, decimal meatKg, decimal quantityKg, decimal cookingCharge)
    {
        decimal amount = type switch
        {
            OrderType.Korma => meatKg * p.KormaMeatPerKg + cookingCharge,
            OrderType.BeefKorma => meatKg * p.BeefKormaMeatPerKg + cookingCharge,
            OrderType.Roast => meatKg * p.RoastMeatPerKg + cookingCharge,
            OrderType.Haleem => quantityKg * p.HaleemPerKg + cookingCharge,
            OrderType.Biryani => p.TierPrice(type, riceKg) + meatKg * p.BiryaniMeatPerKg + cookingCharge,
            _ => p.TierPrice(type, riceKg) + cookingCharge
        };
        return Math.Round(amount, 2);
    }
}
