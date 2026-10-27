using CateringMvc.Data;
using CateringMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CateringMvc.Controllers;

public class PendingOrdersController : Controller
{
    private readonly AppDbContext _db;

    public PendingOrdersController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var pending = await _db.Orders.AsNoTracking().Include(o => o.Lines)
            .Where(o => o.Status == OrderStatus.Pending)
            .ToListAsync();

        // Nearest upcoming order on top; orders whose time has already passed come after, most recent first.
        var now = DateTime.Now;
        var orders = pending.Where(o => o.DeliveryDate >= now).OrderBy(o => o.DeliveryDate).ThenBy(o => o.CreatedAt)
            .Concat(pending.Where(o => o.DeliveryDate < now).OrderByDescending(o => o.DeliveryDate))
            .ToList();

        // Meat needed today (all orders whose date is today, pending + done).
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);
        var todayOrders = await _db.Orders.AsNoTracking().Include(o => o.Lines)
            .Where(o => o.DeliveryDate >= today && o.DeliveryDate < tomorrow)
            .ToListAsync();
        var meatLines = todayOrders.SelectMany(o => o.Lines.Where(l => l.MeatKg > 0).Select(l => (Order: o, Line: l))).ToList();
        ViewBag.TodayOrderCount = todayOrders.Count;
        ViewBag.TodayMeatTotal = meatLines.Sum(x => x.Line.MeatKg);
        ViewBag.TodayMeatPending = meatLines.Where(x => x.Order.Status == OrderStatus.Pending).Sum(x => x.Line.MeatKg);
        ViewBag.TodayMeatDone = meatLines.Where(x => x.Order.Status == OrderStatus.Completed).Sum(x => x.Line.MeatKg);
        ViewBag.TodayMeatBreakdown = meatLines.GroupBy(x => x.Line.Description)
            .Select(g => new KeyValuePair<string, decimal>(g.Key, g.Sum(x => x.Line.MeatKg))).OrderByDescending(x => x.Value).ToList();
        return View(orders);
    }

    /// <summary>
    /// Order tayyar ho kar de diya gaya: ab ye Complete Orders mein chala jata hai.
    /// paymentReceived = true:  baqi payment bhi mil gayi, Complete Orders mein green.
    /// paymentReceived = false: payment baqi hai, Complete Orders mein yellow/red rehta hai aur Remaining Payments mein bhi.
    /// </summary>
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id, bool paymentReceived = false)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order != null && order.Status == OrderStatus.Pending)
        {
            order.Status = OrderStatus.Completed;
            if (paymentReceived)
                order.AdvancePaid = order.TotalPrice;

            await _db.SaveChangesAsync();

            TempData["Msg"] = order.Remaining > 0
                ? $"Order #{order.Id} done ho gaya. Rs {order.Remaining:N0} baqi hain, ye Remaining Payments mein hai."
                : $"Order #{order.Id} done ho gaya (payment poori) aur Complete Orders mein chala gaya.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order != null)
        {
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
            TempData["Msg"] = $"Order #{id} cancel kar diya gaya.";
        }
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Layout ka alert is ko har minute check karta hai:
    /// jin pending orders ka time 1 ghante ke andar hai (ya guzar chuka hai) unki list.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Due()
    {
        var now = DateTime.Now;
        var limit = now.AddMinutes(60);

        var orders = await _db.Orders.AsNoTracking().Include(o => o.Lines)
            .Where(o => o.Status == OrderStatus.Pending && o.DeliveryDate <= limit)
            .OrderBy(o => o.DeliveryDate)
            .ToListAsync();

        return Json(orders.Select(o => new
        {
            id = o.Id,
            key = $"{o.Id}|{o.DeliveryDate:yyyyMMddHHmm}",
            customer = o.CustomerName,
            phone = o.Phone1,
            items = o.ItemName,
            details = o.QuantityText,
            when = o.DeliveryDate.ToString("dd MMM, hh:mm tt"),
            minutes = (int)Math.Round((o.DeliveryDate - now).TotalMinutes)
        }));
    }
}
