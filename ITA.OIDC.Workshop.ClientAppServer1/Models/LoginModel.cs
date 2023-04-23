using System.ComponentModel.DataAnnotations;

namespace ITA.OIDC.Workshop.ClientAppServer1.Models;

public sealed class LoginModel
{
    [Display(Name = "Логин")]
    [Required(ErrorMessage = "Поле является обязательным")]
    public string Login { get; set; }
        
    [Display(Name = "Пароль")]
    [Required(ErrorMessage = "Поле является обязательным")]
    public string Password { get; set; }
        
    public string? ReturnUrl { get; set; }
}