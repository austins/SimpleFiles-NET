using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using SimpleFiles.ViewModels;

namespace SimpleFiles.Controllers
{
    [Authorize]
    public class FilesController : Controller
    {
        private const long MaxFileSize = 5000000000; // 5 GB

        private readonly string[] _fileTypesAllowed =
        {
            "image/png", "image/jpeg", "image/pjpeg", "image/gif",
            "text/plain", "application/zip", "application/x-rar-compressed", "audio/mpeg"
        };

        // GET: Files
        [Route("files/{page:int?}")]
        public ActionResult Index(int? page)
        {
            ViewBag.FileTypesAllowed = _fileTypesAllowed;

            ViewBag.SearchTerm = null;
            if (!String.IsNullOrWhiteSpace(Request.QueryString["search"]))
                ViewBag.SearchTerm = Request.QueryString["search"].Trim();

            ViewBag.Files = Models.File.GetFiles(Server.MapPath("~/uploads"), page ?? 1, ViewBag.SearchTerm);

            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("files")]
        public ActionResult Index(FileViewModels.UploadViewModel upload)
        {
            var uploadPath = "";
            var tempPath = "";

            // Check if a file has been sent from the form.
            if ((upload.File == null) || (upload.File.ContentLength == 0))
                ModelState.AddModelError("File", "No file uploaded.");
            else
            {
                tempPath = Path.GetTempFileName();
                upload.File.SaveAs(tempPath);

                var fileName = Path.GetFileName(upload.File.FileName);

                // Sanitize file name.
                var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
                var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
                fileName = Regex.Replace(fileName, invalidRegStr, "_");
                fileName = fileName.Replace("+", "_"); // Remove double escape sequence.

                uploadPath = Path.Combine(Server.MapPath("~/uploads"), fileName);

                // Check if file with the name already exists.
                if (System.IO.File.Exists(uploadPath))
                    ModelState.AddModelError("File", "A file with the name \"" + fileName + "\" already exists.");

                // Check file size.
                if (upload.File.ContentLength > MaxFileSize)
                    ModelState.AddModelError("File",
                        "The file you uploaded is too large. The max file size allowed is: " +
                        Models.File.FormatSize(MaxFileSize));

                // Only allow certain file formats.
                var fileType = Models.File.GetMimeTypeFromFile(tempPath);
                if (Array.IndexOf(_fileTypesAllowed, fileType) == -1)
                    ModelState.AddModelError("File",
                        "The file you uploaded is not of an allowed file type (" + fileType + ").");
            }

            // Don't continue with the upload if there are any errors.
            if (!ModelState.IsValid)
            {
                if (tempPath != "")
                    System.IO.File.Delete(tempPath);

                ViewBag.FileTypesAllowed = _fileTypesAllowed;
                ViewBag.Files = Models.File.GetFiles(Server.MapPath("~/uploads"));

                return View(upload);
            }

            // Upload the file.
            System.IO.File.Move(tempPath, uploadPath);

            return RedirectToAction("Index");
        }
    }
}