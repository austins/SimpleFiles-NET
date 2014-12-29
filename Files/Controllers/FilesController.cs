using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;
using Files.ViewModels;

namespace Files.Controllers
{
    [Authorize]
    public class FilesController : Controller
    {
        private const long _maxFileSize = 5000000000;
        private readonly string[] _fileTypesAllowed = { "image/png", "image/jpeg", "image/pjpeg", "image/gif", "text/plain", "application/zip", "application/x-rar-compressed", "audio/mpeg" };

        // GET: Files
        [Route("files/{page:int?}")]
        public ActionResult Index(int? page)
        {
            ViewBag.FileTypesAllowed = _fileTypesAllowed;
            ViewBag.Files = Models.File.GetFiles(Server.MapPath("~/uploads"), page ?? 1);

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
                string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
                string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
                fileName = System.Text.RegularExpressions.Regex.Replace(fileName, invalidRegStr, "_");
                fileName = fileName.Replace("+", "_"); // Remove double escape sequence.

                uploadPath = Path.Combine(Server.MapPath("~/uploads"), fileName);

                // Check if file with the name already exists.
                if (System.IO.File.Exists(uploadPath))
                    ModelState.AddModelError("File", "A file with the name \"" + fileName + "\" already exists.");

                // Check file size.
                if (upload.File.ContentLength > _maxFileSize)
                    ModelState.AddModelError("File",
                        "The file you uploaded is too large. The max file size allowed is: " +
                        Files.Models.File.FormatSize(_maxFileSize));

                // Only allow certain file formats.
                var fileType = Models.File.GetMimeTypeFromFile(tempPath);
                if (Array.IndexOf(_fileTypesAllowed, fileType) == -1)
                    ModelState.AddModelError("File", "The file you uploaded is not of an allowed file type (" + fileType + ").");
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