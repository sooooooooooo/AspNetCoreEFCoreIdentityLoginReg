using System.ComponentModel.DataAnnotations;

namespace LoginRegWithIdentity.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserNameLogin {get;set;}

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string PasswordLogin {get;set;}
    }
}