using System.ComponentModel.DataAnnotations;

namespace Files.ViewModels
{
    public class EntryViewModels
    {
        public class CreatePasswordViewModel
        {
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required(ErrorMessage = "The Confirm Password field is required.")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            public string ConfirmPassword { get; set; }
        }

        public class SignInViewModel
        {
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
}