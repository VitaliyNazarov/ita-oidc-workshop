using System.ComponentModel.DataAnnotations;

namespace ITA.OIDC.Workshop.AuthServer.Models
{
    public class AuthorizeViewModel
    {
        [Display(Name = "Application")]
        public string? ApplicationName { get; set; }

        [Display(Name = "Scope")]
        public string? Scope { get; set; }
    }
}
