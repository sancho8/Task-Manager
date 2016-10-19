using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Table.Models;

namespace Table.Controllers
{
    [RequireHttps]
    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult Index()
        {
            using (TaskContext context = new TaskContext())
            {
                HttpCookie cookie = Request.Cookies["Authorization"];
                if (String.IsNullOrWhiteSpace(cookie["Id"].ToString()))
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

                String login = HttpUtility.UrlDecode(cookie["Login"]);
                ViewBag.UserLogin = login;
                var currentUser = context.Users.Single(x => x.Login == login);
                ViewBag.UserEmail = currentUser.Email.ToString();
                ViewBag.UserNeedDelivery = currentUser.NeedDelivery.ToString();

                return View("Home", currentUser);
            }
        }

        [HttpPost]
        public ActionResult UpdateProfile(string login, string email, string oldPassword,
                                        string newPassword, string confirmPassword)
        {
            using (TaskContext context = new TaskContext())
            {
                HttpCookie cookie = Request.Cookies["Authorization"];
                int id = Int32.Parse(cookie["id"]);
                var currentUser = (from e in context.Users
                                   where e.Id == id
                                   select e).Single();
                if(!currentUser.Password.Any(ch => ch < '0' || ch > '9')){
                    ViewBag.ValidateErrorMessage = "Этот тип аккаунта нельзя редактировать";
                    return Index();
                }
                if (!String.IsNullOrWhiteSpace(login)&&(currentUser.Login!=login))
                {
                    if (login.Length < 5)
                    {
                        ViewBag.ValidateErrorMessage = "Логин должен быть длиннее 5 символов";
                        return Index();
                    }
                    if(context.Users.Any(o => o.Login ==login))
                    {
                        ViewBag.ValidateErrorMessage = "Логин уже используется";
                        return Index();
                    }
                    currentUser.Login = login;
                    context.SaveChanges();
                    cookie["Login"] = login;
                    Response.Cookies.Add(cookie);
                }
                if (!String.IsNullOrWhiteSpace(email)&&(currentUser.Email!=email))
                {
                    Regex valEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    if (!valEmail.IsMatch(email))
                    {
                        ViewBag.ValidateErrorMessage = "Введите корректный почтовый ящик";
                        return Index();
                    }
                    if (context.Users.Any(o => o.Email == email))
                    {
                        ViewBag.ValidateErrorMessage = "Почтовый ящик уже используется";
                        return Index();
                    }
                    currentUser.Email = email;
                    context.SaveChanges();
                }
                if(!String.IsNullOrWhiteSpace(oldPassword) || !String.IsNullOrWhiteSpace(newPassword) || !String.IsNullOrWhiteSpace(confirmPassword))
                {
                    if(oldPassword != currentUser.Password)
                    {
                        ViewBag.ValidateErrorMessage = "Введите корректный текущий пароль";
                        return Index();
                    }
                    Regex valPassword = new Regex(@"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^(.{8,15})$");
                    if (!valPassword.IsMatch(newPassword))
                    {
                        ViewBag.ValidateErrorMessage = "Введите корректный пароль";
                        return Index();
                    }
                    if (newPassword != confirmPassword)
                    {
                        ViewBag.ValidateErrorMessage = "Пароли не совпадают";
                        return Index();
                    }
                    currentUser.Password = newPassword;
                    context.SaveChanges();
                }
            }
            return Index();
        }

        [HttpGet]
        public int getTaskStats(string param)
        {
            using (TaskContext context = new TaskContext())
            {
                HttpCookie cookie = Request.Cookies["Authorization"];
                int id = Int32.Parse(cookie["Id"]);
                switch (param)
                {
                    case "uncompleted": return (from e in context.Tasks
                                where e.UserId == id && e.IsComplete == false
                                select e).Count();
                    case "completed": return (from e in context.Tasks
                                where e.UserId == id && e.IsComplete == true
                                select e).Count();
                    case "up-to-date": return (from e in context.Tasks
                                where e.UserId == id && e.Data < DateTime.Now &&e.IsComplete == false
                                select e).Count();
                    default: return 0;
                }
            }
        }
    }
}