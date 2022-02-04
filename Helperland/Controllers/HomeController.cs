using Helperland.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Helperland.Data;
using Microsoft.EntityFrameworkCore;
using System.Web;
using System.Net.Mail;
using System.Net;

namespace Helperland.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        HelperlanddContext db = new HelperlanddContext();
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult faqs()
        {
            return View();
        }

        public IActionResult Price()
        {
            return View();
        }

        public IActionResult Contact()
        { 
            return View();
        }
        [HttpPost]
        
        public IActionResult Contact(ContactU contactu)
        {
            ContactU contactUs = new ContactU();
            contactUs.Name = contactu.Name + contactu.Name;
            contactUs.Email = contactu.Email;
            contactUs.PhoneNumber = contactu.PhoneNumber;
            contactUs.Message = contactu.Message;
            contactUs.Subject = contactu.Subject;
            contactUs.CreatedBy = contactu.ContactUsId;
            contactUs.CreatedOn = DateTime.UtcNow; 
            db.ContactUs.Add(contactu);
            db.SaveChanges();
            return View("Contact");
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult _login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult _login(User u)
        {
            User Users = new User();
            var User = db.Users.Where(model => model.Email == u.Email && model.Password == u.Password).FirstOrDefault(); ;
            if (User != null)
            {
                ViewBag.SuccessMessase = "<script>alert('Login Successful)</script>";
                return RedirectToAction("About");
            }
            else
            {
                ViewBag.ErrorMessage = "<script>alert('Email or Password is incorrect')</script>";
                return PartialView("_login");
            }
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string EmailID)
        {
            string resetCode = Guid.NewGuid().ToString();
            var link = "<a href='" + Url.Action("ResetPassword", "Home", null, "http") + "'>Reset Password</a>";
            using (var db = new HelperlanddContext())
            {

                var getUser = (from s in db.Users where s.Email == EmailID select s).FirstOrDefault();
          
                if (getUser != null)
                {
                    //getUser.Resetpasscode = resetCode;
                    //db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();

                    var subject = "Password Reset Request";
                    var body = "Hi " + getUser.FirstName + ", <br/> You recently requested to reset your password for your account. Click the link below to reset it. " +

                         " <br/><br/><a href='" + link + "'>" + link + "</a> <br/><br/>" +
                         "If you did not request a password reset, please ignore this email or reply to let us know.<br/><br/> Thank you";

                    SendEmail(getUser.Email, body, subject);

                    ViewBag.Message = "Reset password link has been sent to your email id.";
                }
                else
                {
                    ViewBag.Message = "User doesn't exists.";
                    return View();
                }
            }

            return View();
        }

        private void SendEmail(string emailAddress, string body, string subject)

        {
            using (MailMessage mm = new MailMessage("Helperlandservices@gmail.com", emailAddress))
            {
                mm.Subject = subject;
                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("helperlandservices@gmail.com", "2022#helperland");
                smtp.EnableSsl = true;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.Send(mm);

            }
        }
        public ActionResult ResetPassword()
        {   
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(User u)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                using (var db = new HelperlanddContext())
                {
                    var getUser = db.Users.Where(p => p.Email == u.Email).FirstOrDefault();
                    if (getUser != null)
                    {
                        getUser.Password = u.Password;
                        getUser.Email = u.Email;
                        db.Users.Update(getUser);
                        db.SaveChanges();
                        message = "New password updated successfully";
                    }
                }
            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
            return View();
        }

        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateAccount(User user)
        {
            User users = new User();
            users.UserTypeId = 1;
            db.Users.Add(user);
            db.SaveChanges();
            return View("CreateAccount");
        }


        public IActionResult serviceproviderSignup()
        {
            return View();
        }
        [HttpPost]
        public IActionResult serviceproviderSignup(User user)
        {
            User users = new User();
            users.UserTypeId = 2;
            db.Users.Add(user);
            db.SaveChanges();
            return View("serviceproviderSignup");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    internal class HttpContext
    {
    }
}
