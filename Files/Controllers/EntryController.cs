using System;
using System.Web.Mvc;
using System.Web.Security;
using Files.Library;
using Files.ViewModels;

namespace Files.Controllers
{
    public class EntryController : Controller
    {
        // GET: Home
        [Route("")]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Files");

            return RedirectToAction("SignIn");
        }

        public ActionResult CreatePassword()
        {
            if (!String.IsNullOrWhiteSpace(Config.Get("Password")))
                return RedirectToAction("Index");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePassword(EntryViewModels.CreatePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrWhiteSpace(model.Password) && !String.IsNullOrWhiteSpace(model.ConfirmPassword) &&
                    model.Password.Equals(model.ConfirmPassword))
                {
                    Config.Set("Password", PasswordHash.PasswordHash.CreateHash(model.Password));

                    FormsAuthentication.SetAuthCookie("File_User", false);

                    return RedirectToAction("Index", "Files");
                }

                ModelState.AddModelError("Password", "The passwords do not match.");
            }

            return View(model);
        }

        public ActionResult SignIn()
        {
            if (String.IsNullOrWhiteSpace(Config.Get("Password")))
                return RedirectToAction("CreatePassword");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(EntryViewModels.SignInViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (PasswordHash.PasswordHash.ValidatePassword(model.Password, Config.Get("Password")))
                {
                    FormsAuthentication.SetAuthCookie("File_User", false);

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "Files");
                }

                ModelState.AddModelError("Password", "The password is incorrect.");
            }

            return View(model);
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("SignIn");
        }
    }
}