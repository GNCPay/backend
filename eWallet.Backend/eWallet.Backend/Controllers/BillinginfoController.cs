using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eWallet.Backend.Controllers
{
    public class BillinginfoController : Controller
    {

        // GET: Billinginfo
        [Authorize(Roles = "SYSTEM, MERCHANT, GNC, CUSTOMER")]
        public ActionResult Billing_info()
        {
            return View("~/Views/Billinginfo/Billing_info.cshtml");
        }
        //public ActionResult ListBillinginfo()
        //{
        //    return View("~/Views/Box/Billing_info.cshtml");
        //}

        public ActionResult ViewBillinginfo(string Id)
        {
            eWallet.Data.DynamicObj transaction = (eWallet.Data.DynamicObj)Helper.DataHelper.Get("billing_info", Query.EQ("_id", long.Parse(Id)));
            ViewBag.Title = "Chi tiết Billinginfo";
            return View("~/Views/Report/Detail.cshtml", transaction);
        }

        public JsonResult SearchResultBill(int? _id, string code, string customer_id, string customer_name, string provider, string service, string status, int? page, int? page_size)
        {
            IMongoQuery query = null;
            if (_id != null)
                query = (query == null) ? Query.EQ("_id", _id) : Query.And(
                    query,
                    Query.EQ("_id", _id)
                    );
            if (!String.IsNullOrEmpty(code))
                query = (query == null) ? Query.EQ("code", code) : Query.And(
                    query,
                    Query.EQ("code", code)
                    );
            if (!String.IsNullOrEmpty(customer_id))
                query = (query == null) ? Query.EQ("customer_id", customer_id) : Query.And(
                    query,
                    Query.EQ("customer_id", customer_id)
                    );
            if (!String.IsNullOrEmpty(customer_name))
                query = (query == null) ? Query.EQ("customer_name", customer_name) : Query.And(
                    query,
                    Query.EQ("customer_name", customer_name)
                    );
            if (!String.IsNullOrEmpty(provider))
                query = (query == null) ? Query.EQ("provider.code", provider) : Query.And(
                    query,
                    Query.EQ("provider.code", provider)
                    );
            if (!String.IsNullOrEmpty(service))
                query = (query == null) ? Query.EQ("service.code", service) : Query.And(
                    query,
                    Query.EQ("service.code", service)
                    );
            if (!String.IsNullOrEmpty(status))
                query = (query == null) ? Query.EQ("status", status) : Query.And(
                    query,
                    Query.EQ("status", status)
                    );

            if (page == null) page = 1;
            if (page_size == null) page_size = 25;
            long total_page = 0;
            var _list = Helper.DataHelper.ListPagging("billing_info",
                query,
                SortBy.Descending("_id"),
                (int)page_size,
                (int)page,
                out total_page
                );
            var list_accounts = (from e in _list select e).Select(p => new
            {
                _id = p._id,
                code = p.code,
                customer_id = p.customer_id,
                customer_name = p.customer_name,
                provider = p.provider.code,
                service = p.service.code,
                status = p.status
            }).ToArray();
            return Json(new { total = total_page, list = list_accounts }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListResultBillinginfo(int? _id, string code, string customer_id, string customer_name, string provider, string service, string status, int? page, int? page_size)
        {
            IMongoQuery query = null;
            if (_id != null)
                query = (query == null) ? Query.EQ("_id", _id) : Query.And(
                    query,
                    Query.EQ("_id", _id)
                    );
            if (!String.IsNullOrEmpty(code))
                query = (query == null) ? Query.EQ("code", code) : Query.And(
                    query,
                    Query.EQ("code", code)
                    );
            if (!String.IsNullOrEmpty(customer_id))
                query = (query == null) ? Query.EQ("customer_id", customer_id) : Query.And(
                    query,
                    Query.EQ("customer_id", customer_id)
                    );
            if (!String.IsNullOrEmpty(customer_name))
                query = (query == null) ? Query.EQ("customer_name", customer_name) : Query.And(
                    query,
                    Query.EQ("customer_name", customer_name)
                    );
            if (!String.IsNullOrEmpty(provider))
                query = (query == null) ? Query.EQ("provider.name", provider) : Query.And(
                    query,
                    Query.EQ("provider.name", provider)
                    );
            if (!String.IsNullOrEmpty(service))
                query = (query == null) ? Query.EQ("service", service) : Query.And(
                    query,
                    Query.EQ("service", service)
                    );
            if (!String.IsNullOrEmpty(status))
                query = (query == null) ? Query.EQ("status", status) : Query.And(
                    query,
                    Query.EQ("status", status)
                    );
            
            if (page == null) page = 1;
            if (page_size == null) page_size = 25;
            long total_page = 0;
            var _list = Helper.DataHelper.ListPagging("billing_info",
                query,
                SortBy.Ascending("_id"),
                (int)page_size,
                (int)page,
                out total_page
                );
            var list_accounts = (from e in _list select e).Select(p => new
            {
                _id = p._id,
                code = p.code,
                customer_id = p.customer_id,
                customer_name = p.customer_name,
                provider = p.provider.code,
                service=p.service.code,
                status = p.status
            }).ToArray();
            return Json(new { total = total_page, list = list_accounts }, JsonRequestBehavior.AllowGet);
        }
    }
}