using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eWallet.Backend.Controllers
{
    public class FinanceAccountController : Controller
    {
        //
        // GET: /FinanceAccount/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewAccount(string Id)
        {
            eWallet.Data.DynamicObj transaction = (eWallet.Data.DynamicObj)Helper.DataHelper.Get("finance_account", Query.EQ("_id", long.Parse(Id)));
            ViewBag.Title = "Chi tiết tài khoản";
            return View("~/Views/Report/Detail.cshtml", transaction);
        }

        public ActionResult ViewTransaction(string Id)
        {
            eWallet.Data.DynamicObj transaction = (eWallet.Data.DynamicObj)Helper.DataHelper.Get("finance_transaction", Query.EQ("_id", Id));
            ViewBag.Title = "Chi tiết giao dịch";
            return View("~/Views/Report/Detail.cshtml", transaction);
        }
        [Authorize]
        public ActionResult Setting()
        {
            return View();
        }

        [Authorize]
        public ActionResult SettingChartOfAccount()
        {
            
            return View("~/Views/Box/FinanceAccount_ChartOfAccount.cshtml");
        }

        [Authorize]
        public ActionResult SettingTransactionConfig()
        {
            ViewBag.transaction_config = Helper.DataHelper.List("finance_transaction_cfg", null);
            return View("~/Views/Box/FinanceAccount_TransactionConfig.cshtml");
        }

        [Authorize]
        public ActionResult ListAccounts()
        {
            return View("~/Views/Box/FinanceAccount_ListAccounts.cshtml");
        }

        public JsonResult JsonResultFA(long? _id, string name, int? profile, string system_created_time, string status, int? page, int? page_size)
        {
            IMongoQuery query = Query.NE("type", "P");
            if (_id!=null)
                query = Query.And(
                    query,
                 Query.EQ("_id", _id)
                 );
            if (!String.IsNullOrEmpty(name))
                query = Query.And(
                    query,
                    Query.EQ("name", name)
                    );
            if (profile!=null)
                query = Query.And(
                    query,
                    Query.EQ("profile", profile)
                    );
            if (!string.IsNullOrEmpty(system_created_time))
                query = Query.And(
                    query,
                    Query.EQ("system_created_time", system_created_time)
                    );
            if (!String.IsNullOrEmpty(status))
                query = Query.And(
                    query,
                    Query.EQ("status", status)
                    );
            if (page == null) page = 1;
            if (page_size == null) page_size = 25;
            long total_page = 0;
            var _list = Helper.DataHelper.ListPagging("finance_account",
                query,
                SortBy.Ascending("_id"),
                (int)page_size,
                (int)page,
                out total_page
                );
            var list_accounts = (from e in _list select e).Select(p => new
            {
                _id = p._id,
                name = p.name,
                profile = p.profile,
                system_created_time = p.system_created_time,
                status = p.status
            }).ToArray();
            return Json(new { total = total_page, list = list_accounts }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JsonListAccounts(int? profile, string status, DateTime? created_date_from, DateTime? created_date_to, int? page, int? page_size)
        {
            IMongoQuery query = Query.NE("type", "P");
            if (profile != null)
                query = Query.And(
                    query,
                 Query.EQ("profile", profile)
                 );
            if (!String.IsNullOrEmpty(status))
                query = Query.And(
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
            var _list = Helper.DataHelper.ListPagging("finance_account",
                query,
                SortBy.Ascending("_id"),
                (int)page_size,
                (int)page,
                out total_page
                );
            var list_accounts = (from e in _list select e).Select(p => new
            {
                _id = p._id,
                name = p.name,
                profile = p.profile,
                balance = p.balance,
                available_balance = p.available_balance,
                system_created_time = p.system_created_time,
                status = p.status
            }).ToArray();
            return Json(new { total = total_page, list = list_accounts }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JsonListChartOfAccounts(int? page, int? page_size)
        {
            if (page == null) page = 1;
            if (page_size == null) page_size = 25;
            long total_page = 0;
            var _list = Helper.DataHelper.ListPagging("finance_chart_of_account",
                null,
                SortBy.Ascending("_id"),
                (int)page_size,
                (int)page,
                out total_page
                );
            var list_accounts = (from e in _list select e).Select(p => new
            {
                _id = p._id,
                name = p.name,
                type = p.type,
                parent = p.parent,
                child_id = p.child_id
            }).ToArray();
            return Json(new { total = total_page, list = list_accounts }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult ListTransactions()
        {
            //ViewBag.list_transactions = Helper.DataHelper.List("finance_transaction", null);
            return View("~/Views/Box/FinanceAccount_ListTransactions.cshtml");
        }

        public JsonResult JsonListTransactions(string business_transaction,string channel, string service, string provider, string type, string status, DateTime? created_date_from, DateTime? created_date_to, int? page, int? page_size)
        {
            IMongoQuery query = null;
            if (!String.IsNullOrEmpty(business_transaction))
                query = (query == null) ? Query.EQ("business_transaction", business_transaction) : Query.And(
                    query,
                    Query.EQ("business_transaction", business_transaction)
                    );
            if (!String.IsNullOrEmpty(channel))
                query = (query == null) ? Query.EQ("channel", channel) : Query.And(
                    query,
                    Query.EQ("channel", channel)
                    );
            if (!String.IsNullOrEmpty(service))
                query = (query == null) ? Query.EQ("service", service) : Query.And(
                    query,
                    Query.EQ("service", service)
                    );
            if (!String.IsNullOrEmpty(provider))
                query = (query == null) ? Query.EQ("provider", provider) : Query.And(
                    query,
                    Query.EQ("provider", provider)
                    );
            if (!String.IsNullOrEmpty(type))
                query = (query == null) ? Query.EQ("type", type) : Query.And(
                    query,
                    Query.EQ("type", type)
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
            var _list = Helper.DataHelper.ListPagging("finance_transaction",
                query,
                SortBy.Ascending("_id"),
                (int)page_size,
                (int)page,
                out total_page
                );
            var list_transactions = (from e in _list select e).Select(p => new
            {
                _id = p._id,
                business_transaction = p.business_transaction,
                channel = p.channel,
                service = p.service,
                provider = p.provider,
                type = p.transaction.ToUpper(),
                amount = p.amount,
                system_created_time = p.system_created_time,
                status = p.status
            }).ToArray();
            return Json(new { total = total_page, list = list_transactions }, JsonRequestBehavior.AllowGet);
        }
    }
}