using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eWallet.Backend.Controllers
{
    public class TransactionController : Controller
    {
        //
        // GET: /Transaction/
        public ActionResult Index(string Id)
        {
            eWallet.Data.DynamicObj transaction = (eWallet.Data.DynamicObj)Helper.DataHelper.Get("transactions", Query.EQ("_id", Id));
            ViewBag.Title = "Chi tiết giao dịch";
            return View("~/Views/Report/Detail.cshtml", transaction);
        }

        public ActionResult Search()
        {
            return View();
        }
        public ActionResult Today()
        {
            return View();
        }

        public JsonResult JsonResultT(string _id, string profile, string status, string channel, string service, string provider, string payment_provider, string type, string system_created_time, int? page, int? page_size)
        {
            IMongoQuery query = null;
            if (!string.IsNullOrEmpty(_id))
                query =(query == null) ? Query.EQ("_id", _id): Query.And(
                    query,
                 Query.EQ("_id", _id)
                 );

            if(!string.IsNullOrEmpty(type))
            {
                query = (query == null) ? Query.EQ("transaction_type", type) : Query.And(
                    query,
                    Query.EQ("transaction_type", type)
                    );
            }
            if (!string.IsNullOrEmpty(channel))
            {
                query = (query == null) ? Query.EQ("channel", channel) : Query.And(
                    query,
                    Query.EQ("channel", channel)
                    );
            }

            if (!string.IsNullOrEmpty(service))
            {
                query = (query == null) ? Query.EQ("service", service) : Query.And(
                    query,
                    Query.EQ("service", service)
                    );
            }

            if (!string.IsNullOrEmpty(provider))
            {
                query = (query == null) ? Query.EQ("provider", provider) : Query.And(
                    query,
                    Query.EQ("provider", provider)
                    );
            }

            if (!string.IsNullOrEmpty(payment_provider))
            {
                query = (query == null) ? Query.EQ("payment_provider", payment_provider) : Query.And(
                    query,
                    Query.EQ("payment_provider", payment_provider)
                    );
            }

            if (!string.IsNullOrEmpty(profile))
                query = (query == null) ? Query.EQ("created_by", profile) : Query.And(
                    query,
                    Query.EQ("created_by", profile)
                    );
            if (!String.IsNullOrEmpty(status))
                query = (query == null) ? Query.EQ("status", status) : Query.And(
                    query,
                    Query.EQ("status", status)
                    );
            if (!string.IsNullOrEmpty(system_created_time))
                query = (query == null) ? Query.EQ("system_created_time", system_created_time) : Query.And(
                    query,
                    Query.GTE("system_created_time", system_created_time)
                    );
           
            if (page == null) page = 1;
            if (page_size == null) page_size = 25;
            long total_page = 0;
            var _list = Helper.DataHelper.ListPagging("transactions",
            query,
            SortBy.Ascending("_id"),
            (int)page_size,
            (int)page,
            out total_page
            );

            var list_accounts = (from e in _list select e).Select(p => new
            {
                _id = p._id,
                created_by = p.created_by,
                transaction_type = p.transaction_type,
                channel = p.channel,
                service = p.service,
                provider = p.provider,
                payment_provider = p.payment_provider,
                amount = p.amount,
                system_created_time = p.system_created_time,
                status = p.status

            }).ToArray();

            return Json(new { total = total_page, list = list_accounts }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JsonListTransactions(int? profile, string today, string status, DateTime? created_date_from, DateTime? created_date_to, int? page, int? page_size)
        {
            IMongoQuery query = null;
   
            if (profile != null)
                query = (query == null) ? Query.EQ("created_by", profile) : Query.And(
                    query,
                    Query.EQ("created_by", profile)
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
            var _list = Helper.DataHelper.ListPagging("transactions",
            query,
            SortBy.Ascending("_id"),
            (int)page_size,
            (int)page,
            out total_page
            );

            var list_accounts = (from e in _list select e).Select(p => new
            {
                _id = p._id,
                created_by = p.created_by,
                transaction_type = p.transaction_type,
                channel = p.channel,
                service = p.service,
                provider = p.provider,
                payment_provider = p.payment_provider,
                amount = p.amount,
                system_created_time = p.system_created_time,
                status = p.status

            }).ToArray();

            return Json(new { total = total_page, list = list_accounts }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JsonListTodayTransactions(int? profile,string today, string status, DateTime? created_date_from, DateTime? created_date_to, int? page, int? page_size)
        {
            IMongoQuery query = Query.NE("type", "P");
            today = System.DateTime.Now.ToString("yyyyMMdd");

            if (today != null)
                query = Query.And(
                    query,
                 Query.EQ("system_created_date", today)
                 );
          
            //if (profile != null)
            //    query = (query == null) ? Query.EQ("created_by", profile) : Query.And(
            //        query,
            //        Query.EQ("created_by", profile)
            //        );
            //if (!String.IsNullOrEmpty(status))
            //    query = (query == null) ? Query.EQ("status", status) : Query.And(
            //        query,
            //        Query.EQ("status", status)
            //        );
            //if (created_date_from != null)
            //    query = Query.And(
            //        query,
            //        Query.GTE("system_created_time", ((DateTime)created_date_from).ToString("yyyyMMddHHmmss"))
            //        );
            //if (created_date_to != null)
            //    query = Query.And(
            //        query,
            //        Query.LTE("system_created_time", ((DateTime)created_date_to).ToString("yyyyMMddHHmmss"))
            //        );

            if (page == null) page = 1;
            if (page_size == null) page_size = 25;
            long total_page = 0;
            var _list = Helper.DataHelper.ListPagging("transactions",
            query,
            SortBy.Ascending("_id"),
            (int)page_size,
            (int)page,
            out total_page
            );

            var list_accounts = (from e in _list select e).Select(p => new
             {
                 _id = p._id,
                 created_by = p.created_by,
                 transaction_type = p.transaction_type,
                 channel = p.channel,
                 service = p.service,
                 provider = p.provider,
                 payment_provider = p.payment_provider,
                 amount = p.amount,
                 system_created_time = p.system_created_time,
                 status = p.status

             }).ToArray();

            return Json(new { total = total_page, list = list_accounts }, JsonRequestBehavior.AllowGet);
        }
    }
}