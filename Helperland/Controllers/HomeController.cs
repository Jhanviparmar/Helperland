using Helperland.Data;
using Helperland.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;

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
        

        ////private readonly HelperlanddContext _DbContext;

        ////public HomeController(HelperlanddContext DbContext)
        ////{
        ////    _DbContext = DbContext;
        ////}

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
            if (login_user != null)
            {
                if (login_user.UserTypeId == 1)
                {
                    HttpContext.Session.SetInt32("User_id", login_user.UserId);
                    HttpContext.Session.SetInt32("Usertype_id", login_user.UserTypeId);
                    HttpContext.Session.SetString("username", u.FirstName + " " + u.LastName);
                    return RedirectToAction("sdashboard", "Home");
                }
                else if (login_user.UserTypeId == 2)
                {
                    HttpContext.Session.SetInt32("User_id", login_user.UserId);
                    HttpContext.Session.SetInt32("Usertype_id", login_user.UserTypeId);
                    HttpContext.Session.SetString("username", u.FirstName + " " + u.LastName);
                    return RedirectToAction("SPDashboard", "Home");
                    
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
                user.IsRegisteredUser = false;
                user.WorksWithPets = false;
                user.CreatedDate = DateTime.Now;
                user.ModifiedDate = DateTime.Now;
                user.ModifiedBy = 0;
                user.IsApproved = false;
                user.IsActive = true;
                user.IsDeleted = false;
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
                user.IsRegisteredUser = false;
                user.WorksWithPets = false;
                user.CreatedDate = DateTime.Now;
                user.ModifiedDate = DateTime.Now;
                user.ModifiedBy = 0;
                user.IsApproved = false;
                user.IsActive = true;
                user.IsDeleted = false;
                db.Users.Add(user);
                db.SaveChanges();
                ViewBag.Msg = "Your service Provider account is created!! Now go to Login";
                return View();
            }
            return View();
        }

        public IActionResult BookService()
        {
            var UserID = HttpContext.Session.GetInt32("User_id");

            if (UserID != null)
            {
                return View();
            }
            ViewBag.msg = "<alert>You have to login first to Book a Service.</alert>";
            return RedirectToAction("Index");
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

        public IActionResult Details()
        {
            var UserID = HttpContext.Session.GetInt32("User_id");
            List<UserAddress> add = db.UserAddresses.Where(x => x.UserId == UserID).ToList();
            System.Threading.Thread.Sleep(2000);
            return View(add);

        }

        public string SaveAddress([FromBody] UserAddress address)
        {
            address.UserId = (int)HttpContext.Session.GetInt32("User_id");
            address.IsDefault = false;
            address.IsDeleted = false;
            db.UserAddresses.Add(address);
            db.SaveChanges();
            return "true";
        }

        [HttpPost]
        public string ConfirmServiceRequest([FromBody] ServiceRequest sr)
        {

            sr.UserId = (int)HttpContext.Session.GetInt32("User_id");
            //UserAddress address = db.UserAddresses.Where(x => x.AddressId == sr.AddressId).SingleOrDefault();
            sr.ServiceId = 8570;
            sr.ServiceHourlyRate = 9;
            sr.PaymentDue = true;
            sr.CreatedDate = DateTime.Now;
            sr.ModifiedDate = DateTime.Now;
            sr.ModifiedBy = 13;
            sr.Distance = 15;
            sr.Status = 0;
            db.ServiceRequests.Add(sr);
            db.SaveChanges();
            return "true";
        }

        public IActionResult sdashboard()
        {
            int userid = (int)HttpContext.Session.GetInt32("User_id");
            List<ServiceRequest> obj = new List<ServiceRequest>();
            obj = db.ServiceRequests.Where(x => x.UserId == userid && x.Status == 0).ToList();
            return View(obj);
        }

        public IActionResult shistory()
        {
            int userid = (int)HttpContext.Session.GetInt32("User_id");
            List<ServiceRequest> obj = db.ServiceRequests.Where(x => x.UserId == userid && x.Status == 2 || x.Status == 3).ToList();
            return View(obj);
        }

        public bool CancelRequest([FromBody] ServiceRequest sr)
        {
            ServiceRequest obj = db.ServiceRequests.Where(x => x.ServiceRequestId == sr.ServiceRequestId).FirstOrDefault();
            obj.Status = 3;
            obj.Comments = sr.Comments;
            db.ServiceRequests.Update(obj);
            db.SaveChanges();
            return true;
        }

        public IActionResult favouritepros()
        {
            return View();
        }

        public IActionResult settings()
        {
            var a = (int)HttpContext.Session.GetInt32("User_id");
            User u = db.Users.Where(x => x.UserId == a).FirstOrDefault();
            return View(u);
        }

        public string UpdateDetails([FromBody] User book)
        {
            var a = (int)HttpContext.Session.GetInt32("User_id");
            User u = db.Users.Where(x => x.UserId == a).FirstOrDefault();
            u.FirstName = book.FirstName;
            u.LastName = book.LastName;
            u.Mobile = book.Mobile;
            db.Users.Update(u);
            db.SaveChanges();

            
            HttpContext.Session.SetString("username", u.FirstName + " " + u.LastName);
            return "true";
        }
        public string Updatepassword([FromBody] User password)
        {  
                var a = (int)HttpContext.Session.GetInt32("User_id");
                User u = db.Users.Where(x => x.UserId == a).FirstOrDefault();
                if (u.Password == password.Password)
                {
                    u.Password = password.ConfirmPassword;
                    db.Users.Update(u);
                    db.SaveChanges();
                    return "true";
                }
                return "false";
        }
        public IActionResult CustomerAddress()
        {
            var UserID = HttpContext.Session.GetInt32("User_id");
            List<UserAddress> add = db.UserAddresses.Where(x => x.UserId == UserID).ToList();
            return View(add);

        }

        public string SaveNewAddress([FromBody] UserAddress address)
        {
            address.UserId = (int)HttpContext.Session.GetInt32("User_id");
            address.IsDefault = false;
            address.IsDeleted = false;
            db.UserAddresses.Add(address);
            db.SaveChanges();
            return "true";
        }

        public IActionResult EditAddress()
        {
            var a = (int)HttpContext.Session.GetInt32("User_id");
            UserAddress u = db.UserAddresses.Where(x => x.AddressId == a).FirstOrDefault();
            return View(u);
        }

        [HttpPost]
        public string Editaddress([FromBody] UserAddress add)
        {
            var a = (int)HttpContext.Session.GetInt32("User_id");
            UserAddress ua = db.UserAddresses.Where(x => x.UserId == a).FirstOrDefault();
            ua.AddressLine1 = add.AddressLine1;
            ua.AddressLine2 = add.AddressLine2;
            ua.PostalCode = add.PostalCode;
            ua.City = add.City;
            ua.Mobile = add.Mobile;
            db.UserAddresses.Update(ua);
            db.SaveChanges();
            return "true";
        }

        [HttpPost]
        public string DeleteAddress(int Id)
        {
            UserAddress userAddress = db.UserAddresses.Where(x => x.AddressId == Id).FirstOrDefault();
            db.UserAddresses.Remove(userAddress);
            db.SaveChanges();
            return "true";
        }

        public IActionResult SPDashboard()
        {
            return View();
        }

        public IActionResult SPNewRequest()
        {
            return View();
        }
        public IActionResult SPUpcomingServices()
        {
            return View();
        }
        public IActionResult SPServiceHistory()
        {
            return View();
        }
        public IActionResult SPRatings()
        {
            return View();
        }
        public IActionResult BlockCustomer()
        {
            return View();
        }

        public IActionResult ServiceProviderSettings()
        {
            var a = (int)HttpContext.Session.GetInt32("User_id");
            User u = db.Users.Where(x => x.UserId == a).FirstOrDefault();
            return View(u);
        }

        public IActionResult spAddress()
        {
            var a = (int)HttpContext.Session.GetInt32("User_id");
            UserAddress u = db.UserAddresses.Where(x => x.AddressId == a).FirstOrDefault();
            return View(u);
        }

        [HttpPost]
        public string spAddress([FromBody] UserAddress add)
        {
            var a = (int)HttpContext.Session.GetInt32("User_id");
            UserAddress ua = db.UserAddresses.Where(x => x.UserId == a).FirstOrDefault();
            ua.AddressLine1 = add.AddressLine1;
            ua.AddressLine2 = add.AddressLine2;
            ua.PostalCode = add.PostalCode;
            ua.City = add.City;
            ua.Mobile = add.Mobile;
            db.UserAddresses.Update(ua);
            db.SaveChanges();
            return "true";
        }

        //public IActionResult spAddress()
        //{
        //    var a = (int)HttpContext.Session.GetInt32("User_id");
        //    UserAddress u = db.UserAddresses.Where(x => x.AddressId == a).FirstOrDefault();
        //    return View(u);
        //}

        //[HttpPost]
        //public string spAddress([FromBody] UserAddress add)
        //{
        //    var a = (int)HttpContext.Session.GetInt32("User_id");
        //    UserAddress ua = db.UserAddresses.Where(x => x.UserId == a).FirstOrDefault();
        //    ua.AddressLine1 = add.AddressLine1;
        //    ua.AddressLine2 = add.AddressLine2;
        //    ua.PostalCode = add.PostalCode;
        //    ua.City = add.City;
        //    ua.Mobile = add.Mobile;
        //    db.UserAddresses.Update(ua);
        //    db.SaveChanges();
        //    return "true";
        //}


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
