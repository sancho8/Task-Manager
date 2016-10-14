using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
            ViewBag.UserLogin = cookie["Login"];
            return View(page);
        }

        public ActionResult Login()
        {
            return PartialView("LoginWindow");
        }

        public ActionResult SendFeedback(string name, string email, string feedback)
        {
            return RedirectToAction("Index","Home");
        }
    }
}