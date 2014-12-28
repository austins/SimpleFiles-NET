using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;
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
            const string expiryCacheKey = "uploadsFolderLastModified";
            const short pageSize = 10;
            var pageIndex = page ?? 1;
            var filesCacheKey = "files_p" + pageIndex;
            var uploadsFolderPath = Server.MapPath("~/uploads");
            var totalFileCount = Directory.EnumerateFiles(uploadsFolderPath).Count();
            var cache = MemoryCache.Default;
            var cachedLastModified = Convert.ToDateTime(cache.Get(expiryCacheKey));
            var files = (StaticPagedList<File>) cache.Get(filesCacheKey);
            var uploadsFolderLastModified = Directory.GetLastWriteTimeUtc(uploadsFolderPath);

            if ((DateTime.Compare(cachedLastModified, uploadsFolderLastModified) != 0) || files == null)
            {
                // Reset the cache.
                foreach (var element in cache)
                    cache.Remove(element.Key);

                var uploadedFilePaths =
                    Directory.EnumerateFiles(uploadsFolderPath).Skip(pageSize*(pageIndex - 1)).Take(pageSize);

                var tempFiles = new List<File>();
                foreach (var path in uploadedFilePaths)
                    tempFiles.Add(new File(path));

                files = new StaticPagedList<File>(tempFiles, pageIndex, pageSize, totalFileCount);

                cache.Set(expiryCacheKey, uploadsFolderLastModified, ObjectCache.InfiniteAbsoluteExpiration);
                cache.Set(filesCacheKey, files, ObjectCache.InfiniteAbsoluteExpiration);
            }

            return View(files);
        }
    }
}