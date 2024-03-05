using System.ComponentModel.DataAnnotations;
namespace SportsStore.Models.ViewModels
{
    public class LoginModel
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Password { get; set; }
        
        // ReturnUrl property stores the URL to redirect the user after successful login.
        public string ReturnUrl { get; set; } = "/";
    }
}
