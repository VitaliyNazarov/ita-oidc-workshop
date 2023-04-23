using System.ComponentModel.DataAnnotations;

namespace ITA.OIDC.Workshop.AuthServer.Models
{
    public class LoginViewModel
    {
        [Display(Name = "Логин")]
        [Required(AllowEmptyStrings = false)]
        public string Login { get; set; }
        
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
        
        public bool? RememberMe { get; set; }
        
        public string? ReturnUrl { get; set; }
    }
}