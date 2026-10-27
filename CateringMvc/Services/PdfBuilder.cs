using CateringMvc.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CateringMvc.Services;

public static class PdfBuilder
{
    private static string ItemsText(Order o) =>
        o.Lines.Count > 0
            ? string.Join("\n", o.Lines.Select(l => $"{l.Description}: {l.Detail} - Rs {l.Amount:N0}"))
            : $"{o.ItemName}: {o.QuantityText}";

    private static string PayBg(Order o) =>
        o.Remaining <= 0 ? Colors.Green.Lighten4
        : o.AdvancePaid > 0 ? Colors.Yellow.Lighten3
        : Colors.Red.Lighten4;

    // ---------------- Complete Orders PDF ----------------
    public static byte[] Build(IReadOnlyList<Order> orders, string? search, DateTime? fromDate, DateTime? toDate)
    {
        var grandTotal = orders.Sum(o => o.TotalPrice);
        var grandPaid = orders.Sum(o => o.AdvancePaid);
        var grandRemaining = orders.Sum(o => o.Remaining);
        var range = fromDate == null && toDate == null
            ? "All dates"
            : $"{(fromDate?.ToString("dd MMM yyyy") ?? "start")} to {(toDate?.ToString("dd MMM yyyy") ?? "today")}";
        var searchText = string.IsNullOrWhiteSpace(search) ? "" : $"  |  Search: {search}";

        return Document.Create(doc =>
        {
            doc.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(24);
                page.DefaultTextStyle(x => x.FontSize(8));

                page.Header().Column(col =>
                {
                    col.Item().Text($"{BusinessInfo.Name} - Complete Orders").FontSize(18).Bold();
                    col.Item().Text($"{range}{searchText}  |  Orders: {orders.Count}  |  Total: Rs {grandTotal:N0}  |  Paid: Rs {grandPaid:N0}  |  Remaining: Rs {grandRemaining:N0}  |  Generated: {DateTime.Now:dd MMM yyyy, hh:mm tt}")
                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                    col.Item().Text($"{BusinessInfo.Address}  |  Phone: {BusinessInfo.Phone}").FontSize(8).FontColor(Colors.Grey.Darken1);
                    col.Item().Text("Green = fully paid, Yellow = partially paid, Red = unpaid").FontSize(8).FontColor(Colors.Grey.Darken1);
                });

                page.Content().PaddingTop(10).Table(t =>
                {
                    t.ColumnsDefinition(c =>
                    {
                        c.ConstantColumn(22);   // #
                        c.ConstantColumn(62);   // booked
                        c.ConstantColumn(62);   // order date & time
                        c.RelativeColumn(2);    // customer
                        c.ConstantColumn(64);   // phone 1
                        c.ConstantColumn(64);   // phone 2
                        c.RelativeColumn(3);    // address
                        c.RelativeColumn(4);    // items
                        c.ConstantColumn(48);   // total
                        c.ConstantColumn(48);   // paid
                        c.ConstantColumn(48);   // remaining
                    });

                    t.Header(h =>
                    {
                        foreach (var title in new[] { "#", "Booked", "Order date & time", "Customer", "Phone 1", "Phone 2", "Address", "Items", "Total", "Paid", "Remaining" })
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text(title).Bold();
                    });

                    foreach (var o in orders)
                    {
                        IContainer Cell(IContainer c, int i) => c.Background(PayBg(o)).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3);

                        t.Cell().Element(c => Cell(c, 0)).Text(o.Id.ToString());
                        t.Cell().Element(c => Cell(c, 1)).Text($"{o.CreatedAt:dd MMM yyyy}\n{o.CreatedAt:hh:mm tt}");
                        t.Cell().Element(c => Cell(c, 2)).Text($"{o.DeliveryDate:dd MMM yyyy}\n{o.DeliveryDate:hh:mm tt}");
                        t.Cell().Element(c => Cell(c, 3)).Text(o.CustomerName);
                        t.Cell().Element(c => Cell(c, 4)).Text(o.Phone1);
                        t.Cell().Element(c => Cell(c, 5)).Text(o.Phone2 ?? "-");
                        t.Cell().Element(c => Cell(c, 6)).Text(o.Address);
                        t.Cell().Element(c => Cell(c, 7)).Text(ItemsText(o));
                        t.Cell().Element(c => Cell(c, 8)).AlignRight().Text(o.TotalPrice.ToString("N0")).Bold();
                        t.Cell().Element(c => Cell(c, 9)).AlignRight().Text(o.AdvancePaid.ToString("N0"));
                        t.Cell().Element(c => Cell(c, 10)).AlignRight().Text(o.Remaining.ToString("N0")).Bold();
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    // ---------------- Remaining Payments PDF ----------------
    public static byte[] BuildRemaining(IReadOnlyList<Order> orders, string? search)
    {
        var totalRemaining = orders.Sum(o => o.Remaining);
        var searchText = string.IsNullOrWhiteSpace(search) ? "" : $"  |  Search: {search}";

        return Document.Create(doc =>
        {
            doc.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(24);
                page.DefaultTextStyle(x => x.FontSize(8.5f));

                page.Header().Column(col =>
                {
                    col.Item().Text($"{BusinessInfo.Name} - Remaining Payments").FontSize(18).Bold();
                    col.Item().Text($"Customers: {orders.Count}{searchText}  |  Total remaining: Rs {totalRemaining:N0}  |  Generated: {DateTime.Now:dd MMM yyyy, hh:mm tt}")
                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                    col.Item().Text($"{BusinessInfo.Address}  |  Phone: {BusinessInfo.Phone}").FontSize(8).FontColor(Colors.Grey.Darken1);
                    col.Item().Text("Yellow = partially paid, Red = unpaid").FontSize(8).FontColor(Colors.Grey.Darken1);
                });

                page.Content().PaddingTop(10).Table(t =>
                {
                    t.ColumnsDefinition(c =>
                    {
                        c.ConstantColumn(24);   // #
                        c.ConstantColumn(66);   // order date & time
                        c.RelativeColumn(2);    // customer
                        c.ConstantColumn(70);   // phone 1
                        c.ConstantColumn(70);   // phone 2
                        c.RelativeColumn(3);    // address
                        c.RelativeColumn(3);    // items
                        c.ConstantColumn(46);   // order status
                        c.ConstantColumn(52);   // total
                        c.ConstantColumn(52);   // paid
                        c.ConstantColumn(56);   // remaining
                    });

                    t.Header(h =>
                    {
                        foreach (var title in new[] { "#", "Order date & time", "Customer", "Phone 1", "Phone 2", "Address", "Items", "Order", "Total", "Paid", "Remaining" })
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text(title).Bold();
                    });

                    foreach (var o in orders)
                    {
                        IContainer Cell(IContainer c, int i) => c.Background(PayBg(o)).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3);

                        t.Cell().Element(c => Cell(c, 0)).Text(o.Id.ToString());
                        t.Cell().Element(c => Cell(c, 1)).Text($"{o.DeliveryDate:dd MMM yyyy}\n{o.DeliveryDate:hh:mm tt}");
                        t.Cell().Element(c => Cell(c, 2)).Text(o.CustomerName);
                        t.Cell().Element(c => Cell(c, 3)).Text(o.Phone1);
                        t.Cell().Element(c => Cell(c, 4)).Text(o.Phone2 ?? "-");
                        t.Cell().Element(c => Cell(c, 5)).Text(o.Address);
                        t.Cell().Element(c => Cell(c, 6)).Text(o.ItemName);
                        t.Cell().Element(c => Cell(c, 7)).Text(o.Status == OrderStatus.Pending ? "Pending" : "Done");
                        t.Cell().Element(c => Cell(c, 8)).AlignRight().Text(o.TotalPrice.ToString("N0"));
                        t.Cell().Element(c => Cell(c, 9)).AlignRight().Text(o.AdvancePaid.ToString("N0"));
                        t.Cell().Element(c => Cell(c, 10)).AlignRight().Text(o.Remaining.ToString("N0")).Bold();
                    }

                    // Aakhir mein total baqi.
                    t.Cell().ColumnSpan(10).AlignRight().PaddingTop(6).Text("Total remaining:").Bold();
                    t.Cell().PaddingTop(6).AlignRight().Text($"Rs {totalRemaining:N0}").Bold();
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        }).GeneratePdf();
    }
}
