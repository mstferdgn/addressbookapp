using Microsoft.AspNetCore.Mvc;

namespace AddressBookWebUI.ViewComponents
{
    public class TryBetul : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
