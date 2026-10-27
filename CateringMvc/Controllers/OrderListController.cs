using CateringMvc.Data;
using CateringMvc.Models;
using CateringMvc.Services;
using CateringMvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CateringMvc.Controllers;

public class OrderListController : Controller
{
    private readonly AppDbContext _db;

    public OrderListController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Index(string? q, DateTime? fromDate, DateTime? toDate)
    {
        var vm = new OrderListViewModel
        {
            Q = q,
            FromDate = fromDate,
            ToDate = toDate,
            Orders = await Filter(q, fromDate, toDate).ToListAsync()
        };
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order != null)
        {
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Pdf(string? q, DateTime? fromDate, DateTime? toDate)
    {
        var orders = await Filter(q, fromDate, toDate).ToListAsync();
        var bytes = PdfBuilder.Build(orders, q, fromDate, toDate);
        return File(bytes, "application/pdf", $"orders-{DateTime.Now:yyyyMMdd-HHmm}.pdf");
    }

    // Complete Orders mein sirf Completed orders aate hain. Date filter "Order date" par lagta hai.
    private IQueryable<Order> Filter(string? q, DateTime? fromDate, DateTime? toDate)
    {
        var query = _db.Orders.AsNoTracking().Include(o => o.Lines).Where(o => o.Status == OrderStatus.Completed);

        if (fromDate.HasValue)
        {
            var from = fromDate.Value.Date;
            query = query.Where(o => o.DeliveryDate >= from);
        }
        if (toDate.HasValue)
        {
            // Order date ke saath time bhi hota hai, is liye poora din shamil karte hain.
            var toExclusive = toDate.Value.Date.AddDays(1);
            query = query.Where(o => o.DeliveryDate < toExclusive);
        }
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

        return query.OrderByDescending(o => o.DeliveryDate).ThenByDescending(o => o.CreatedAt);
    }
}
