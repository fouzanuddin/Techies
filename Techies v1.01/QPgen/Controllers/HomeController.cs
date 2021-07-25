using QPgen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace QPgen.Controllers
{
    public class HomeController : Controller
    {
        databaseHelperController db = new databaseHelperController();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ContactUs()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            if(email != "")
            {
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(email);
                if (match.Success)
                {
                    int x = db.checkemail(email);
                    if(x == 1)
                    {
                        Session["email"] = email;
                        int z = sendotp(email);
                        if(z == 0)
                        {
                            return RedirectToAction("VerifyOtp", "Home");
                        }
                        else
                        {
                            ViewData["message"] = "Something went wrong!";
                        }
                    }
                    else
                    {
                        ViewData["message"] = "Email id is not registered";
                    }
                }
                else
                {
                    ViewData["message"] = "Invalid Email";
                }
            }
            else
            {
                ViewData["message"] = "Empty Form";
            }
            return View();
        }
        public ActionResult VerifyOtp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult VerifyOtp(string txtotp)
        {
            if(txtotp != "")
            {
                if(txtotp.Length == 6 && txtotp == Session["otp"].ToString())
                {
                    ViewBag.message = "done";
                }
                else
                {
                    ViewBag.message = "Invalid OTP";
                }
            }
            else
            {
                ViewBag.message = "Empty Form";
            }
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string pass1, string pass2)
        {
            if (pass1 != "" && pass2 != "")
            {
                if (pass1 == pass2)
                {
                    int id = db.getID(Session["email"].ToString());
                    int x = db.changepassword(id, pass1);
                    if (x == 1)
                    {
                        return Json("done");
                    }
                    else
                    {
                        return Json("Something went wrong");
                    }
                }
                else
                {
                    return Json("Password and Re-Entered Password does not match");
                }
            }
            else
            {
                return Json("Current password does not match");
            }
        }
        public ActionResult login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult login(Login l)
        {
            if (ModelState.IsValid)
            {
                int status = db.checklogin(l.username, l.password);
                if(status == 1)
                {
                    ViewBag.message = 1;
                    int id = db.getID(l.username);
                    Session["id"] = id.ToString();
                    if(id != 1)
                    {
                        return RedirectToAction("Dashboard","examiner");
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "admin");
                    }
                }
                else
                {
                    ViewBag.message = 0; 
                }
                return View();
            }
            return View();
        }
        int sendotp(string tempemail)
        {
            string[] data = new string[2];
            data[0] = "ladaebs14@gmail.com";
            data[1] = "Abhay@123";
            Random rand = new Random();
            int otp = rand.Next(100000, 999999);
            string body = "Your OTP for Email Verification is: " + otp;
            Session["otp"] = otp; 
            MailMessage mail = new MailMessage(data[0],
                tempemail,
                "Question Paper Generator System",
                body);
            mail.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");
            SmtpClient client = new SmtpClient("smtp.gmail.com");
            client.Port = 587;

            client.Credentials = new System.Net.NetworkCredential(data[0],
                data[1]);
            client.EnableSsl = true;
            int signal;
            try
            {
                client.Send(mail);
                signal = 0;
            }
            catch (SmtpException)
            {
                signal = 1;
            }
            return signal;
        }
    }
}