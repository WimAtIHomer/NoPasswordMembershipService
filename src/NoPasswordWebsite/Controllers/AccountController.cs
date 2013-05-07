using System;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Security;
using NoPasswordMembershipService;
using NoPasswordWebsite.Entities;
using NoPasswordWebsite.Models;

namespace NoPasswordWebsite.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserContext _userContext;
        private readonly UserRepository _userRepository;
        private readonly LoginService _loginService;

        public AccountController()
        {
            _userContext = new UserContext();
            _userRepository = new UserRepository(_userContext);
            _loginService = new LoginService(_userRepository);
        }

        //
        // GET: /Account/

        public ActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            {
                return View("SendLogin");
            }
            var model = new LoginModel
                {
                    Email = email,
                    Password = password,
                    RememberMe = false,
                    ResetOld = false
                };
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
			try
			{
				if (_loginService.Login(model.Email, model.Password, model.RememberMe, model.ResetOld))
				{
					return RedirectToAction("Index", "Home");
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("Email", ex.Message);
			}
            return View();
        }

        [HttpPost]
        public ActionResult SendLogin(LoginEmailModel model)
        {
			try 
			{
				var password = _loginService.SetRandomLoginPassword(model.Email);
				var user = _userRepository.Find(model.Email);
				SendLoginEmail(model.Email, "Login", user.Name, password);
				var userModel = new RegisterModel
					{
						Email = user.Email,
						UserName = user.Name
					};
				return View("LoginWelkom", userModel);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("Email", ex.Message);
			}
            return View();				
        }


        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
			try 
			{
				var user = new User
					{
						Name = model.UserName,
						Email = model.Email,
						IsAdmin = false,
						LastLoginDate = DateTime.Now
					};
				_userContext.Users.Add(user);
				_userContext.SaveChanges();
				var password = _loginService.SetRandomLoginPassword(model.Email);
				SendLoginEmail(model.Email, "Login", user.Name, password);
				return View("LoginWelkom", model);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("Email", ex.Message);
			}
            return View();			
        }

        public ActionResult DeleteUser(string id)
        {
            var user = _userRepository.Find(id);
            _userContext.Users.Remove(user);
            _userRepository.SaveChanges();
            return RedirectToAction("Register");
        }

        public ActionResult Manage()
        {
            return View();
        }

        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private void SendLoginEmail(string email, string subject, string username, string password)
        {
			var message = string.Format(@"<p>Welkom {0}<br /> <br /> To login click <a href='http://localhost:2727/Account/Login/?email={1}&password={2}'>here</a>, <br />", username, Url.Encode(email), Url.Encode(password));
            var mail = new MailMessage();
            var smtp = new SmtpClient();
            mail.To.Add(new MailAddress(email));
            mail.Subject = subject;
            mail.Body = message;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.Normal;

            smtp.Send(mail);
        }
    }
}
