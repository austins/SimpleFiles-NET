using System.Web;

namespace SimpleFiles.ViewModels
{
    public class FileViewModels
    {
        public class UploadViewModel
        {
            public HttpPostedFileBase File { get; set; }
        }
    }
}