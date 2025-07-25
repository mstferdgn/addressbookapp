using Microsoft.AspNetCore.Mvc;

namespace AddressBookWebUI.Components
{
    public class DenemeViewComponent:ViewComponent
    {

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
