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
using Microsoft.AspNetCore.Http;

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
        [ValidateAntiForgeryToken]
        public IActionResult Contact(ContactU contactu)
        {
            contactu.Name = HttpContext.Request.Form["firstName"] + " " + HttpContext.Request.Form["lastName"];
            contactu.CreatedOn = DateTime.Now;
            contactu.CreatedBy = contactu.ContactUsId;
            db.ContactUs.Add(contactu);
            db.SaveChanges();
            TempData["Msg"] = "Your Response has been recorded";
            return RedirectToAction("Contact");
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

            var login_user = db.Users.Where(x => x.Email == u.Email && x.Password == u.Password).FirstOrDefault();
            /*User login_user = db.Users.Where(model => model.Email == u.Email && model.Password == u.Password).FirstOrDefault()*/
            
            if (login_user != null)
            {
                if (login_user.UserTypeId == 1)
                {
                    HttpContext.Session.SetInt32("User_id", login_user.UserId);
                    TempData["username"] = login_user.FirstName;
                    TempData["LastName"] = login_user.LastName;
                    TempData["Email"] = login_user.Email;
                    TempData["Mobile"] = login_user.Mobile;
                    return RedirectToAction("CustomerPages", "Home");
                }
                else if (login_user.UserTypeId == 2)
                {
                    HttpContext.Session.SetInt32("User_id", login_user.UserId);
                    TempData["username"] = login_user.FirstName;
                    TempData["LastName"] = login_user.LastName;
                    TempData["Email"] = login_user.Email;
                    TempData["Mobile"] = login_user.Mobile;
                    TempData["Password"] = login_user.Password;
                    return RedirectToAction("ServiceproviderPages", "Home");
                    
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ViewBag.message = "Username or Password is Incorrect. Please try again";
                return RedirectToAction("Price");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("User_id");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ForgetPassword(User user)
        {
            string resetCode = Guid.NewGuid().ToString();
            var link = "<a href='" + Url.Action("ResetPassword", "Home", null, "http") + "'>Reset Password</a>";
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
            if (ModelState.IsValid)
            {
                user.UserTypeId = 1;
                user.CreatedDate = DateTime.Now;
                user.ModifiedDate = DateTime.Now;
                db.Users.Add(user);
                db.SaveChanges();
                ViewBag.Msg = "Your Customer account is created!! Now go to Login.";
                return View();
            }
            return View();
        }

        public IActionResult serviceproviderSignup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult serviceproviderSignup(User user)
        {
            if (ModelState.IsValid)
            {
                user.UserTypeId = 2;
                user.CreatedDate = DateTime.Now;
                user.ModifiedDate = DateTime.Now;
                db.Users.Add(user);
                db.SaveChanges();
                ViewBag.Msg = "Your service Provider account is created!! Now go to Login";
                return View();
            }
            return View();
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

        public IActionResult CustomerPages()
        {
            return View();
        }

        public IActionResult ServiceRequest()
        {
            List<ServiceRequest> sr = db.ServiceRequests.Where(x => x.UserId == 13).ToList();
            return View(sr);
        }

        public IActionResult ServiceHistory()
        {
            List<ServiceRequest> sr = db.ServiceRequests.Where(x => x.UserId == 13).ToList();
            return View(sr);
        }

        public IActionResult Mysettings()
        {
            return View();
        }

        [HttpPost]
        public string mydetails([FromBody] User u)
        {
            db.Users.Update(u);
            db.SaveChanges();
            return "true";
        }
        public string saveAddress([FromBody] UserAddress address)
        {
            address.UserId = 13;
            address.IsDefault = false;
            address.IsDeleted = false;
            db.UserAddresses.Add(address);
            db.SaveChanges();
            return "true";
        }

        public IActionResult address()
        {
            List<UserAddress> add = db.UserAddresses.Where(x => x.UserId == 13).ToList();
            System.Threading.Thread.Sleep(2000);
            return View(add); 
        }

        public string updateaddress([FromBody] UserAddress change)
        {
            UserAddress add = db.UserAddresses.Where(x => x.AddressId == 4).FirstOrDefault();

            add.AddressLine1 = change.AddressLine1;
            add.AddressLine2 = change.AddressLine2;
            add.PostalCode = change.PostalCode;
            add.City = change.City;
            add.Mobile = change.Mobile;
            db.UserAddresses.Update(add);
            db.SaveChanges();
            return "true";

        }
        [HttpPost]
        public string password([FromBody] User u)
        {
            User user = new User();
            user.Password = u.Password;
            db.Users.Update(u);
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
