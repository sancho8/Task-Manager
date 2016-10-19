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
            cookie.Expires = DateTime.MaxValue;
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
            if (cookie["Login"] != "")
            {
                return RedirectToAction("Index", "Task");
            }
            else
            {
                return MoveToPage("About");
            }
        }

        public ActionResult MoveToHomePage()
        {
            HttpCookie cookie = Request.Cookies["Authorization"];
            ViewBag.UserLogin = HttpUtility.UrlDecode(cookie["Login"]);
            return View("Home");
        }

        public ActionResult MoveToPage(string page)
        {
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

        public ActionResult Login()
        {
            return PartialView("LoginWindow");
        }

        public ActionResult SendFeedback(string name, string email, string feedback)
        {
            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress("yaroshenko.aleksandr8@gmail.com", "Task-Manager");
            // кому отправляем
            MailAddress to = new MailAddress("98sancho@ukr.net");
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = "Отзыв о проекте Doer task-manager";
            // текст письма
            m.Body = "<h2>От: " + name + ", Email: " + email + "</h2><br />";
            m.Body += "<h2>" + feedback + "</h2>";
            // письмо представляет код html
            m.IsBodyHtml = true;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            // логин и пароль
            smtp.Credentials = new NetworkCredential("yaroshenko.aleksandr8@gmail.com", "assasin777");
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = true;
            smtp.Send(m);
            return RedirectToAction("Index","Home");
        }
    }
}