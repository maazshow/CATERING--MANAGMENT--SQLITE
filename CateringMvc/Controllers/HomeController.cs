using Microsoft.AspNetCore.Mvc;

namespace CateringMvc.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();

    public IActionResult Error() => View();
}
