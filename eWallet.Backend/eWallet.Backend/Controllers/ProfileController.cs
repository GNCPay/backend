using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eWallet.Backend.Controllers
{
    public class ProfileController : Controller
    {
        //
        // GET: /Profile/
        [Authorize(Roles = "SysCoreAdmin")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewProfile(string Id)
        {
            eWallet.Data.DynamicObj transaction = (eWallet.Data.DynamicObj)Helper.DataHelper.Get("profile", Query.EQ("_id",long.Parse(Id)));
            ViewBag.Title = "Chi tiết Profile";
            return View("~/Views/Report/Detail.cshtml", transaction);
        }

        //public ActionResult JsonSearch(string keyword)
        //{
        //    keyword = "1523900004";
        //    eWallet.Data.DynamicObj transaction = (eWallet.Data.DynamicObj)Helper.DataHelper.Get("profile", Query.EQ("_id", long.Parse(keyword)));
        //    ViewBag.Title = "Chi tiết Result";
        //    return View("~/Views/Report/Detail.cshtml", transaction);
        //}

        public JsonResult JsonResult(int? _Id, string User_name, string Full_Name,string Personal_Id, string mobile,string status, int? page, int? page_size)
        {
            IMongoQuery query = Query.NE("type", "P");
            if (_Id != null)
                query = Query.And(
                    query,
                 Query.EQ("_id", _Id)
                 );
            if (!String.IsNullOrEmpty(status))
                query = Query.And(
                    query,
                    Query.EQ("status", status)
                    );
            if (!String.IsNullOrEmpty(mobile))
                query = Query.And(
                    query,
                    Query.EQ("mobile", mobile)
                    );
            if (!String.IsNullOrEmpty(User_name))
                query = Query.And(
                    query,
                    Query.EQ("user_name", User_name)
                    );
            if (!String.IsNullOrEmpty(Full_Name))
                query = Query.And(
                    query,
                    Query.EQ("full_name", Full_Name)
                    );
            if (!String.IsNullOrEmpty(Personal_Id))
                query = Query.And(
                    query,
                    Query.EQ("personal_id", Personal_Id)
                    );
                if (page == null) page = 1;
                if (page_size == null) page_size = 25;
         long total_page = 0;
         var _list = Helper.DataHelper.ListPagging("profile",
             query,
             SortBy.Ascending("_id"),
             (int)page_size,
             (int)page,
             out total_page
             );
         var list_accounts = (from e in _list select e).Select(p => new
         {
             _id = p._id,
             user_name = p.user_name,
             full_name = p.full_name,
             mobile = p.mobile,
             address = p.address,
             personal_id = p.personal_id,
             personal_id_issued_date = p.personal_id_issued_date,
             personal_id_issued_by = p.personal_id_issued_by,
             system_created_date = p.system_created_date,
             status = p.status
         }).ToArray();
         return Json(new { total = total_page, list = list_accounts, error_message = "o_0" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JsonListProfile(int? id, string status, string user_name, string full_name, string mobile,string personal_id, int? page, int? page_size)
        {
            IMongoQuery query = null;
            if (id != null)
                query = Query.And(
                    query,
                 Query.EQ("_id", id)  
                 );
            
            if (!String.IsNullOrEmpty(status))
                query = Query.And(
                    query,
                    Query.EQ("status", status)
                    );
            if (mobile != null)
                query = Query.And(
                    query,
                    Query.EQ("mobile", mobile)
                    );
            if (user_name != null)
                query = Query.And(
                    query,
                    Query.EQ("user_name", user_name)
                    );
            if (full_name != null)
                query = Query.And(
                    query,
                    Query.EQ("full_name", full_name)
                    );
            if (personal_id != null)
                query = Query.And(
                    query,
                    Query.EQ("personal_id", personal_id)
                    );
            if (page == null) page = 1;
            if (page_size == null) page_size = 25;
            long total_page = 0;
            var _list = Helper.DataHelper.ListPagging("profile",
                query,
                SortBy.Ascending("_id"),
                (int)page_size,
                (int)page,
                out total_page
                );
            var list_accounts = (from e in _list select e).Select(p => new
            {
                _id = p._id,
                user_name = p.user_name,
                full_name = p.full_name,
                mobile = p.mobile,
                personal_id=p.personal_id,
                status = p.status
            }).ToArray();
            return Json(new { total = total_page, list = list_accounts }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult JsonListChartOfProfile(int? page, int? page_size)
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

        //public ActionResult ListProfile()
        //{
        //    return View("~/Views/Box/CustomerProfile_ListProfile.cshtml");
        //}
        public ActionResult CustomerProfile_ListProfile()
        {
            return View("~/Views/Profile/CustomerProfile_ListProfile.cshtml");
        }
        public ActionResult ResultProfile()
        {
            return View("~/Views/Box/ResultProfile.cshtml");
        }
    }
}