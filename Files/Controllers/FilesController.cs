using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using Files.ViewModels;
using PagedList;
using File = Files.Models.File;

namespace Files.Controllers
{
    [Authorize]
    public class FilesController : Controller
    {
        // GET: Files
        [Route("files/{page:int?}")]
        public ActionResult Index(int? page)
        {
            ViewBag.Files = Files.Models.File.GetFiles(Server.MapPath("~/uploads"), page ?? 1);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(FileViewModels.UploadViewModel upload)
        {
            //// Verify that the user selected a file
            //if (file != null && file.ContentLength > 0)
            //{
            //    // extract only the fielname
            //    var fileName = Path.GetFileName(file.FileName);
            //    // store the file inside ~/App_Data/uploads folder
            //    var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
            //    file.SaveAs(path);
            //}

            if (ModelState.IsValid)
            {
                //if ((upload.File != null) && (upload.File.ContentLength > 0))
                //{
                //    var fileName = Path.GetFileName(upload.File.FileName);
                //    var path = Path.Combine(Server.MapPath("~/uploads"), fileName);
                //    upload.File.SaveAs(path);
                //}
            }

            ViewBag.Files = Files.Models.File.GetFiles(Server.MapPath("~/uploads"));

            return View("Index", upload);        
        }
    }
}