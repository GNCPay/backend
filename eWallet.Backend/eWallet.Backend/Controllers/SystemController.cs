using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eWallet.Backend.Controllers
{
    public class SystemController : Controller
    {
        [Authorize()]
        public ActionResult Setting()
        {
            if (User.IsInRole("SysAdmin"))
                return View();
            else return View();
        }
        
        [Authorize(Roles = "SysCoreAdmin")]
        public ActionResult SettingCoreChannelAPI()
        {
            //List Channel Here
            ViewBag.list_channels = Helper.DataHelper.List("channels", null);
            return View("~/Views/Box/Setting_CoreChannelAPI.cshtml");
        }
        
        [Authorize(Roles = "SysCoreAdmin")]
        public ActionResult SettingPartnerChannelAPI()
        {
            //List Channel Here
            ViewBag.list_channels = Helper.DataHelper.List("config", null);
            return View("~/Views/Box/Setting_PartnerChannelAPI.cshtml");
        }

        [Authorize]
        public ActionResult Users()
        {
            return View();
        }

        
	}
}