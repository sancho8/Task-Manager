using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Table.Models;
using System.Text.RegularExpressions;

namespace Table.Controllers
{
    [RequireHttps]
    public class AuthController : Controller
    {
        string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectToDatabase"].ToString();
        // GET: Auth
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public bool LogInUser(string login, string password)
        {
            using (TaskContext context = new TaskContext())
            {
                try
                {
                    var a = (from e in context.Users
                             where e.Login == login && e.Password == password
                             select e).Single();

                    HttpCookie authCookie = new HttpCookie("Authorization");
                    authCookie.Expires = DateTime.MaxValue;
                    authCookie["Login"] = HttpUtility.UrlEncode(login);
                    authCookie["Id"] = a.Id.ToString();
                    Response.Cookies.Add(authCookie);
                    ViewBag.UserLogin = login;
                    RedirectToAction("Index", "Tasks");
                    return true;    //correct user data
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public ActionResult LogOutUser()
        {
            HttpCookie authCookie = new HttpCookie("Authorization");
            authCookie["Login"] = "";
            authCookie["Id"] = "";
            Response.Cookies.Add(authCookie);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public bool CheckFormInput(string field, string value)
        {
            using (TaskContext context = new TaskContext())
            {
                switch (field)
                {
                    case "login":
                        if (context.Users.Any(o => o.Login == value))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    case "email":
                        if (context.Users.Any(o => o.Email == value))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    default: return true;
                }
            }
        }

        [HttpPost]
        public string RegisterUser(string login, string email, string password, string confirmPassword, bool needDelivery)
        {
            using (TaskContext context = new TaskContext())
            {
                if (password != confirmPassword)
                {
                    return "Пароли не совпадают";
                }
                if (context.Users.Any(o => o.Login == login))
                {
                    return "Пользователь с таким логином уже существует";
                }
                if (context.Users.Any(o => o.Email == email))
                {
                    return "Пользователь с таким почтовым ящиком уже существует";
                }
                if (login.Length < 5)
                {
                    return "Логин должен иметь длинну более 5 символов";
                }
                Regex valPassword = new Regex(@"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^(.{8,15})$");
                if(!valPassword.IsMatch(password))
                {
                    return "Введите корректный пароль";
                }
                Regex valEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                if (!valEmail.IsMatch(email))
                {
                    return "Введите корректный email";
                }
                try
                {
                    var command = "SELECT TOP 1 Id FROM Users ORDER BY Id DESC";
                    int id = context.Database.SqlQuery<int>(command).Single() + 1;
                    context.Users.Add(new Models.User(id, login, password, email, needDelivery, false));
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                return "True";
            }
        }

        [HttpPost]
        public void RegisterFacebook(string userid, string name, string email)
        {
            using (TaskContext context = new TaskContext())
            {
                if (context.Users.Any(o => o.Password == userid))
                {
                    var a = (from o in context.Users
                             where o.Password == userid
                             select o).Single();
                    a.Login = name;
                    a.Email = email;
                    context.SaveChanges();
                    LogInUser(a.Login, a.Password);
                }
                else
                {
                    var command = "SELECT TOP 1 Id FROM Users ORDER BY Id DESC";
                    int id = context.Database.SqlQuery<int>(command).Single() + 1;
                    context.Users.Add(new Models.User(id, name, userid, email, true, true));
                    context.SaveChanges();
                    LogInUser(name, userid);
                }
            }
        }

        [HttpPost]
        public void RegisterVK(string userid, string name, string email)
            {
            using (TaskContext context = new TaskContext())
            {
                if (context.Users.Any(o => o.Password == userid))
                {
                    var a = (from o in context.Users
                             where o.Password == userid
                             select o).Single();
                    a.Login = name;
                    a.Email = email;
                    context.SaveChanges();
                    LogInUser(a.Login, a.Password);
                }
                else
                {
                    var command = "SELECT TOP 1 Id FROM Users ORDER BY Id DESC";
                    int id = context.Database.SqlQuery<int>(command).Single() + 1;
                    context.Users.Add(new Models.User(id, name, userid, email, true, true));
                    context.SaveChanges();
                    LogInUser(name, userid);
                }
            }
        }
        [HttpPost]
        public void RegisterGoogle(string userid, string name, string email)
        {
            using (TaskContext context = new TaskContext())
            {
                if (context.Users.Any(o => o.Password == userid))
                {
                    var a = (from o in context.Users
                             where o.Password == userid
                             select o).Single();
                    a.Login = name;
                    a.Email = email;
                    context.SaveChanges();
                    LogInUser(a.Login, a.Password);
                }
                else
                {
                    var command = "SELECT TOP 1 Id FROM Users ORDER BY Id DESC";
                    int id = context.Database.SqlQuery<int>(command).Single() + 1;
                    context.Users.Add(new Models.User(id, name, userid, email, true));
                    context.SaveChanges();
                    LogInUser(name, userid);
                }
            }
        }
    }
}