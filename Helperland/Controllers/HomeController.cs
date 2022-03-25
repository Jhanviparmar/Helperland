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
                    HttpContext.Session.SetString("username", login_user.FirstName + " " + login_user.LastName);
                    return RedirectToAction("sdashboard", "Home");
                }
                else if (login_user.UserTypeId == 2 && login_user.IsActive == true)
                {
                    HttpContext.Session.SetInt32("User_id", login_user.UserId);
                    HttpContext.Session.SetInt32("Usertype_id", login_user.UserTypeId);
                    HttpContext.Session.SetString("username", login_user.FirstName + " " + login_user.LastName);
                    return RedirectToAction("SPDashboard", "Home");   
                }
                else if (login_user.UserTypeId == 3)
                {
                    HttpContext.Session.SetInt32("User_id", login_user.UserId);
                    HttpContext.Session.SetInt32("Usertype_id", login_user.UserTypeId);
                    HttpContext.Session.SetString("username", login_user.FirstName + " " + login_user.LastName);
                    return RedirectToAction("ServiceRequests", "Home");
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
                user.IsActive = false;
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
        public string BookServiceRequest([FromBody] ServiceRequest book)
        {
            int userID = (int)HttpContext.Session.GetInt32("User_id");
            book.UserId = userID;
            book.ServiceId = 1000;
            book.PaymentDue = true;
            book.CreatedDate = DateTime.Now;
            book.ModifiedDate = DateTime.Now;
            book.ModifiedBy = userID;
            book.Distance = 10;
            book.Status = 0;                

            db.ServiceRequests.Add(book);
            db.SaveChanges();

            book.ServiceId = 1000 + book.ServiceRequestId;
            db.ServiceRequests.Update(book);
            db.SaveChanges();

            UserAddress address = db.UserAddresses.Where(x => x.AddressId == book.AddressId).SingleOrDefault();
            ServiceRequestAddress requestAddress = new ServiceRequestAddress();
            requestAddress.ServiceRequestId = book.ServiceRequestId;
            requestAddress.AddressLine1 = address.AddressLine1;
            requestAddress.AddressLine2 = address.AddressLine2;
            requestAddress.City = address.City;
            requestAddress.State = address.State;
            requestAddress.PostalCode = address.PostalCode;
            requestAddress.Email = address.Email;
            requestAddress.Mobile = address.Mobile;

            db.ServiceRequestAddresses.Add(requestAddress);
            db.SaveChanges();

            ModelState.Clear();
            return book.ServiceId.ToString();
        }

        public IActionResult sdashboard()
        {
            int userid = (int)HttpContext.Session.GetInt32("User_id");
            List<ServiceRequest> obj = new List<ServiceRequest>();
            obj = db.ServiceRequests.Where(x => x.UserId == userid && x.Status == 0).ToList();
            return View(obj);
        }

        public IActionResult srModel(int id)
        {
            var query = (from sr in db.ServiceRequests
                         join sra in db.ServiceRequestAddresses on sr.ServiceRequestId equals sra.ServiceRequestId
                         where sr.ServiceRequestId == id
                         select new CustomModel
                         {
                             ServiceRequestId = sr.ServiceRequestId,
                             ServiceId = sr.ServiceId,
                             ServiceStartDate = sr.ServiceStartDate,
                             ServiceHours = sr.ServiceHours,
                             Comments = sr.Comments,
                             HasPets = sr.HasPets,
                             TotalCost = sr.TotalCost,

                             AddressLine1 = sra.AddressLine1,
                             AddressLine2 = sra.AddressLine2,
                             City = sra.City,
                             State = sra.State,
                             PostalCode = sra.PostalCode,
                             Mobile = sra.Mobile

                         }).SingleOrDefault();

            return View(query);
        }
    

        public IActionResult shistory()
        {
            int userid = (int)HttpContext.Session.GetInt32("User_id");
            List<ServiceRequest> obj = db.ServiceRequests.Where(x => x.UserId == userid && x.Status == 1 || x.Status == 2).ToList();
            return View(obj);
        }

        public bool CancelRequest([FromBody] ServiceRequest sr)
        {
            ServiceRequest obj = db.ServiceRequests.Where(x => x.ServiceRequestId == sr.ServiceRequestId).FirstOrDefault();
            obj.Status = 2;
            obj.Comments = sr.Comments;
            db.ServiceRequests.Update(obj);
            db.SaveChanges();
            return true;
        }

        public bool RescheduleRequest([FromBody] ServiceRequest sr)
        {
            ServiceRequest obj = db.ServiceRequests.Where(x => x.ServiceRequestId == sr.ServiceRequestId).FirstOrDefault();
            obj.ServiceStartDate = sr.ServiceStartDate;
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

        public IActionResult EditAddress(int id, bool isAjax = false)
        {
            UserAddress address = db.UserAddresses.Where(x => x.AddressId == id).FirstOrDefault();
            if (isAjax)
            {

                return View(address);
            }
            return View(address);

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
            var a = (int)HttpContext.Session.GetInt32("User_id");
            UserAddress ua = db.UserAddresses.Where(x => x.UserId == a).FirstOrDefault();
            var query = (from sr in db.ServiceRequests
                         join sra in db.ServiceRequestAddresses
                         on sr.ServiceRequestId equals sra.ServiceRequestId
                         join u in db.UserAddresses on sr.UserId equals u.UserId
                         join fav in db.FavoriteAndBlockeds on u.UserId equals fav.TargetUserId
                         where ua.PostalCode == sra.PostalCode && sr.Status == 0 && fav.IsBlocked == false 
                        
                         select new CustomModel
                         {
                             ServiceRequestId = sr.ServiceRequestId,
                             ServiceId = sr.ServiceId,
                             ServiceStartDate = sr.ServiceStartDate,
                             ServiceHours = sr.ServiceHours,
                             Comments = sr.Comments,
                             HasPets = sr.HasPets ,
                             SubTotal = sr.SubTotal,

                             AddressLine1 = sra.AddressLine1,
                             AddressLine2 = sra.AddressLine2,
                             City = sra.City,
                             State = sra.State,
                             PostalCode = sra.PostalCode,
                             Mobile = sra.Mobile

                         }); 
            return View(query);
        }

        public IActionResult NewRequestModal(int id)
        {
            var a = (int)HttpContext.Session.GetInt32("User_id");
            User user = db.Users.Where(x => x.UserId == a).FirstOrDefault();

            var query = (from sr in db.ServiceRequests
                         join sra in db.ServiceRequestAddresses on sr.ServiceRequestId equals sra.ServiceRequestId
                         where sr.ServiceRequestId == id
                         join u in db.Users on sr.UserId equals u.UserId
                         select new CustomModel
                         {
                             ServiceId = sr.ServiceId,
                             ServiceRequestId = sr.ServiceRequestId,
                             ServiceStartDate = sr.ServiceStartDate,
                             ServiceHours = sr.ServiceHours,
                             SubTotal = sr.SubTotal,

                             AddressLine1 = sra.AddressLine1,
                             AddressLine2 = sra.AddressLine2,
                             PostalCode = sra.PostalCode,
                             City = sra.City,

                             FirstName = u.FirstName,
                             LastName = u.LastName

                         }).SingleOrDefault();

            return View(query);
        }


        //0:new request  1:completed  2:cancelled 4:accepted 
        public string acceptedService(int id)
        {
            var a = (int)HttpContext.Session.GetInt32("User_id");
            User user = db.Users.Where(x => x.UserId == a).FirstOrDefault();

            ServiceRequest sr = db.ServiceRequests.Where(x => x.ServiceRequestId == id).FirstOrDefault();
            sr.ServiceProviderId = a;
            sr.Status = 4;
            sr.SpacceptedDate = DateTime.Now;
            db.ServiceRequests.Update(sr);
            db.SaveChanges();
            return "true";
        }

        public IActionResult SPUpcomingServices()
        {
            var a = (int)HttpContext.Session.GetInt32("User_id");
            User user = db.Users.Where(x => x.UserId == a).FirstOrDefault();

            var query = (from sr in db.ServiceRequests
                         join sra in db.ServiceRequestAddresses
                         on sr.ServiceRequestId equals sra.ServiceRequestId
                         join u in db.Users on sr.UserId equals u.UserId
                         where sr.ServiceProviderId == a && sr.Status == 4
                         select new CustomModel
                         {
                             ServiceId = sr.ServiceId,
                             ServiceRequestId = sr.ServiceRequestId,
                             ServiceStartDate = sr.ServiceStartDate,
                             SubTotal = sr.SubTotal,
                             ServiceHours = sr.ServiceHours,
                             Comments = sr.Comments,
                             HasPets = sr.HasPets,
                             TotalCost = sr.TotalCost,

                             AddressLine1 = sra.AddressLine1,
                             AddressLine2 = sra.AddressLine2,
                             PostalCode = sra.PostalCode,
                             City = sra.City,

                             FirstName = u.FirstName,
                             LastName = u.LastName

                         }).ToList();

            return View(query);
        }

        public IActionResult UpcomingServiceModal(int id)
        {
            var a = (int)HttpContext.Session.GetInt32("User_id");
            User user = db.Users.Where(x => x.UserId == a).FirstOrDefault();
            
            var query = (from sr in db.ServiceRequests
                         join sra in db.ServiceRequestAddresses on sr.ServiceRequestId equals sra.ServiceRequestId
                         where sr.ServiceRequestId == id
                         join u in db.Users on sr.UserId equals u.UserId
                         select new CustomModel
                         {
                             ServiceId = sr.ServiceId,
                             ServiceRequestId = sr.ServiceRequestId,
                             ServiceStartDate = sr.ServiceStartDate,
                             SubTotal = sr.SubTotal,
                             ServiceHours = sr.ServiceHours,

                             AddressLine1 = sra.AddressLine1,
                             AddressLine2 = sra.AddressLine2,
                             PostalCode = sra.PostalCode,
                             City = sra.City,

                             FirstName = u.FirstName,
                             LastName = u.LastName

                         }).SingleOrDefault();

            return View(query);
        }
        

        public string CancelServiceRequest(int id)
        {
            var a = (int)HttpContext.Session.GetInt32("User_id");
            User user = db.Users.Where(x => x.UserId == a).FirstOrDefault();

            ServiceRequest sr = db.ServiceRequests.Where(x => x.ServiceRequestId == id).FirstOrDefault();
            sr.ServiceProviderId = a;
            sr.Status = 2;
            sr.SpacceptedDate = DateTime.Now;
            db.ServiceRequests.Update(sr);
            db.SaveChanges();
            return "true";
        }

        public string CompletedService(int id)
        {
            ServiceRequest sr = db.ServiceRequests.Where(x => x.ServiceRequestId == id).FirstOrDefault();
            sr.Status = 1;
            db.ServiceRequests.Update(sr);
            db.SaveChanges();

            var aa = db.FavoriteAndBlockeds.Where(x => x.UserId == sr.ServiceProviderId && x.TargetUserId == sr.UserId).FirstOrDefault();

            if (aa == null)
            {
                FavoriteAndBlocked fav = new FavoriteAndBlocked();
                fav.UserId = (int)sr.ServiceProviderId;
                fav.TargetUserId = sr.UserId;
                fav.IsFavorite = false;
                fav.IsBlocked = false;
                db.FavoriteAndBlockeds.Add(fav);
                db.SaveChanges();
            }
            return "true";
        }
        public IActionResult SPServiceHistory()
        {
            var a = (int)HttpContext.Session.GetInt32("User_id");
            
            var query = (from sr in db.ServiceRequests
                         join sra in db.ServiceRequestAddresses
                         on sr.ServiceRequestId equals sra.ServiceRequestId
                         join u in db.Users on sr.UserId equals u.UserId
                         where sr.ServiceProviderId == a && sr.Status == 1
                         select new CustomModel
                         {
                             ServiceId = sr.ServiceId,
                             ServiceRequestId = sr.ServiceRequestId,
                             ServiceStartDate = sr.ServiceStartDate,
                             SubTotal = sr.SubTotal,

                             AddressLine1 = sra.AddressLine1,
                             AddressLine2 = sra.AddressLine2,
                             PostalCode = sra.PostalCode,
                             City = sra.City,

                             FirstName = u.FirstName,
                             LastName = u.LastName
                         }).ToList();

            return View(query);
        }
        public IActionResult SPRatings()
        {
            return View();
        }
        public IActionResult BlockCustomer()
        {
            int id = (int)HttpContext.Session.GetInt32("User_id");

            var query = from user in db.Users
                        join fav in db.FavoriteAndBlockeds on user.UserId equals fav.TargetUserId
                        where fav.UserId == id
                        select new CustomModel
                        {
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            IsBlocked = fav.IsBlocked,
                            Id = fav.Id
                        };
            return View(query);
        }
        
        public IActionResult BlockCustomer1(int id)
        {
            FavoriteAndBlocked fav = db.FavoriteAndBlockeds.Where(x => x.Id == id).FirstOrDefault();
            fav.IsBlocked = true;
            db.FavoriteAndBlockeds.Update(fav);
            db.SaveChanges();
            return RedirectToAction("BlockCustomer");
        }

        public IActionResult UnblockCustomer(int id)
        {
            FavoriteAndBlocked fav = db.FavoriteAndBlockeds.Where(x => x.Id == id).FirstOrDefault();
            fav.IsBlocked = false;
            db.FavoriteAndBlockeds.Update(fav);
            db.SaveChanges();
            return RedirectToAction("BlockCustomer");
        }


        public IActionResult ServiceProviderSettings()
        {
            int a = (int)HttpContext.Session.GetInt32("User_id");

            SPSettings spsettings = new SPSettings();
            spsettings.Users = db.Users.Where(x => x.UserId == a).FirstOrDefault();
            spsettings.UserAddresses = db.UserAddresses.Where(x => x.UserId == a).FirstOrDefault();
            return View(spsettings);
        }

        [HttpPost]

        public IActionResult ServiceProviderSettings(SPSettings sp)
        {
            int userid = (int)HttpContext.Session.GetInt32("User_id");
            User u = db.Users.Where(x => x.UserId == userid).FirstOrDefault();
            u.LastName = sp.Users.LastName;
            u.FirstName = sp.Users.FirstName;
            u.Mobile = sp.Users.Mobile;
            db.Users.Update(u);
            db.SaveChanges();

            UserAddress ua = db.UserAddresses.Where(x => x.UserId == userid).FirstOrDefault();
            if (ua != null)
            {
                ua.AddressLine1 = sp.UserAddresses.AddressLine1;
                ua.AddressLine2 = sp.UserAddresses.AddressLine2;
                ua.PostalCode = sp.UserAddresses.PostalCode;
                ua.City = sp.UserAddresses.City;
                ua.Mobile = sp.Users.Mobile;
                db.UserAddresses.Update(ua);
                db.SaveChanges();
            }
            else
            {
                UserAddress address = new UserAddress();
                address.UserId = userid;
                address.Mobile = sp.UserAddresses.Mobile;
                address.City = sp.UserAddresses.City;
                address.AddressLine1 = sp.UserAddresses.AddressLine1;
                address.AddressLine2 = sp.UserAddresses.AddressLine2;
                address.PostalCode = sp.UserAddresses.PostalCode;
                db.UserAddresses.Add(address);
                db.SaveChanges();
            }
            HttpContext.Session.SetString("username", u.FirstName);
            return View();
        }

        public IActionResult ServiceRequests()
        {
            
            var query = (from sr in db.ServiceRequests
                         join sra in db.ServiceRequestAddresses
                         on sr.ServiceRequestId equals sra.ServiceRequestId
                         join u in db.Users on sr.UserId equals u.UserId
                         select new CustomModel
                         {
                             ServiceId = sr.ServiceId,
                             
                             ServiceStartDate = sr.ServiceStartDate,
                             ServiceRequestId = sr.ServiceRequestId,
                             Status = sr.Status,

                             AddressLine1 = sra.AddressLine1,
                             AddressLine2 = sra.AddressLine2,
                             PostalCode = sra.PostalCode,
                             City = sra.City,

                             FirstName = u.FirstName,
                             LastName = u.LastName

                         }).ToList(); 
            return View(query);
        }

        public IActionResult EditReSchedule(int id)
        {
            var query = (from sr in db.ServiceRequests
                         join sra in db.ServiceRequestAddresses
                         on sr.ServiceRequestId equals sra.ServiceRequestId
                         where sr.ServiceRequestId == id

                         select new CustomModel
                         {
                             ServiceId = sr.ServiceId,
                             ServiceRequestId = sr.ServiceRequestId,
                             ServiceStartDate = sr.ServiceStartDate,

                             AddressLine1 = sra.AddressLine1,
                             AddressLine2 = sra.AddressLine2,
                             PostalCode = sra.PostalCode,
                             City = sra.City,

                         }).FirstOrDefault();
            return View(query);

        }

        [HttpPost]
        public string EditReschedule([FromBody] CustomModel add)
        {
            ServiceRequest sr = db.ServiceRequests.Where(x => x.ServiceRequestId == add.ServiceRequestId).FirstOrDefault();
            sr.ServiceStartDate = add.ServiceStartDate;
            sr.Comments = add.Comments;
            db.ServiceRequests.Update(sr);
            db.SaveChanges();

            ServiceRequestAddress sra = db.ServiceRequestAddresses.Where(x => x.ServiceRequestId == add.ServiceRequestId).FirstOrDefault();
            sra.AddressLine1 = add.AddressLine1;
            sra.AddressLine2 = add.AddressLine2;
            sra.PostalCode = add.PostalCode;
            sra.City = add.City;
            db.ServiceRequestAddresses.Update(sra);
            db.SaveChanges();

            return "true";
        }

        //[HttpPost]
        //public IActionResult servicesearch(string searchby)
        //{ 
        //    var users = db.Users.ToList();
        //    if (searchby != null)
        //    {
        //        users = db.Users.Where(x => x.FirstName.Contains(searchby)).ToList();
        //    }
        //    return View(users);
        // }


        public IActionResult UserManagement()
        {
            User user = new User();
            UserAddress useradd = new UserAddress();
            var query = (from u in db.Users
                         join ua in db.UserAddresses
                         on u.UserId equals ua.UserId

                         select new CustomUser
                         {
                             UserId = u.UserId,
                             FirstName = u.FirstName,
                             LastName = u.LastName,
                             UserTypeId = u.UserTypeId,
                             IsActive = u.IsActive,
                             

                             AddressLine1 = ua.AddressLine1,
                             AddressLine2 = ua.AddressLine2,
                             PostalCode = ua.PostalCode,
                             City = ua.City,

                         }).ToList();
            return View(query);
        }
        
        public IActionResult Activate(int id)
        {
            User u = db.Users.Where(x => x.UserId == id).FirstOrDefault();
            u.IsActive = true;
            db.Users.Update(u);
            db.SaveChanges();
            return RedirectToAction("UserManagement");
        }

        public IActionResult Deactivate(int id)
        {
            User u = db.Users.Where(x => x.UserId == id).FirstOrDefault();
            u.IsActive = false;
            db.Users.Update(u);
            db.SaveChanges();
            return RedirectToAction("UserManagement");
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
