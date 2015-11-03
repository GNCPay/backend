using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;

using Microsoft.Owin.Security;
using eWallet.Backend.Models;
using MongoDB.AspNet.Identity;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using System.Collections;
using System.IO;
using System.Text;
using System.Web.Security;
using System.Security.Cryptography;

namespace eWallet.Backend.Controllers
{

    [Authorize]
    public class AccountController : Controller
    {
               
        public ActionResult Setting()
        {
            return View();
        }

        [Authorize(Roles = "SYSTEM")]
        public ActionResult AccountManagement()
        {
            //List Channel Here
            ViewBag.list_users = Helper.DataHelper.List("users", null);
            return View("~/Views/Box/Account_ProfileManagement.cshtml");
        }

        //public JsonResult UpdateOrganization(string _id, string Organization_code)
        //{
        //    var id = new ObjectId(_id);
        //    dynamic user = Helper.DataHelper.Get("users", Query.EQ("_id", id));
        //    user.Organization_code = Organization_code;
        //    Helper.DataHelper.SaveUpdate("users", user);
        //    return Json(new { error_code = "00", error_message = "Cập nhật Organization_code thành công !" }, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult UpdateStatus(string _id, string Status)
        {
            var id = new ObjectId(_id);
            dynamic user = Helper.DataHelper.Get("users", Query.EQ("_id", id));
            user.Status = Status;
            Helper.DataHelper.SaveUpdate("users", user);
            return Json(new { error_code = "00", error_message = "cập nhật trạng thái thành công !" }, JsonRequestBehavior.AllowGet);
        }

      public enum Supported_HA
      {
          SHA256,  SHA384,
          SHA512
      }
        class Hashing
        {
            public static string ComputeHash(string plaintext, Supported_HA hash, byte[] salt)
            {
                int minSaltLength = 4;
                int maxSaltLenght = 6;
                byte[] SaltBytes = null;

                if(salt!=null)
                {
                    SaltBytes = salt;
                }
                else
                {
                    Random r = new Random();
                    int SaltLenght = r.Next(minSaltLength, maxSaltLenght);
                    SaltBytes = new byte[SaltLenght];
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    rng.GetNonZeroBytes(SaltBytes);
                    rng.Dispose();
                }

                byte[] plaintData = ASCIIEncoding.UTF8.GetBytes(plaintext);
                byte[] plainDataAndSalt = new byte[plaintData.Length + SaltBytes.Length];

                for(int x=0; x<plaintData.Length;x++)
                {
                    plainDataAndSalt[x] = plaintData[x];
                    
                }
                for (int n = 0; n < SaltBytes.Length; n++)
                    plainDataAndSalt[plaintData.Length + n] = SaltBytes[n];

                byte[] hashValue = null;

                switch(hash)
                {
                    case Supported_HA.SHA256:
                        SHA256Managed sha= new SHA256Managed();
                        hashValue= sha.ComputeHash(plainDataAndSalt);
                        sha.Dispose();
                        break;

                    case Supported_HA.SHA384:
                        SHA384Managed sha1 = new SHA384Managed();
                        hashValue = sha1.ComputeHash(plainDataAndSalt);
                        sha1.Dispose();
                        break;
                    case Supported_HA.SHA512:
                        SHA512Managed sha2 = new SHA512Managed();
                        hashValue = sha2.ComputeHash(plainDataAndSalt);
                        sha2.Dispose();
                        break;
                }

                byte[] resuflt = new byte[hashValue.Length + SaltBytes.Length];
                for (int x = 0; x < hashValue.Length; x++)
                    resuflt[x] = hashValue[x];
                for (int n = 0; n < SaltBytes.Length; n++)
                    resuflt[hashValue.Length + n] = SaltBytes[n];
                return Convert.ToBase64String(resuflt);
            }

