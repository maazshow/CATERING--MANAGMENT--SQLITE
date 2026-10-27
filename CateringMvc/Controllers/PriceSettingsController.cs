using CateringMvc.Data;
using CateringMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CateringMvc.Controllers;

public class PriceSettingsController : Controller
{
    private readonly AppDbContext _db;

    public PriceSettingsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var settings = await _db.PriceSettings.AsNoTracking().FirstOrDefaultAsync() ?? new PriceSetting();
        return View(settings);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(PriceSetting model)
    {
        if (!ModelState.IsValid)
            return View("Index", model);

        var s = await _db.PriceSettings.FirstOrDefaultAsync();
        if (s == null)
        {
            s = new PriceSetting();
            _db.PriceSettings.Add(s);
        }

        s.BiryaniPrice8Kg = model.BiryaniPrice8Kg;
        s.BiryaniPrice10Kg = model.BiryaniPrice10Kg;
        s.BiryaniPrice12Kg = model.BiryaniPrice12Kg;
        s.BiryaniPrice14Kg = model.BiryaniPrice14Kg;
        s.ZardaPrice8Kg = model.ZardaPrice8Kg;
        s.ZardaPrice10Kg = model.ZardaPrice10Kg;
        s.ZardaPrice12Kg = model.ZardaPrice12Kg;
        s.ZardaPrice14Kg = model.ZardaPrice14Kg;
        s.ChanaDaigPrice8Kg = model.ChanaDaigPrice8Kg;
        s.ChanaDaigPrice10Kg = model.ChanaDaigPrice10Kg;
        s.ChanaDaigPrice12Kg = model.ChanaDaigPrice12Kg;
        s.ChanaDaigPrice14Kg = model.ChanaDaigPrice14Kg;
        s.WhiteRicePrice8Kg = model.WhiteRicePrice8Kg;
        s.WhiteRicePrice10Kg = model.WhiteRicePrice10Kg;
        s.WhiteRicePrice12Kg = model.WhiteRicePrice12Kg;
        s.WhiteRicePrice14Kg = model.WhiteRicePrice14Kg;
        s.SimpleRicePrice8Kg = model.SimpleRicePrice8Kg;
        s.SimpleRicePrice10Kg = model.SimpleRicePrice10Kg;
        s.SimpleRicePrice12Kg = model.SimpleRicePrice12Kg;
        s.SimpleRicePrice14Kg = model.SimpleRicePrice14Kg;
        s.GurChawalPrice8Kg = model.GurChawalPrice8Kg;
        s.GurChawalPrice10Kg = model.GurChawalPrice10Kg;
        s.GurChawalPrice12Kg = model.GurChawalPrice12Kg;
        s.GurChawalPrice14Kg = model.GurChawalPrice14Kg;
        s.BiryaniMeatPerKg = model.BiryaniMeatPerKg;
        s.BiryaniCookingCharge = model.BiryaniCookingCharge;
        s.ZardaCookingCharge = model.ZardaCookingCharge;
        s.ChanaDaigCookingCharge = model.ChanaDaigCookingCharge;
        s.WhiteRiceCookingCharge = model.WhiteRiceCookingCharge;
        s.SimpleRiceCookingCharge = model.SimpleRiceCookingCharge;
        s.GurChawalCookingCharge = model.GurChawalCookingCharge;
        s.HaleemCookingCharge = model.HaleemCookingCharge;
        s.HaleemPerKg = model.HaleemPerKg;
        s.KormaMeatPerKg = model.KormaMeatPerKg;
        s.KormaCookingPerKg = model.KormaCookingPerKg;
        s.BeefKormaMeatPerKg = model.BeefKormaMeatPerKg;
        s.BeefKormaCookingPerKg = model.BeefKormaCookingPerKg;
        s.RoastMeatPerKg = model.RoastMeatPerKg;
        s.RaitaPerKg = model.RaitaPerKg;
        s.RoastCookingPerKg = model.RoastCookingPerKg;

        await _db.SaveChangesAsync();
        TempData["Saved"] = "Prices saved.";
        return RedirectToAction(nameof(Index));
    }
}
