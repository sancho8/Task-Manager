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
        //return true if login and password values are correct
        [HttpPost]
        public bool LogInUser(string login, string password)
        {
            using (TaskContext context = new TaskContext())
            {
                try
                {
                    // Try to find user with values of login and password in params.
                    var a = (from e in context.Users
                             where e.Login == login && e.Password == password
                             select e).Single();
                    // Get authentication cookie.
                    HttpCookie authCookie = new HttpCookie("Authorization"); 
                    // Session never ends.
                    authCookie.Expires = DateTime.MaxValue;

                    // Set values of authentication cookie.
                    authCookie["Login"] = HttpUtility.UrlEncode(login);
                    authCookie["Id"] = a.Id.ToString();
                    // Update cookie.
                    Response.Cookies.Add(authCookie);
                    // Initialize param for displaying user login in view.
                    ViewBag.UserLogin = login; 
                    // Reload current page with new(authenticated) status of user.
                    Redirect(Request.UrlReferrer.ToString()); 
                    // It means that params are correct and operation comleted succesfully.
                    return true;
                }
                catch (Exception ex)
                {
                    // It means that params is incorrect and operation failed.
                    return false;
                }
            }
        }

        public ActionResult LogOutUser()
        {
            // Get authentication cookie.
            HttpCookie authCookie = new HttpCookie("Authorization"); 
            // Reset values of cookie.
            authCookie["Login"] = "";
            authCookie["Id"] = "";
            // Update cookie.
            Response.Cookies.Add(authCookie);
            return RedirectToAction("Index", "Home");
        }
        // Used for asynchronous validation(in time display error message).
        // First param - name of field, second param - value of field.
        // Return true if user with this param exist.
        [HttpPost]
        public bool CheckFormInput(string field, string value)
        {
            using (TaskContext context = new TaskContext())
            {
                switch (field)
                {
                    case "login":
                        // Return false if user with this login already exist.
                        if (context.Users.Any(o => o.Login == value))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    case "email":
                        // Return false if user with this email address already exist.
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

        // Script for validation of registration form and registering of user.
        // Return error message in case of incorrect input, return "True" in case of successfull operation.
        [HttpPost]
        public string RegisterUser(string login, string email, string password, string confirmPassword, bool needDelivery)
        {
            using (TaskContext context = new TaskContext())
            {
                // Check password mathcing.
                if (password != confirmPassword)
                {
                    return "Пароли не совпадают";
                }
                // Check if user with this login already exist.
                if (context.Users.Any(o => o.Login == login))
                {
                    return "Пользователь с таким логином уже существует";
                }
                // Check if user with this email already exist.
                if (context.Users.Any(o => o.Email == email))
                {
                    return "Пользователь с таким почтовым ящиком уже существует";
                }
                // Check length of login(must be 5 symbols or more).
                if (login.Length < 5)
                {
                    return "Логин должен иметь длинну более 5 символов";
                }
                // Regular expression for password validation.
                // Must have numbers, case-sensitive, at least 8 symbols.
                Regex valPassword = new Regex(@"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^(.{8,15})$");
                // Check if password match reg expression.
                if(!valPassword.IsMatch(password))
                {
                    return "Введите корректный пароль";
                }
                // Regular expression for email validation.
                Regex valEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                // Check if email matches reg expression.
                if (!valEmail.IsMatch(email))
                {
                    return "Введите корректный email";
                }
                try
                {
                    // Select new id number for inserting.
                    var command = "SELECT TOP 1 Id FROM Users ORDER BY Id DESC";
                    int id = context.Database.SqlQuery<int>(command).Single() + 1;
                    // Adding new user to database.
                    context.Users.Add(new Models.User(id, login, password, email, needDelivery, false));
                    // Saving chages to data
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                // Validation is successful
                return "True";
            }
        }

        // Script for registration through Facebook
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
    }
}