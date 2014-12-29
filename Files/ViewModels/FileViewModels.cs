using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Files.ViewModels
{
    public class FileViewModels
    {
        public class UploadViewModel
        {
            public HttpPostedFileBase File { get; set; }
        }
    }
}