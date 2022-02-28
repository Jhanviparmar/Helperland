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
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(User u)
        {
            User Users = new User();
            var User = db.Users.Where(model => model.Email == u.Email && model.Password == u.Password).FirstOrDefault(); ;
            if (User != null)
            {
                return RedirectToAction("About");
            }
            else
            {
                TempData["ErrorMessage"] = "*Username or Password is invalid";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult ForgetPassword(User user)
        {
            string resetCode = Guid.NewGuid().ToString();
            var link = "<a href='" + Url.Action("ResetPassword", "Home", null, "http") + "'>Reset Password</a>";
            //string baseUrl = string.Format("{0}://{1}",
            //           HttpContext.Request.Scheme, HttpContext.Request.Host);
            using (var db = new HelperlanddContext())
            {

                var getUser = db.Users.FirstOrDefault(x => x.Email.Equals(user.Email));

                if (getUser != null)
                {
                    var senderemail = new MailAddress("Helperlandservices@gmail.com", "Reset your password");
                    var receiveremail = new MailAddress(user.Email);

                    var password = "2022#helperland";
                    var subject = "Password Reset Request";
                    var body = "Hi " + getUser.FirstName + ", You recently requested to reset your password for your account. Click the link to reset it. " 
                                     + link + 
                                     "If you did not request a password reset, please ignore this email or reply to let us know.Thank you";
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderemail.Address, password)
                    };
                    using (var message = new MailMessage(senderemail, receiveremail)
                    {
                        Subject = subject,
                        Body = body

                    })
                    {
                        smtp.Send(message);
                    }
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    return View();

                }
            }
        }

        public ActionResult ResetPassword()
        {   
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(User u)
        {
            var getUser = db.Users.FirstOrDefault(x => x.Email.Equals(u.Email));
            if (getUser != null)
            {
                getUser.Email = u.Email;
                getUser.Password = u.Password;
                db.Users.Update(getUser);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Error"] = "Invalid email";
                return RedirectToAction("ResetPassword");
            }
        }
        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateAccount(User user)
        {
            User users = new User();
            user.UserTypeId = 1;
            db.Users.Add(user);
            db.SaveChanges();
            return View("Index");
        }

        public IActionResult serviceproviderSignup()
        {
            return View();
        }
        [HttpPost]
        public IActionResult serviceproviderSignup(User user)
        {
            //User users = new User();
            user.UserTypeId = 2;
            db.Users.Add(user);
            db.SaveChanges();
            return View("Index");
        }

        public IActionResult BookService()
        {
            return View();
        }

        public string PostalCode(string postalcode)
        {

            var PostalCode = db.Zipcodes.Where(x => x.ZipcodeValue == postalcode).SingleOrDefault();
            string IsValid;
            if (PostalCode != null)
            {
                IsValid = "true";
            }
            else
            {
                IsValid = "false";
            }
            return IsValid;

        }
        [HttpPost]
        public string ConfirmServiceRequest([FromBody] ServiceRequest sr)
        {
            
            sr.UserId = 13;
            sr.ServiceId = 8570;
            sr.ServiceHourlyRate = 9;
            sr.PaymentDue = true;
            sr.CreatedDate = DateTime.Now;
            sr.ModifiedDate = DateTime.Now;
            sr.ModifiedBy = 13;
            sr.Distance = 15;
            db.ServiceRequests.Add(sr);
            db.SaveChanges();
            return "true";
        }


        public IActionResult Details()
        {
            List<UserAddress> add = db.UserAddresses.Where(x => x.UserId == 13).ToList();
            System.Threading.Thread.Sleep(2000);
            return View(add);
        }

        public string SaveAddress([FromBody] UserAddress address)
        {
            address.UserId = 13;
            address.IsDefault = false;
            address.IsDeleted = false;
            db.UserAddresses.Add(address);
            db.SaveChanges();
            return "true";
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
