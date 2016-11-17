using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Table.Hubs;
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
            cookie.Expires = DateTime.MaxValue; //users always stay rememered
            //check if user is logged in
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
            //if loginned -> redirect to task table page
            if (cookie["Login"] != "")
            {
                return RedirectToAction("Index", "Task");
            }
            //by default -> redirect to about page
            else
            {
                return MoveToPage("About");
            }
        }

        //action called when profile icon is clicked
        public ActionResult MoveToHomePage()
        {
            HttpCookie cookie = Request.Cookies["Authorization"];
            ViewBag.UserLogin = HttpUtility.UrlDecode(cookie["Login"]);
            string id = cookie["ID"];
            using (TaskContext context = new TaskContext())
            {
                User a = (User) (from e in context.Users
                        where e.Id.ToString() == id
                        select e).Single();
                return View("Home",a);
            }
        }

        //redirectong to pages(called from header menu)
        public ActionResult MoveToPage(string page)
        {
             //for pages 'tasks' and 'home' there are actions onload
             //so we run them, in case of other page 
             //we directly show view
            if(page == "Tasks")
            {
                return RedirectToAction("Index", "Task");
            }
            if (page == "Home")
            {
                return RedirectToAction("Index", "Profile");
            }
            HttpCookie cookie = Request.Cookies["Authorization"];
            ViewBag.UserLogin = HttpUtility.UrlDecode(cookie["Login"]);
            return View(page);
        }

        public ActionResult SendFeedback(string name, string email, string feedback)
        {
            //sender
            MailAddress from = new MailAddress("yaroshenko.aleksandr8@gmail.com", "Task-Manager");
            //applier
            MailAddress to = new MailAddress("98sancho@ukr.net");
            //creating message object
            MailMessage m = new MailMessage(from, to);
            //message subject
            m.Subject = "Отзыв о проекте Doer task-manager";
            //message text
            m.Body = "<h2>От: " + name + ", Email: " + email + "</h2><br />";
            m.Body += "<h2>" + feedback + "</h2>";
            //specify, that body is html
            m.IsBodyHtml = true;
            //adress of smtp-server and port
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            //login and password of sender account
            smtp.Credentials = new NetworkCredential("yaroshenko.aleksandr8@gmail.com", "assasin777");
            //specify delivery method
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            //turn ssl on
            smtp.EnableSsl = true;
            //sending
            smtp.Send(m);
            return RedirectToAction("Index","Home");
        }
    }
}