            public static bool Confirm(string plainText, string hashValue, Supported_HA hash)
            {
                byte[] hashBytes = Convert.FromBase64String(hashValue);
                int hashSize = 0;
                switch(hash)
                {
                    case Supported_HA.SHA256:
                        hashSize = 32;
                        break;
                    case Supported_HA.SHA384:
                        hashSize = 48;
                        break;
                    case Supported_HA.SHA512:
                        hashSize = 64;
                        break;
                }
                byte[] saltBytes = new byte[hashBytes.Length - hashSize];
                for (int x = 0; x < saltBytes.Length; x++)
                    saltBytes[x] = hashBytes[hashSize + x];
                string NewHash = ComputeHash(plainText, hash, saltBytes);

                return (hashValue == NewHash);
            }
        }
        public JsonResult ResetPassWord(string _id)
        {     
            var id = new ObjectId(_id);           
            dynamic user = Helper.DataHelper.Get("users", Query.EQ("_id", id));
            string passwordnew="123456";
            string hasspassnew = UserManager.PasswordHasher.HashPassword(passwordnew);
            user.PasswordHash = hasspassnew;
            Helper.DataHelper.SaveUpdate("users", user);
            return Json(new { error_code = "00", error_message = "cập nhật trạng thái thành công !" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateRoles(string _id, string[] Roles)
        {
            var id = new ObjectId(_id);
            dynamic user = Helper.DataHelper.Get("users", Query.EQ("_id", id));
            user.Roles = Roles;
            Helper.DataHelper.SaveUpdate("users", user);
            return Json(new { error_code = "00", error_message = "cập nhật phân quyền thành công!" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UserNameResult(string Username, int? page, int? page_size)
        {
            IMongoQuery query = null;
            if (!String.IsNullOrEmpty(Username))
                query = (query == null) ? Query.EQ("UserName", Username.ToLower()) : Query.And(
                    query,
                    Query.EQ("UserName", Username.ToLower())
                    );
            if (page == null) page = 1;
            if (page_size == null) page_size = 25;
            long total_page = 0;
            var _list = Helper.DataHelper.ListPagging("users",
                query,
                SortBy.Descending("_id"),
                (int)page_size,
                (int)page,
                out total_page
                );
            var list_accounts = (from e in _list select e).Select(p => new
            {
                _id = p._id.ToString(),
                UserName = p.UserName,
                Roles = p.Roles,
                Status = p.Status
            }).ToArray();     
            return Json(new { total = total_page, list = list_accounts }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RolesResult(Int64? _id, string UserName, string Roles, string Organization_code, int? page, int? page_size)
        {
            IMongoQuery query = null;
            if (_id!=null)
                query = (query == null) ? Query.EQ("users", _id) : Query.And(
                    query,
                    Query.EQ("users", _id)
                    );
            if (!String.IsNullOrEmpty(UserName))
                query = (query == null) ? Query.EQ("users", UserName) : Query.And(
                    query,
                    Query.EQ("users", UserName)
                    );
            if (!String.IsNullOrEmpty(Roles))
                query = (query == null) ? Query.EQ("users", Roles) : Query.And(
                    query,
                    Query.EQ("users", Roles)
                    );
            if (page == null) page = 1;
            if (page_size == null) page_size = 25;
            long total_page = 0;
            var _list = Helper.DataHelper.ListPagging("users",
                query,
                SortBy.Descending("_id"),
                (int)page_size,
                (int)page,
                out total_page
                );
            var list_accounts = (from e in _list select e).Select(p => new
            {
                _id=p._id.ToString(),
                UserName = p.UserName,
                Roles = p.Roles,
                //Organization_code=p.Organization_code,
                Status = p.Status
            }).ToArray();
            return Json(new { total = total_page, list = list_accounts }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "SYSTEM")]
        public ActionResult RoleManagement()
        {
            //List Channel Here
            ViewBag.list_channels = Helper.DataHelper.List("config", null);
            return View("~/Views/Box/Account_RoleManagement.cshtml");
        }
        public AccountController()
        //: this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
            this.UserManager = new UserManager<ApplicationUser>
                (
                new UserStore<ApplicationUser>("CoreDB_Server")
                );
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        //[Authorize(Roles = "SYSTEM, MERCHANT, GNC, CUSTOMER")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            dynamic profile = Helper.DataHelper.Get("users", Query.EQ("UserName", model.UserName));

            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    if(user.Roles[0]== "SYSTEM" || user.Roles[0] == "MERCHANT" || user.Roles[0] == "GNC")
                    {
                        if (profile.Status != "LOCKED")
                        {
                            await SignInAsync(user, model.RememberMe);
                            return RedirectToLocal(returnUrl);
                        }
                        else
                        {
                            ModelState.AddModelError("", "tài khoản của bạn đang bị khóa vui lòng liên hệ admin để được hỗ trợ !");
                        }
                    }
                    else
                        ModelState.AddModelError("", "tài khoản của bạn không thể đăng nhâp vui lòng liên hệ admin để được hỗ trợ !");
                }
                else
                    ModelState.AddModelError("", "Invalid username or password.");
            }           

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if(CheckIphone(model.Mobile)==true)
                {
                    if(CheckPhoneSupport(model.Mobile)==true)
                    {
                        var user = new ApplicationUser() { UserName = model.UserName };
                        var result = await UserManager.CreateAsync(user, model.Password);
                        if (result.Succeeded)
                        {
                            //add Roles to User
                            string[] roles = new string[] { "CUSTOMER" };
                            var roleResult = await UserManager.AddToRoleAsync(user.Id, roles[0]);
                            await SignInAsync(user, isPersistent: false);
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            AddErrors(result);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Số điện thoại không chính xác !");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Số điện thoại đã tồn tại!");
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        // kiem tra sdt
        public static bool CheckIphone(string iphone)
        {
            string a = iphone.Insert(0, "84");
            a = a.Remove(2, 1);
            dynamic profile = Helper.DataHelper.Get("profile", Query.EQ("mobile", a));
            if (profile != null)
            {
                return false;
            }
            return true;
        }
        public static bool CheckPhoneSupport(string phone_number)
        {
            const int RegionConuntryCode = 84;

            if (phone_number.Length == 10)
            {
                if (phone_number.StartsWith("+"))
                {
                    phone_number = phone_number.Replace("+", "0");
                }

                if (phone_number.StartsWith("0" + RegionConuntryCode))
                {
                    phone_number = phone_number.Replace("0" + RegionConuntryCode, "0");
                }
                string[] networkSupport_2 = { "096", "097", "098", "090", "093", "091", "094", "092", "099" };
                const int networkLength = 3;
                var startphone_number = phone_number.Substring(0, networkLength);
                return networkSupport_2.Any(startphone_number.Equals);

            }
            if (phone_number.Length == 11)
            {
                if (phone_number.StartsWith("+"))
                {
                    phone_number = phone_number.Replace("+", "0");
                }

                if (phone_number.StartsWith("0" + RegionConuntryCode))
                {
                    phone_number = phone_number.Replace("0" + RegionConuntryCode, "0");
                }

                string[] networkSupport_1 = {"0162", "0163", "0164", "0165", "0166", "0167", "0168", "0169",
            "0120", "0121", "0122","0126","0128",
            "0123","0124","0125","0127","0129",
            "0188", "0186",
            "0199"};
                const int networkLength_2 = 4;
                var startphone_number_2 = phone_number.Substring(0, networkLength_2);
                return networkSupport_1.Any(startphone_number_2.Equals);
            }
            return false;
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        TempData["alertMessage"] = "Mat khau thay doi thanh cong !";
                        return RedirectToAction("Manage");
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                if (CheckIphone(model.Mobile) == true)
                {
                    if (CheckPhoneSupport(model.Mobile) == true)
                    {
                        var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                        if (info == null)
                        {
                            return View("ExternalLoginFailure");
                        }
                        var user = new ApplicationUser() { UserName = model.UserName };
                        var result = await UserManager.CreateAsync(user);
                        if (result.Succeeded)
                        {
                            result = await UserManager.AddLoginAsync(user.Id, info.Login);
                            if (result.Succeeded)
                            {
                                string[] roles = new string[] { "CUSTOMER" };
                                var roleResult = await UserManager.AddToRoleAsync(user.Id, roles[0]);
                                await SignInAsync(user, isPersistent: false);
                                return RedirectToLocal(returnUrl);
                            }
                        }
                        AddErrors(result);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Số điện thoại không chính xác !");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Số điện thoại đã tồn tại!");
                }
                // Get the information about the user from the external login provider     
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}