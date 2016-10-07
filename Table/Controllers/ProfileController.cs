using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Table.Models;

namespace Table.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult Index()
        {
            using (TaskContext context = new TaskContext())
            {
                HttpCookie cookie = Request.Cookies["Authorization"];
                if (cookie["Id"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                int id = Int32.Parse(cookie["Id"]);
                int allTasks = (from e in context.Tasks
                                where e.UserId == id
                                select e).Count();
                int completedTasks = (from e in context.Tasks
                                      where e.UserId == id && e.IsComplete == true
                                      select e).Count();
                int overduedTasks = (from e in context.Tasks
                                     where e.UserId == id && e.Data < DateTime.Now && e.Data.Value != null
                                     select e).Count();
                ViewBag.CompletedStat = completedTasks.ToString() + "/" + allTasks.ToString();
                ViewBag.UncompletedStats = overduedTasks.ToString();

                String login = cookie["Login"];
                var currentUser = context.Users.Single(x => x.Login == login);
                ViewBag.UserLogin = currentUser.Login.ToString();
                ViewBag.UserEmail = currentUser.Email.ToString();
                ViewBag.UserNeedDelivery = currentUser.NeedDelivery.ToString();

                return View("Home", currentUser);
            }
        }

        [HttpPost]
        public string UpdateProfileData(string login, string email, string oldPassword,
                                        string newPassword, string confirmPassword)
        {
            using (TaskContext context = new TaskContext())
            {
                if (context.Users.Any(o => o.Login == login))
                {
                    return "Пользоавтель с таким логином уже есть";
                }
                if (context.Users.Any(o => o.Email == email))
                {
                    return "Пользоавтель с таким почтовым адрессом уже есть";
                }
                else
                {
                    return "Success";
                }
            }
        }
    }
}