using CateringMvc.Data;
using CateringMvc.Models;
using CateringMvc.Services;
using CateringMvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CateringMvc.Controllers;

public class OrderController : Controller
{
    private readonly AppDbContext _db;

    public OrderController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var vm = new OrderCreateViewModel { Prices = await GetPrices() };
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrderCreateViewModel vm)
    {
        var prices = await GetPrices();
        vm.Prices = prices;
        vm.Items ??= new List<OrderItemInput>();

        // Items ke binding errors hata dein, hum neeche khud check karte hain.
        foreach (var key in ModelState.Keys.Where(k => k.StartsWith("Items")).ToList())
            ModelState.Remove(key);

        var today = DateTime.Today;
        if (vm.DeliveryDate.Date < today)
            ModelState.AddModelError(nameof(vm.DeliveryDate), "Purani date nahi chun sakte.");

        if (vm.Items.Count == 0)
            ModelState.AddModelError("", "Kam az kam ek item add karein.");

        var lines = new List<OrderLine>();
        var n = 0;
        foreach (var it in vm.Items)
        {
            n++;
            var rice = it.RiceKg ?? 0;
            var meat = it.MeatKg ?? 0;
            var qty = it.QuantityKg ?? 0;
            var cook = it.CookingCharge ?? 0;
            var other = it.OtherCharge ?? 0;
            var raitaKg = (it.ItemType is OrderType.Biryani or OrderType.ChanaDaig or OrderType.SimpleRice or OrderType.Korma) ? (it.RaitaKg ?? 0) : 0;
            var label = $"Item {n} ({it.ItemType})";

            if (rice < 0 || meat < 0 || qty < 0 || cook < 0 || other < 0 || raitaKg < 0)
            {
                ModelState.AddModelError("", $"{label}: minus value nahi ho sakti.");
                continue;
            }

            string description, detail;
            var cookText = cook > 0 ? $" (other charges Rs {cook:N0})" : "";

            switch (it.ItemType)
            {
                case OrderType.Biryani:
                    if (!OrderCreateViewModel.RiceOptions.Contains(rice))
                    { ModelState.AddModelError("", $"{label}: rice 8, 10, 12 ya 14 kg chunen."); continue; }
                    if (meat <= 0)
                    { ModelState.AddModelError("", $"{label}: meat kg likhein."); continue; }
                    qty = 0;
                    description = "Biryani Daig";
                    detail = $"{rice:0.##} kg rice + {meat:0.##} kg meat{cookText}";
                    break;

                case OrderType.Zarda:
                    if (!OrderCreateViewModel.RiceOptions.Contains(rice))
                    { ModelState.AddModelError("", $"{label}: choose 8, 10, 12 or 14 kg."); continue; }
                    meat = 0; qty = 0;
                    description = "Zarda";
                    detail = $"{rice:0.##} kg rice{cookText}";
                    break;

                case OrderType.Korma:
                case OrderType.BeefKorma:
                case OrderType.Roast:
                    if (meat <= 0)
                    { ModelState.AddModelError("", $"{label}: enter meat kg."); continue; }
                    rice = 0; qty = 0;
                    // Cooking rate per kg meat always comes from Price Settings; "other charges" are extra.
                    var cookRate = it.ItemType == OrderType.BeefKorma ? prices.BeefKormaCookingPerKg
                                 : it.ItemType == OrderType.Roast ? prices.RoastCookingPerKg
                                 : prices.KormaCookingPerKg;
                    var cookAmount = Math.Round(meat * cookRate, 2);
                    cook = cookAmount + other;
                    description = it.ItemType == OrderType.BeefKorma ? "Beef Korma" : it.ItemType == OrderType.Roast ? "Roast" : "Korma";
                    detail = $"{meat:0.##} kg meat" + (other > 0 ? $" (other charges Rs {other:N0})" : "");
                    break;

                case OrderType.Haleem:
                    if (qty <= 0)
                    { ModelState.AddModelError("", $"{label}: haleem kg likhein."); continue; }
                    rice = 0; meat = 0;
                    cook = prices.HaleemCookingCharge + other;
                    description = "Haleem";
                    detail = $"{qty:0.##} kg haleem" + (other > 0 ? $" (other charges Rs {other:N0})" : "");
                    break;

                case OrderType.ChanaDaig:
                    if (!OrderCreateViewModel.RiceOptions.Contains(rice))
                    { ModelState.AddModelError("", $"{label}: choose 8, 10, 12 or 14 kg."); continue; }
                    meat = 0; qty = 0;
                    description = "Chana Wala Chawal";
                    detail = $"{rice:0.##} kg rice{cookText}";
                    break;

                case OrderType.WhiteRice:
                case OrderType.SimpleRice:
                case OrderType.GurChawal:
                    if (!OrderCreateViewModel.RiceOptions.Contains(rice))
                    { ModelState.AddModelError("", $"{label}: choose 8, 10, 12 or 14 kg."); continue; }
                    meat = 0; qty = 0;
                    description = it.ItemType == OrderType.SimpleRice ? "Chinese Rice" : it.ItemType == OrderType.GurChawal ? "Gor Wala Chawal" : "White Rice";
                    detail = $"{rice:0.##} kg rice{cookText}";
                    break;

                default:
                    ModelState.AddModelError("", $"{label}: item ki type sahi nahi.");
                    continue;
            }

            var raitaAmount = Math.Round(raitaKg * prices.RaitaPerKg, 2);
            if (raitaKg > 0) detail += $" + {raitaKg:0.##} kg raita";

            lines.Add(new OrderLine
            {
                ItemType = it.ItemType,
                Description = description,
                Detail = detail,
                RiceKg = rice,
                MeatKg = meat,
                QuantityKg = qty,
                CookingCharge = cook,
                RaitaKg = raitaKg,
                RaitaAmount = raitaAmount,
                // Amount is always calculated on the server from saved prices.
                Amount = PriceCalculator.LineAmount(prices, it.ItemType, rice, meat, qty, cook) + raitaAmount
            });
        }

        var total = lines.Sum(l => l.Amount);
        var advance = vm.AdvancePaid ?? 0;
        if (advance < 0)
            ModelState.AddModelError(nameof(vm.AdvancePaid), "Advance minus nahi ho sakta.");
        else if (lines.Count > 0 && advance > total)
            ModelState.AddModelError(nameof(vm.AdvancePaid), $"Advance total (Rs {total:N0}) se zyada nahi ho sakta.");

        if (!ModelState.IsValid)
            return View("Index", vm);

        var when = vm.DeliveryDate.Date + (vm.DeliveryTime ?? TimeSpan.Zero);

        var order = new Order
        {
            OrderType = lines[0].ItemType,
            CustomerName = vm.CustomerName.Trim(),
            Phone1 = vm.Phone1.Trim(),
            Phone2 = string.IsNullOrWhiteSpace(vm.Phone2) ? null : vm.Phone2.Trim(),
            Address = vm.Address.Trim(),
            RiceKg = lines.Sum(l => l.RiceKg),
            MeatKg = lines.Sum(l => l.MeatKg),
            TotalPrice = total,
            AdvancePaid = advance,
            DeliveryDate = when,
            // Har order pehle Pending list mein jata hai; "Done" dabane par Complete Orders mein jata hai.
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.Now,
            Lines = lines
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        TempData["OrderNo"] = order.Id.ToString();
        TempData["OrderName"] = order.CustomerName;
        TempData["OrderItems"] = string.Join("\n", lines.Select(l => $"{l.Description}: {l.Detail} = Rs {l.Amount:N0}"));
        TempData["OrderTotal"] = order.TotalPrice.ToString("N0");
        TempData["OrderPaid"] = order.AdvancePaid.ToString("N0");
        TempData["OrderRemaining"] = order.Remaining.ToString("N0");
        TempData["OrderPayClass"] = order.PayClass;
        TempData["OrderWhen"] = when.ToString("dd MMM yyyy, hh:mm tt");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Receipt(int id)
    {
        var order = await _db.Orders.AsNoTracking().Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();
        return View(order);
    }

    private async Task<PriceSetting> GetPrices() =>
        await _db.PriceSettings.AsNoTracking().FirstOrDefaultAsync() ?? new PriceSetting();
}
