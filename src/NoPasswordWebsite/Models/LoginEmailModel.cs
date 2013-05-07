using System.ComponentModel.DataAnnotations;

namespace NoPasswordWebsite.Models
{
    public class LoginEmailModel
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}