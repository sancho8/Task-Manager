using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Table.Models;

namespace Table.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            HttpCookie cookie = new HttpCookie("");
            // Session not ends.
            cookie.Expires = DateTime.MaxValue;
            // Check if user is logged in.
            try
            {
                cookie = Request.Cookies["Authorization"];
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
            if (cookie == null)
            {
                cookie = new HttpCookie("Authorization");
                cookie["Login"] = "";
                Response.Cookies.Add(cookie);
                return MoveToPage("About");
            }
            // If loginned - then redirect to task table page.
            if (cookie["Login"] != "")
            {
                return RedirectToAction("Index", "Task");
            }
            // By default - redirect to about page.
            else
            {
                return MoveToPage("About");
            }
        }

        // Action called when profile icon is clicked
        public ActionResult MoveToHomePage()
        {
            HttpCookie cookie = Request.Cookies["Authorization"];
            ViewBag.UserLogin = HttpUtility.UrlDecode(cookie["Login"]);
            string id = cookie["Id"];
            using (TaskContext context = new TaskContext())
            {
                User a = (User) (from e in context.Users
                        where e.Id.ToString() == id
                        select e).Single();
                return View("Home",a);
            }
        }

        // Redirectong to pages(called from header menu)
        public ActionResult MoveToPage(string page)
        {
             // For pages 'Tasks' and 'Home' there are some default controller actions.
             // For other pages we just return view.
            if(page == "Tasks")
            {
                return RedirectToAction("Index", "Task");
            }
            if (page == "Home")
            {
                return RedirectToAction("Index", "Profile");
            }
            HttpCookie cookie = Request.Cookies["Authorization"];
            // Parap for displaying user login in view
            ViewBag.UserLogin = HttpUtility.UrlDecode(cookie["Login"]);
            return View(page);
        }

        // Function for sendin feedback from contacts page.
        public ActionResult SendFeedback(string name, string email, string feedback)
        {
            // Identify sender.
            MailAddress from = new MailAddress("Doer.manager@gmail.com", "Task-Manager");
            // Identify applier.
            MailAddress to = new MailAddress("98sancho@ukr.net");
            // Creating message object.
            MailMessage m = new MailMessage(from, to);
            // Message subject.
            m.Subject = "Отзыв о проекте Doer task-manager";
            // Message text.
            m.Body = "<h2>От: " + name + ", Email: " + email + "</h2><br />";
            m.Body += "<h2>" + feedback + "</h2>";
            // Specify, that body is html.
            m.IsBodyHtml = true;
            // Set address of smtp-server and port.
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            // Login and password of sender account.
            smtp.Credentials = new NetworkCredential("Doer.manager@gmail.com", "doermanager777");
            // Specify delivery method.
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            // Turn ssl on.
            smtp.EnableSsl = true;
            // Sending message.
            smtp.Send(m);
            return RedirectToAction("Index","Home");
        }
    }
}