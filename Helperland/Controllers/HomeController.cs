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
            if (ModelState.IsValid)
            {
                ContactU cu = new ContactU();
                cu.Name = contactu.FirstName + " " + contactu.LastName;
                cu.Email = contactu.Email;
                cu.Message = contactu.Message;
                cu.PhoneNumber = contactu.PhoneNumber;
                cu.Subject = contactu.Subject;
                cu.CreatedOn = DateTime.Now;
                cu.CreatedBy = contactu.ContactUsId;
                db.ContactUs.Add(cu);
                db.SaveChanges();
                TempData["Msg"] = "Your Response has been recorded";
                return RedirectToAction("Contact");
            }
            else
            {
                TempData["error"] = "Problem occured while recording your response!";
                return RedirectToAction("Contact");
            }
        }

        public IActionResult About()
        {
            return View();
        }
        
        //-------------------------Authentication------------------------------------
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]  // 1:Customer 2:ServiceProvider 3:Admin
        public IActionResult Login(User u)
        {

            var login_user = db.Users.Where(x => x.Email == u.Email && x.Password == u.Password).FirstOrDefault();
            if(login_user != null)
            {
                if(login_user.IsActive == true)
                {
                    HttpContext.Session.SetInt32("User_id", login_user.UserId);
                    HttpContext.Session.SetInt32("Usertype_id", login_user.UserTypeId);
                    HttpContext.Session.SetString("username", login_user.FirstName + " " + login_user.LastName);
                    int UserType = login_user.UserTypeId;

                    switch (UserType)
                    {
                        case 1:
                            return RedirectToAction("sdashboard");

                        case 2:
                            return RedirectToAction("SPDashboard");

                        case 3:
                            return RedirectToAction("Servicerequests");

                        default:
                            TempData["alert"] = "No such account found";
                            return RedirectToAction("Index");

                    }
                }
                else
                {
                    TempData["alert"] = "Access Denied!";
                    TempData["msg"] = "You don't have access to this account. Contact Admin for further queries.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["alert"] = "Please try logging in again!";
                TempData["msg"] = "*Email or password is invalid! Please try again.";
                return RedirectToAction("Index");
            }

        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("User_id");
            HttpContext.Session.Remove("Usertype_id");
            HttpContext.Session.Remove("username");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ForgetPassword(User user)
        {
            //string resetCode = Guid.NewGuid().ToString();
            //var link = "<a href='" + Url.Action("ResetPassword", "Home", null, "http") + "'>Reset Password</a>";
            

                var getUser = db.Users.FirstOrDefault(x => x.Email.Equals(user.Email));

                if (getUser != null)
                {
                    string BaseUrl = string.Format("{0}://{1}", HttpContext.Request.Scheme, HttpContext.Request.Host);
                var link = $"{BaseUrl}/Home/ResetPassword?id=" + getUser.UserId;

                var senderemail = new MailAddress("helperlandservices@gmail.com", "Reset your password");
                    var receiveremail = new MailAddress(user.Email);

                    var password = "2022#helperland";
                    var subject = "Password Reset Request";
                    var body = "Hi " + getUser.FirstName + ", You recently requested to reset your password for your account. Click the link to reset it. " 
                                     + link + 
                                     " If you did not request a password reset, please ignore this email or reply to let us know.Thank you";
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
                TempData["alert"] = "Email has been sent!";
                TempData["message1"] = "Email has been sent!";
                return RedirectToAction("Index", "Home");

                }
                else
                {
                    TempData["alert"] = "Email does not exists in records!";
                    TempData["message"] = "Email doesn't exists! Provide valid email id.";
                    return RedirectToAction("Index");

            }
        }

        public IActionResult ResetPassword(int id)
        {
            User user = db.Users.Where(x => x.UserId == id).FirstOrDefault();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(User u)
        {
            if (u.Password.Length >= 8)
            {
                if (u.Password == u.ConfirmPassword)
                {
                    User user = db.Users.Where(x => x.UserId == u.UserId).FirstOrDefault();
                    user.Password = u.Password;
                    user.ModifiedBy = u.UserId;
                    user.ModifiedDate = DateTime.Now;
                    db.Users.Update(user);
                    db.SaveChanges();
                    TempData["success"] = "Password has been changed successfully";
                    return RedirectToAction("ResetPassword");
                }
                
                return RedirectToAction("ResetPassword"); ;
            }
            TempData["error"] = "Password must contain 8 characters.";
            return RedirectToAction("ResetPassword");
        }
        
        //--------------------------------------Book Service-------------------------------------
        
        public IActionResult BookService()
        {
            var UserID = HttpContext.Session.GetInt32("User_id");
            var user = db.Users.Where(x => x.UserId == UserID).FirstOrDefault();

            if (UserID != null)
            {
                if(user.UserTypeId == 1)
                {
                    return View();
                }
                else
                {
                    TempData["alert"] = "This account does not contain customer access.";
                    return RedirectToAction("Index");
                }
                
            }
            TempData["alert"] = "You have to login first before booking service!!";
            return RedirectToAction("Index");
            
        }

        public string PostalCode(string postalcode)
        {
            if (postalcode == null)
            {
                return "false";
            }
            var PostalCode = db.Users.Any(x => x.ZipCode == postalcode);
            string IsValid;
            if (PostalCode)
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
            book.ServiceStartDate = book.ServiceStartDate;
            book.ServiceHours = 3 + book.ServiceHours;
            
            book.CreatedDate = DateTime.Now;
            //book.ModifiedDate = DateTime.Now;
            //book.ModifiedBy = userID;
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
            book.ZipCode = address.PostalCode;
            requestAddress.Email = address.Email;
            requestAddress.Mobile = address.Mobile;

            db.ServiceRequestAddresses.Add(requestAddress);
            db.SaveChanges();

            ServiceRequestExtra extra = new ServiceRequestExtra();
            extra.ServiceRequestId = book.ServiceRequestId;
            extra.ServiceExtraId = book.extraId;
            db.ServiceRequestExtras.Add(extra);
            db.SaveChanges();


            var query = from u in db.Users
                        where u.UserTypeId == 2
                        select u.Email;
            
            var senderemail = new MailAddress("helperlandservices@gmail.com", "New Service Request Alert");
            foreach (var i in query)
            {
                var receiver = new MailAddress(i);
            }
            
            var password = "2022#helperland";
            var subject = "Password Reset Request";
            var body = "Hello Service Provider,\n\nNew Service Request is Available " +
                "in your area\n \n Visit Helperland for further Information.  " +
                "\n\nRegards,\nHelperland Team";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderemail.Address, password)
            };
            using (var message = new MailMessage(senderemail, receiver)
            {
                Subject = subject,
                Body = body

            })
            {
                smtp.Send(message);
            }


            ModelState.Clear();
            return book.ServiceId.ToString();
        }
        
        //----------------------------------Customer Pages ---------------------------------
        
        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateAccount(User user)
        {
            if (ModelState.IsValid)
            {
                var isEmailAlreadyExists = db.Users.Any(x => x.Email == user.Email);
                if (isEmailAlreadyExists)
                {

                    TempData["alert"] = "This email already exists! Try with another email id.";
                    return View(user);
                }
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
                return View(user);
            }
            return View();
        }
        
        //Customer Dashboard :
        public IActionResult sdashboard()
        {
            int userid = (int)HttpContext.Session.GetInt32("User_id");
            User u = db.Users.Where(x => x.UserId == userid).FirstOrDefault();
            if (u != null)
            {
                if(u.UserTypeId == 1)
                {
                    var query = (from sr in db.ServiceRequests
                                 join user in db.Users on sr.UserId equals user.UserId
                                 join rate in db.Ratings on sr.ServiceRequestId equals rate.ServiceRequestId
                                 into Rating
                                 from rate in Rating.DefaultIfEmpty()
                                 where sr.UserId == userid && sr.Status == 0 || sr.Status == 4
                                 select new CustomRatings
                                 {
                                     ServiceRequestId = sr.ServiceRequestId,
                                     ServiceStartDate = sr.ServiceStartDate,
                                     TotalCost = sr.TotalCost,
                                     Status = sr.Status,
                                     ServiceId = sr.ServiceId,

                                     Ratings = rate == null ? 0 : rate.Ratings,

                                     FirstName = user.FirstName,
                                     LastName = user.LastName,
                                     UserProfilePicture = user.UserProfilePicture

                                 }).ToList();

                    return View(query);
                }
                TempData["alret"] = "Access Denied ! Credentials doesn't match Customer records.";
                return RedirectToAction("Index");
            }
            TempData["alret"] = "You have to login first before accessing this page.";
            return RedirectToAction("Index");

        }

        public IActionResult srModel(int id)
        {
           
            var query = (from sr in db.ServiceRequests
                         join sra in db.ServiceRequestAddresses on sr.ServiceRequestId equals sra.ServiceRequestId
                         join u in db.Users on sr.UserId equals u.UserId
                         join sre in db.ServiceRequestExtras on sr.ServiceRequestId equals sre.ServiceRequestId
                         into Sre
                         from sre in Sre.DefaultIfEmpty()
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
                             Mobile = sra.Mobile,

                             ServiceExtraId = sre.ServiceExtraId,

                             Email = u.Email

                         }).SingleOrDefault();

            return View(query);
        }
        
        //Customer History :
        public IActionResult shistory()
        {
            int userid = (int)HttpContext.Session.GetInt32("User_id");
            User u = db.Users.Where(x => x.UserId == userid).FirstOrDefault();
            if( u != null)
            {
                if(u.UserTypeId == 1)
                {
                    var query = (from sr in db.ServiceRequests
                              join user in db.Users
                              on sr.ServiceProviderId equals user.UserId into A
                              from User in A.DefaultIfEmpty()
                              join rate in db.Ratings on sr.ServiceRequestId equals rate.ServiceRequestId
                              into Rating
                              from rate in Rating.DefaultIfEmpty()
                              where sr.UserId == userid && sr.Status != 0
                              select new CustomRatings
                              {
                                  FirstName = User == null ? " " : User.FirstName,
                                  LastName = User == null ? " " : User.LastName,
                                  Ratings = Rating == null ? 0 : rate.Ratings,
                                  ServiceStartDate = sr.ServiceStartDate,
                                  TotalCost = sr.TotalCost,
                                  Status = sr.Status,
                                  ServiceId = sr.ServiceId,
                                  ServiceRequestId = sr.ServiceRequestId,
                                  ServiceProviderId = sr.ServiceProviderId,

                              }).ToList();
                    return View(query);
                }
                TempData["alret"] = "Access Denied ! Credentials doesn't match Customer records.";
                return RedirectToAction("Index");

            }
            TempData["alret"] = "You have to login first before accessing this page.";
            return RedirectToAction("Index");

        }

        public IActionResult shistoryModal(int id)
        {
            var query = (from sr in db.ServiceRequests
                         join sra in db.ServiceRequestAddresses on sr.ServiceRequestId equals sra.ServiceRequestId
                         join sre in db.ServiceRequestExtras on sr.ServiceRequestId equals sre.ServiceRequestId
                         into Sre
                         from sre in Sre.DefaultIfEmpty()
                         join u in db.Users on sr.UserId equals u.UserId
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
                             Mobile = sra.Mobile,

                             ServiceExtraId = sre.ServiceExtraId,

                             Email = u.Email

                         }).SingleOrDefault();

            return View(query);
        }
        public bool CancelRequest([FromBody] ServiceRequest sr)
        {
            ServiceRequest obj = db.ServiceRequests.Where(x => x.ServiceRequestId == sr.ServiceRequestId).FirstOrDefault();
            obj.Status = 2;
            obj.Comments = sr.Comments;
            obj.ModifiedBy = sr.UserId;
            obj.ModifiedDate = sr.ModifiedDate;
            db.ServiceRequests.Update(obj);
            db.SaveChanges();
            return true;
        }

        public bool RescheduleRequest([FromBody] ServiceRequest sr)
        {
            ServiceRequest obj = db.ServiceRequests.Where(x => x.ServiceRequestId == sr.ServiceRequestId).FirstOrDefault();
            obj.ServiceStartDate = sr.ServiceStartDate;
            obj.ModifiedBy = sr.UserId;
            obj.ModifiedDate = sr.ModifiedDate;
            db.ServiceRequests.Update(obj);
            db.SaveChanges();
            return true;
        }

        public IActionResult RateSPModal(int spid)
        {
            User u = db.Users.Where(x => x.UserId == spid).FirstOrDefault();
            return View(u);
        }
        public string RatingSP([FromBody] Rating rate)
        {
            int userid = (int)HttpContext.Session.GetInt32("User_id");
            Rating rt = db.Ratings.Where(x => x.ServiceRequestId == rate.ServiceRequestId).FirstOrDefault();

            if (rt != null)
            {
                rt.Friendly = rate.Friendly;
                rt.OnTimeArrival = rate.OnTimeArrival;
                rt.QualityOfService = rate.QualityOfService;
                rt.Ratings = rate.Ratings;
                rt.RatingDate = DateTime.Now;               
                rt.Comments = rate.Comments;
                db.Ratings.Update(rt);
                db.SaveChanges();
            }
            else
            {
                rate.RatingFrom = userid;
                rate.RatingDate = DateTime.Now;
                db.Ratings.Add(rate);
                db.SaveChanges();
            }
            return "true";
        }

        public IActionResult favouritepros()
        {
            int userid = (int)HttpContext.Session.GetInt32("User_id");
            User u = db.Users.Where(x => x.UserId == userid).FirstOrDefault();
            if (u != null)
            {
                if (u.UserTypeId == 1)
                {
                    return View();
                }
                TempData["alret"] = "Access Denied ! Credentials doesn't match Customer records.";
                return RedirectToAction("Index");

            }
            TempData["alret"] = "You have to login first before accessing this page.";
            return RedirectToAction("Index");
        }

        public IActionResult settings()
        {
            int userid = (int)HttpContext.Session.GetInt32("User_id");
            User u = db.Users.Where(x => x.UserId == userid).FirstOrDefault();
            if (u != null)
            {
                if (u.UserTypeId == 1)
                {
                    return View();
                }
                TempData["alret"] = "Access Denied ! Credentials doesn't match Customer records.";
                return RedirectToAction("Index");

            }
            TempData["alret"] = "You have to login first before accessing this page.";
            return RedirectToAction("Index");
     
        }

        public string UpdateDetails([FromBody] User book)
        {
            var a = (int)HttpContext.Session.GetInt32("User_id");
            User u = db.Users.Where(x => x.UserId == a).FirstOrDefault();
            u.FirstName = book.FirstName;
            u.LastName = book.LastName;
            u.Mobile = book.Mobile;
            u.DateOfBirth = book.DateOfBirth;
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
                    u.ModifiedBy = a;
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
        
        
        //-----------------------------------------------Service Provider Pages----------------------------
        
        public IActionResult serviceproviderSignup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult serviceproviderSignup(User user)
        {
            if (ModelState.IsValid)
            {
                var isEmailAlreadyExists = db.Users.Any(x => x.Email == user.Email);
                if (isEmailAlreadyExists)
                {
                    TempData["alert"] = "This email is already exists! Try with another email id.";
                    return View(user);
                }
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
        
        public IActionResult SPDashboard()
        {
            return View();
        }

        public IActionResult SPNewRequest()
        {
            int userid = (int)HttpContext.Session.GetInt32("User_id");
            UserAddress ua = db.UserAddresses.Where(x => x.UserId == userid).FirstOrDefault();
            User u = db.Users.Where(x => x.UserId == userid).FirstOrDefault();
            if (u != null)
            {
                if (u.UserTypeId == 2)
                {
                    var query = (from sr in db.ServiceRequests
                                 join sra in db.ServiceRequestAddresses
                                 on sr.ServiceRequestId equals sra.ServiceRequestId
                                 join user in db.Users on sr.UserId equals user.UserId
                                 join fb in db.FavoriteAndBlockeds on sr.UserId equals fb.TargetUserId
                                 into fav
                                 from fb in fav.DefaultIfEmpty()
                                 where ua.PostalCode == sra.PostalCode && sr.Status == 0 && sr.ServiceProviderId == null
                                 && (fb.IsBlocked == false || fb.IsBlocked == null)
                                 && (fb.UserId == null || fb.UserId == userid)
                                 select new CustomModel


                                 {
                                     ServiceRequestId = sr.ServiceRequestId,
                                     ServiceId = sr.ServiceId,
                                     ServiceStartDate = sr.ServiceStartDate,
                                     ServiceHours = sr.ServiceHours,
                                     Comments = sr.Comments,
                                     HasPets = sr.HasPets,
                                     SubTotal = sr.SubTotal,

                                     AddressLine1 = sra.AddressLine1,
                                     AddressLine2 = sra.AddressLine2,
                                     City = sra.City,
                                     State = sra.State,
                                     PostalCode = sra.PostalCode,
                                     Mobile = sra.Mobile,

                                     FirstName = user.FirstName,
                                     LastName = user.LastName

                                 });
                    return View(query);
                }
                TempData["alret"] = "Access Denied ! Credentials doesn't match Service Provider records.";
                return RedirectToAction("Index");

            }
            TempData["alret"] = "You have to login first before accessing this page.";
            return RedirectToAction("Index");
        }

        public IActionResult NewRequestModal(int id)
        {
            var a = (int)HttpContext.Session.GetInt32("User_id");
            User user = db.Users.Where(x => x.UserId == a).FirstOrDefault();

            var query = (from sr in db.ServiceRequests
                         join sra in db.ServiceRequestAddresses on sr.ServiceRequestId equals sra.ServiceRequestId
                         join u in db.Users on sr.UserId equals u.UserId
                         join sre in db.ServiceRequestExtras on sr.ServiceRequestId equals sre.ServiceRequestId
                         into Sre
                         from sre in Sre.DefaultIfEmpty()
                         where sr.ServiceRequestId == id
                         
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
                             Mobile = sra.Mobile,

                             ServiceExtraId = sre.ServiceExtraId,

                             FirstName = u.FirstName,
                             LastName = u.LastName,
                             Email = u.Email
                             

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
            int userid = (int)HttpContext.Session.GetInt32("User_id");
            User u = db.Users.Where(x => x.UserId == userid).FirstOrDefault();
            if (u != null)
            {
                if (u.UserTypeId == 2)
                {
                    var query = (from sr in db.ServiceRequests
                                 join sra in db.ServiceRequestAddresses
                                 on sr.ServiceRequestId equals sra.ServiceRequestId
                                 join user in db.Users on sr.UserId equals user.UserId
                                 where sr.ServiceProviderId == userid && sr.Status == 4
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

                                     FirstName = user.FirstName,
                                     LastName = user.LastName

                                 }).ToList();

                    return View(query);
                }
                TempData["alret"] = "Access Denied ! Credentials doesn't match Service Provider records.";
                return RedirectToAction("Index");

            }
            TempData["alret"] = "You have to login first before accessing this page.";
            return RedirectToAction("Index");
            var a = (int)HttpContext.Session.GetInt32("User_id");
            User user = db.Users.Where(x => x.UserId == a).FirstOrDefault();
        }

        public IActionResult UpcomingServiceModal(int id)
        {
            var a = (int)HttpContext.Session.GetInt32("User_id");
            User user = db.Users.Where(x => x.UserId == a).FirstOrDefault();
            
            var query = (from sr in db.ServiceRequests
                         join sra in db.ServiceRequestAddresses on sr.ServiceRequestId equals sra.ServiceRequestId
                         where sr.ServiceRequestId == id
                         join u in db.Users on sr.UserId equals u.UserId
                         join sre in db.ServiceRequestExtras on sr.ServiceRequestId equals sre.ServiceRequestId
                         into Sre
                         from sre in Sre.DefaultIfEmpty()

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
                             Mobile = sra.Mobile,

                             ServiceExtraId = sre.ServiceExtraId,

                             FirstName = u.FirstName,
                             LastName = u.LastName,
                             Email = u.Email

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

            var fb = db.FavoriteAndBlockeds.Where(x => x.UserId == sr.ServiceProviderId && x.TargetUserId == sr.UserId).FirstOrDefault();

            if (fb == null)
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
            int userid = (int)HttpContext.Session.GetInt32("User_id");
            User u = db.Users.Where(x => x.UserId == userid).FirstOrDefault();
            if (u != null)
            {
                if (u.UserTypeId == 2)
                {
                    var query = (from sr in db.ServiceRequests
                                 join sra in db.ServiceRequestAddresses
                                 on sr.ServiceRequestId equals sra.ServiceRequestId
                                 join user in db.Users on sr.UserId equals user.UserId
                                 where sr.ServiceProviderId == userid && sr.Status == 1
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

                                     FirstName = user.FirstName,
                                     LastName = user.LastName

                                 }).ToList();

                    return View(query);
                }
                TempData["alret"] = "Access Denied ! Credentials doesn't match Service Provider records.";
                return RedirectToAction("Index");

            }
            TempData["alret"] = "You have to login first before accessing this page.";
            return RedirectToAction("Index");

        }
        
        public IActionResult SPRatings()
        {
            int userid = (int)HttpContext.Session.GetInt32("User_id");
            User u = db.Users.Where(x => x.UserId == userid).FirstOrDefault();
            if (u != null)
            {
                if (u.UserTypeId == 2)
                {
                    var query = (from User in db.Users
                                 join Rating in db.Ratings
                                 on User.UserId equals Rating.RatingFrom
                                 join ServiceRequest in db.ServiceRequests
                                 on Rating.ServiceRequestId equals ServiceRequest.ServiceRequestId
                                 where Rating.RatingTo == userid
                                 select new CustomRatings
                                 {
                                     ServiceId = ServiceRequest.ServiceId,
                                     FirstName = User.FirstName,
                                     LastName = User.LastName,
                                     RatingDate = (DateTime)Rating.RatingDate,
                                     Ratings = Rating.Ratings,
                                     Comments = Rating.Comments
                                 }).ToList();

                    return View(query);
                }
                TempData["alret"] = "Access Denied ! Credentials doesn't match Service Provider records.";
                return RedirectToAction("Index");

            }
            TempData["alret"] = "You have to login first before accessing this page.";
            return RedirectToAction("Index");
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
            if (ModelState.IsValid)
            {
               
                    u.LastName = sp.Users.LastName;
                    u.FirstName = sp.Users.FirstName;
                    u.Mobile = sp.Users.Mobile;
                    u.Gender = sp.Users.Gender;
                    u.DateOfBirth = sp.Users.DateOfBirth;
                    u.UserProfilePicture = sp.Users.UserProfilePicture;
                    u.ZipCode = sp.UserAddresses.PostalCode;
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
                SPSettings spsettings = new SPSettings();
                spsettings.Users = db.Users.Where(x => x.UserId == userid).FirstOrDefault();
                spsettings.UserAddresses = db.UserAddresses.Where(x => x.UserId == userid).FirstOrDefault();
                return View(spsettings);
            }
            
            HttpContext.Session.SetString("username", u.FirstName);
            SPSettings spsetting = new SPSettings();
            spsetting.Users = db.Users.Where(x => x.UserId == userid).FirstOrDefault();
            spsetting.UserAddresses = db.UserAddresses.Where(x => x.UserId == userid).FirstOrDefault();
            return View(spsetting);
        }


        
        //-----------------------------Admin Pages----------------------------
          
        
        public IActionResult ServiceRequests()
        {
            int userid = (int)HttpContext.Session.GetInt32("User_id");
            User u = db.Users.Where(x => x.UserId == userid).FirstOrDefault();
            if (u != null)
            {
                if (u.UserTypeId == 3)
                {
                    var query = (from sr in db.ServiceRequests
                                 join sra in db.ServiceRequestAddresses on
                                 sr.ServiceRequestId equals sra.ServiceRequestId
                                 join user in db.Users on sr.UserId equals user.UserId
                                 join rating in db.Ratings on sr.ServiceRequestId equals rating.ServiceRequestId 
                                 into Rate
                                 from rating in Rate.DefaultIfEmpty()
                                 join sp in db.Users on sr.ServiceProviderId equals sp.UserId into SP
                                 from sp in SP.DefaultIfEmpty()
                                 select new CustomModel
                                 {

                                     ServiceRequestId = sr.ServiceRequestId,
                                     ServiceId = sr.ServiceId,
                                     ServiceStartDate = sr.ServiceStartDate,
                                     ServiceProviderId = sr.ServiceProviderId,
                                     Status = sr.Status,

                                     AddressLine1 = sra.AddressLine1,
                                     AddressLine2 = sra.AddressLine2,
                                     City = sra.City,
                                     Mobile = sra.Mobile,
                                     PostalCode = sra.PostalCode,

                                     FirstName = user.FirstName,
                                     LastName = user.LastName,
                                     UserProfilePicture = sp.UserProfilePicture,

                                     spFirstName = sp == null ? " " : sp.FirstName,
                                     spLastName = sp == null ? " " : sp.LastName,
                                     Ratings = rating == null ? 0 : rating.Ratings,

                                 }).ToList();
                    return View(query);
                }
                TempData["alret"] = "Access Denied ! Only admins can access this page";
                return RedirectToAction("Index");

            }
            TempData["alret"] = "You have to login first before accessing this page.";
            return RedirectToAction("Index");
            
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
                             CreatedDate = u.CreatedDate,
                             

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

    
}

