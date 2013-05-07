using System.ComponentModel.DataAnnotations;

namespace NoPasswordWebsite.Models
{
    public class LoginModel
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Password { get; set; }
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
        [Display(Name = "Reset old cookies" )]
        public bool ResetOld { get; set; }
    }
}