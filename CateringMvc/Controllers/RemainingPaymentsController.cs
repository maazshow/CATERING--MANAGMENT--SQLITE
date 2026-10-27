using CateringMvc.Data;
using CateringMvc.Models;
using CateringMvc.Services;
using CateringMvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CateringMvc.Controllers;

/// <summary>Jin customers ki payment baqi hai (Pending ho ya Done), unka record.</summary>
public class RemainingPaymentsController : Controller
{
    private readonly AppDbContext _db;

    public RemainingPaymentsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Index(string? q)
    {
        var vm = new RemainingViewModel { Q = q, Orders = await Filter(q).ToListAsync() };
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Pdf(string? q)
    {
        var orders = await Filter(q).ToListAsync();
        var bytes = PdfBuilder.BuildRemaining(orders, q);
        return File(bytes, "application/pdf", $"remaining-payments-{DateTime.Now:yyyyMMdd-HHmm}.pdf");
    }

    /// <summary>
    /// Payment lein. full = true ya amount khali ho to poori baqi payment clear hoti hai,
    /// warna sirf di hui amount jud jati hai. Pending aur Remaining dono pages yahi use karte hain.
    /// </summary>
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Receive(int id, decimal? amount, bool full, string? returnUrl)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order != null && order.Remaining > 0)
        {
            var remaining = order.Remaining;
            var pay = full || amount is null || amount <= 0 ? remaining : Math.Min(amount.Value, remaining);

            order.AdvancePaid += pay;
            await _db.SaveChangesAsync();

            TempData["Msg"] = order.Remaining <= 0
                ? $"Order #{order.Id} ({order.CustomerName}): payment clear ho gayi."
                : $"Order #{order.Id} ({order.CustomerName}): Rs {pay:N0} mil gaye, Rs {order.Remaining:N0} baqi hain.";
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);
        return RedirectToAction(nameof(Index));
    }

    private IQueryable<Order> Filter(string? q)
    {
        var query = _db.Orders.AsNoTracking().Include(o => o.Lines)
            .Where(o => o.TotalPrice - o.AdvancePaid > 0);

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(o =>
                o.CustomerName.Contains(term) ||
                o.Phone1.Contains(term) ||
                (o.Phone2 != null && o.Phone2.Contains(term)) ||
                o.Address.Contains(term) ||
                o.Id.ToString() == term);
        }

        return query.OrderBy(o => o.DeliveryDate).ThenBy(o => o.Id);
    }
}
