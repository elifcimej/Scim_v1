using System.ComponentModel.DataAnnotations;

namespace Scım_v1.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username alanı boş geçilemez.")]
        public string Username { get; set; }

        [Required(ErrorMessage ="Password boş geçilemez.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }

    }
}
