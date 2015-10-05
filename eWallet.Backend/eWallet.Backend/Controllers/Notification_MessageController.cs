using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eWallet.Backend.Controllers
{
    public class Notification_MessageController : Controller
    {
        // GET: Notification_Message
        [Authorize(Roles = "SYSTEM")]
        public ActionResult Index()
        {
            return View();
        }
    }
}