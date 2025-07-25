using AddressBookEL.AllEnums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AddressBookWebUI.Controllers
{
    [Authorize(Roles="Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
