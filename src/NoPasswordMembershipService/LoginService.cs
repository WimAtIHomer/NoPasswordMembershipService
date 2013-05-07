using System;
using System.Globalization;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;

namespace NoPasswordMembershipService
{
    public class LoginService
    {
        private readonly INoPasswordUserRepository _noPasswordUserRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="noPasswordUserRepository"></param>
        public LoginService(INoPasswordUserRepository noPasswordUserRepository)
        {
            _noPasswordUserRepository = noPasswordUserRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        public string SetRandomLoginPassword(string email)
        {
            var user = _noPasswordUserRepository.Find(email);
            if (user == null)
            {
                throw new AuthenticationException("Email does not exists");
            }
			// generate random 12 character password, with 4 special characters
			var password = Membership.GeneratePassword(12, 4);
			var algorithm = HashAlgorithm.Create("SHA256");
            user.Password = Convert.ToBase64String(algorithm.ComputeHash(Encoding.UTF8.GetBytes(password)));
            _noPasswordUserRepository.SaveChanges();
            return password;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="isPersistent"></param>
        /// <param name="resetOld"></param>
        /// <returns></returns>
        public bool Login(string email, string password, bool isPersistent, bool resetOld)
        {
            var user = _noPasswordUserRepository.Find(email);
            if (user == null)
            {
                throw new AuthenticationException("Email does not exists");
            }
			var algorithm = HashAlgorithm.Create("SHA256");
            var hashPassword = Convert.ToBase64String(algorithm.ComputeHash(Encoding.UTF8.GetBytes(password)));
            if (user.Password != hashPassword)
            {
                user.Password = null;
                _noPasswordUserRepository.SaveChanges();
                throw new AuthenticationException("Password does not match, password has been reset request a new password");
            }
            user.Password = null;
            user.LastLoginDate = DateTime.Now;
            if (resetOld)
            {
                user.ResetDate = DateTime.Now;
            }
            _noPasswordUserRepository.SaveChanges();
            var ticket = new FormsAuthenticationTicket(1,
                    email,
                    DateTime.Now,
                    isPersistent ? DateTime.Now.Add(FormsAuthentication.Timeout) : DateTime.Now.AddMinutes(30),
                    isPersistent,
                    DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture),
                    FormsAuthentication.FormsCookiePath);

            // Encrypt the ticket.
            var encTicket = FormsAuthentication.Encrypt(ticket);
            // Create the cookie.
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket)
                {
                    HttpOnly = true,
                    Secure = FormsAuthentication.RequireSSL
                };
            HttpContext.Current.Response.Cookies.Add(cookie); 

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CheckDate()
        {
            var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null) return false;
            var decryptedCookie = FormsAuthentication.Decrypt(cookie.Value);
            if (decryptedCookie == null) return false;
            var email = decryptedCookie.Name;
            var user = _noPasswordUserRepository.Find(email);
            if (user == null)
            {
                throw new AuthenticationException("Email does not exists");
            }
            long loginDateTicks;
            if (!long.TryParse(decryptedCookie.UserData, NumberStyles.None, CultureInfo.InvariantCulture, out loginDateTicks))
            {
                throw new AuthenticationException("Cookie login date not present");
            }
            var loginDate = new DateTime(loginDateTicks);
            if (user.ResetDate > loginDate)
            {
                throw new AuthenticationException("Cookie login date is before reset date, request a new email");
            }
            user.LastLoginDate = DateTime.Now;
            _noPasswordUserRepository.SaveChanges();
            return true;
        }
    }
}
