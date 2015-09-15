using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eWallet.Backend.Controllers
{
    public class OperationController : Controller
    {
        //
        // GET: /Operation/
        
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "SysCoreAdmin")]
        public ActionResult CashOutRequest(string Id)
        {
            if (String.IsNullOrEmpty(Id))
                return View();
            eWallet.Data.DynamicObj transaction = (eWallet.Data.DynamicObj)Helper.DataHelper.Get("operation_request", Query.EQ("_id", Id));
            ViewBag.Title = "Chi tiết yêu cầu";
            return View("~/Views/Report/Detail.cshtml", transaction);
        }

        [Authorize(Roles = "SysCoreAdmin")]
        public ActionResult CashInRequest(string Id)
        {
            if (String.IsNullOrEmpty(Id))
                return View();
            eWallet.Data.DynamicObj transaction = (eWallet.Data.DynamicObj)Helper.DataHelper.Get("operation_request", Query.EQ("_id", Id));
            ViewBag.Title = "Chi tiết yêu cầu";
            return View("~/Views/Report/Detail.cshtml", transaction);
        }

        public JsonResult JsonConfirmRequest(string request_id, string type, string note)
        {
           

            //doan nay tuy tung the loai ma thuc hien
            string request_server = @"{system:'web_frontend', module:'transaction',type:'two_way',function:'operation_confirm',request:{user_id:'" + User.Identity.Name
               + "',confirm_type:'" + type + "',confirm_note:'" + note + "', request_id:'" + request_id + "'}}";
            dynamic result = JObject.Parse(Helper.RequestToServer(request_server));

            return Json(new {error_code="00",error_message="Success" },JsonRequestBehavior.AllowGet);
        }
        public JsonResult JsonListCashOutRequest(int? profile, string status, DateTime? created_date_from, DateTime? created_date_to, int? page, int? page_size)
        {
            IMongoQuery query = Query.EQ("type", "CASHOUT");
            if (profile != null)
                query = (query == null) ? Query.EQ("profile", profile) : Query.And(
                    query,
                    Query.EQ("profile", profile)
                    );
            if (!String.IsNullOrEmpty(status))
                query = (query == null) ? Query.EQ("status", status) : Query.And(
                    query,
                    Query.EQ("status", status)
                    );
            if (created_date_from != null)
                query = Query.And(
                    query,
                    Query.GTE("system_created_time", ((DateTime)created_date_from).ToString("yyyyMMddHHmmss"))
                    );
            if (created_date_to != null)
                query = Query.And(
                    query,
                    Query.LTE("system_created_time", ((DateTime)created_date_to).ToString("yyyyMMddHHmmss"))
                    );
            if (page == null) page = 1;
            if (page_size == null) page_size = 25;
            long total_page = 0;
            var _list = Helper.DataHelper.ListPagging("operation_request",
                query,
                SortBy.Descending("system_created_time"),
                (int)page_size,
                (int)page,
                out total_page
                );
            var list_accounts = (from e in _list select e).Select(p => new
            {
                _id = p._id,
                business_transaction = p.transaction_ref,
                profile = p.profile,
                amount = p.amount,
                channel = p.channel,
                receiver = new { account_number = p.account_number, account_name = p.account_name, account_bank = p.account_bank, account_branch = p.account_branch },
                system_created_time = p.system_created_time,
                status = p.status
            }).ToArray();
            return Json(new { total = total_page, list = list_accounts }, JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult JsonListCashInRequest(int? profile, string status, DateTime? created_date_from, DateTime? created_date_to, int? page, int? page_size)
        {
            IMongoQuery query = Query.EQ("type", "CASHIN");
            if (profile != null)
                query = (query == null) ? Query.EQ("profile", profile) : Query.And(
                    query,
                    Query.EQ("profile", profile)
                    );
            if (!String.IsNullOrEmpty(status))
                query = (query == null) ? Query.EQ("status", status) : Query.And(
                    query,
                    Query.EQ("status", status)
                    );
            if (created_date_from != null)
                query = Query.And(
                    query,
                    Query.GTE("system_created_time", ((DateTime)created_date_from).ToString("yyyyMMddHHmmss"))
                    );
            if (created_date_to != null)
                query = Query.And(
                    query,
                    Query.LTE("system_created_time", ((DateTime)created_date_to).ToString("yyyyMMddHHmmss"))
                    );
            if (page == null) page = 1;
            if (page_size == null) page_size = 25;
            long total_page = 0;
            var _list = Helper.DataHelper.ListPagging("operation_request",
                query,
                SortBy.Descending("system_created_time"),
                (int)page_size,
                (int)page,
                out total_page
                );
            var list_accounts = (from e in _list select e).Select(p => new
            {
                _id = p._id,
                business_transaction = p.transaction_ref,
                profile = p.profile,
                service = p.service,
                channel = p.channel,
                amount = p.amount,
                note = p.note,
                sender = new { account_number = p.account_number, transfer_date = p.transfer_date, account_bank = p.account_bank },
                system_created_time = p.system_created_time,
                status = p.status
            }).ToArray();
            return Json(new { total = total_page, list = list_accounts }, JsonRequestBehavior.AllowGet);
        }
    }
}