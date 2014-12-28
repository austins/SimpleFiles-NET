using System.Web.Mvc;
using Files.ViewModels;

namespace Files.Controllers
{
    [Authorize]
    public class FilesController : Controller
    {
        // GET: Files
        [Route("files/{page:int?}")]
        public ActionResult Index(int? page)
        {
            ViewBag.Files = Models.File.GetFiles(Server.MapPath("~/uploads"), page ?? 1);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(FileViewModels.UploadViewModel upload)
        {
            if (ModelState.IsValid)
            {
                //if ((upload.File != null) && (upload.File.ContentLength > 0))
                //{
                //    var fileName = Path.GetFileName(upload.File.FileName);
                //    var path = Path.Combine(Server.MapPath("~/uploads"), fileName);
                //    upload.File.SaveAs(path);
                //}
            }

            ViewBag.Files = Models.File.GetFiles(Server.MapPath("~/uploads"));

            return View("Index", upload);
        }
    }
}