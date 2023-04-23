using System.ComponentModel.DataAnnotations;

namespace ITA.OIDC.Workshop.AuthServer.Models
{
    public class LoginViewModel
    {
        [Display(Name = "Логин")]
        [Required(AllowEmptyStrings = false)]
        public string Login { get; set; } = string.Empty;
        
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; } = string.Empty;
        
        public bool? RememberMe { get; set; }
        
        public string? ReturnUrl { get; set; }
    }
}