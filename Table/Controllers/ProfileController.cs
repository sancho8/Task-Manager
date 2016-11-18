using System;
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
                // Get authorization cookie.
                HttpCookie cookie = Request.Cookies["Authorization"];
                // Check if cookie not initialized.
                if (cookie == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                // Check if cookie is empty.
                if (String.IsNullOrWhiteSpace(cookie["Id"].ToString()))
                {
                    return RedirectToAction("Index", "Home");
                }
                // Check if user login exist
                String login = HttpUtility.UrlDecode(cookie["Login"]);
                if (login == null || String.IsNullOrWhiteSpace(login))
                {
                    return RedirectToAction("Index", "Home");
                }
                // Get id of user.
                int id = Int32.Parse(cookie["Id"]);
                // Get number of user's all tasks.
                int allTasks = (from e in context.Tasks
                                where e.UserId == id
                                select e).Count();
                // Get number of users's completed tasks.
                int completedTasks = (from e in context.Tasks
                                      where e.UserId == id && e.IsComplete == true
                                      select e).Count();
                // Get number of all tasks that are not completed.
                // And date specified for this task already gone. 
                int overduedTasks = (from e in context.Tasks
                                     where e.UserId == id && e.Data < DateTime.Now && e.Data.Value != null
                                     select e).Count();
                // Get current user for passing as model to view
                var currentUser = context.Users.Single(x => x.Login == login);
                // Needs for using in 'Profile information' section
                ViewBag.UserEmail = currentUser.Email.ToString();
                // Needs for using in 'Profile information' section 
                ViewBag.UserNeedDelivery = currentUser.NeedDelivery.ToString();
                // Needs for display in view
                ViewBag.UserLogin = login;
                return View("Home", currentUser);
            }
        }

        // Called after submiting 'Change profile' form
        [HttpPost]
        public ActionResult UpdateProfile(string login, string email, string oldPassword,
                                        string newPassword, string confirmPassword)
        {
            using (TaskContext context = new TaskContext())
            {
                // Get authorization cookie
                HttpCookie cookie = Request.Cookies["Authorization"];
                // Get user id
                int id = Int32.Parse(cookie["id"]);
                // Get current user from database
                var currentUser = (from e in context.Users
                                   where e.Id == id
                                   select e).Single();

                // Check if user login was changed in form.
                if (!String.IsNullOrWhiteSpace(login) && (currentUser.Login != login))
                {
                    // Check if length of user login is correct.
                    if (login.Length < 5)
                    {
                        ViewBag.ValidateErrorMessage = "Логин должен быть длиннее 5 символов";
                        return Index();
                    }
                    // Check if this login dont match with any other.
                    if (context.Users.Any(o => o.Login == login))
                    {
                        ViewBag.ValidateErrorMessage = "Логин уже используется";
                        return Index();
                    }
                    // Set new value of user login.
                    currentUser.Login = login;
                    // Update login value in database and in cookie.
                    context.SaveChanges();
                    cookie["Login"] = login;
                    Response.Cookies.Add(cookie);
                }

                // Check if user email field was changed.
                if (!String.IsNullOrWhiteSpace(email) && (currentUser.Email != email))
                {
                    // Regular expression for validating email value.
                    Regex valEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    // Check if email is correct.
                    if (!valEmail.IsMatch(email))
                    {
                        ViewBag.ValidateErrorMessage = "Введите корректный почтовый ящик";
                        return Index();
                    }
                    // Check if this email don't match any other.
                    if (context.Users.Any(o => o.Email == email))
                    {
                        ViewBag.ValidateErrorMessage = "Почтовый ящик уже используется";
                        return Index();
                    }
                    // Update value of user email and update.
                    currentUser.Email = email;
                    context.SaveChanges();
                }

                // Check if password field in form was changed.
                if (!String.IsNullOrWhiteSpace(oldPassword) || !String.IsNullOrWhiteSpace(newPassword) || !String.IsNullOrWhiteSpace(confirmPassword))
                {
                    // Check if old password entered correctly.
                    if (oldPassword != currentUser.Password)
                    {
                        ViewBag.ValidateErrorMessage = "Введите корректный текущий пароль";
                        return Index();
                    }
                    // Regular expression for validating user's password.
                    Regex valPassword = new Regex(@"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^(.{8,15})$");
                    // Check if new password is correct
                    if (!valPassword.IsMatch(newPassword))
                    {
                        ViewBag.ValidateErrorMessage = "Введите корректный пароль";
                        return Index();
                    }
                    // Check if password confirmed correctly
                    if (newPassword != confirmPassword)
                    {
                        ViewBag.ValidateErrorMessage = "Пароли не совпадают";
                        return Index();
                    }
                    // Update value of user password and save changes.
                    currentUser.Password = newPassword;
                    context.SaveChanges();
                }
            }
            return Index();
        }

        // Change delivery status.
        [HttpPost]
        public void ChangeDeliveryStatus(bool status)
        {
            // Get authorization cookie
            HttpCookie cookie = Request.Cookies["Authorization"];
            // Get user id
            int id = Int32.Parse(cookie["Id"]);
            using (TaskContext context = new TaskContext())
            {
                // Get current user from database
                var currentUser = (from e in context.Users
                         where e.Id == id
                         select e).Single();
                // Change delivery status of current user
                currentUser.NeedDelivery = status;
                // Save changes to database
                context.SaveChanges();
            }
        }

        // Get task stats(for displlay on profile page in infographical charts)
        [HttpGet]
        public int getTaskStats(string param)
        {
            using (TaskContext context = new TaskContext())
            {
                // Get authorization cookie.
                HttpCookie cookie = Request.Cookies["Authorization"];
                // Get user id
                int id = Int32.Parse(cookie["Id"]);
                // Check param that specify, number of which tasks needs return.
                switch (param)
                {
                    // Return number of all uncompleted tasks.
                    case "uncompleted":
                        return (from e in context.Tasks
                                where e.UserId == id && e.IsComplete == false
                                select e).Count();
                    // Return number of all completed tasks.
                    case "completed":
                        return (from e in context.Tasks
                                where e.UserId == id && e.IsComplete == true
                                select e).Count();
                    // Return number of all tasks that are 'up-to-date'
                    case "up-to-date":
                        return (from e in context.Tasks
                                where e.UserId == id && e.Data < DateTime.Now && e.IsComplete == false
                                select e).Count();
                    default: return 0;
                }
            }
        }

        // Get stats for task by priority (for disply in infographical charts on profile page).
        // param must consist 2 symbols: 
        // First symbol: if 'c' it means completed tasks, other symbols - uncompleted
        // Second symbol specify priority letter, 'N' means - without priority
        [HttpGet]
        public int getPriorityStats(string param)
        {
            using (TaskContext context = new TaskContext())
            {
                // Get authorization cookie
                HttpCookie cookie = Request.Cookies["Authorization"];
                // Get id of current user
                int id = Int32.Parse(cookie["Id"]);
                // Return stats for completed tasks
                if (param[1] == 'c')
                {
                    // Return number of completed task with special priority
                    switch (param[0])
                    {
                        case 'A':
                            return (from e in context.Tasks
                                 where e.UserId == id && e.Priority.Contains("A") && e.IsComplete==true
                                 select e).Count();
                        case 'B':
                            return (from e in context.Tasks
                                    where e.UserId == id && e.Priority.Contains("B") && e.IsComplete == true
                                    select e).Count();
                        case 'C':
                            return (from e in context.Tasks
                                    where e.UserId == id && e.Priority.Contains("C") && e.IsComplete == true
                                    select e).Count();
                        case 'D':
                            return (from e in context.Tasks
                                    where e.UserId == id && e.Priority.Contains("D") && e.IsComplete == true
                                    select e).Count();
                        case 'N':
                            return (from e in context.Tasks
                                    where e.UserId == id && e.Priority==" " && e.IsComplete == true
                                    select e).Count();
                    }
                }
                // Return stats for uncompleted tasks.
                else
                {
                    // Return number of completed task with special priority.
                    switch (param[0])
                    {
                        case 'A':
                            return (from e in context.Tasks
                                    where e.UserId == id && e.Priority.Contains("A")
                                    select e).Count();
                        case 'B':
                            return (from e in context.Tasks
                                    where e.UserId == id && e.Priority.Contains("B")
                                    select e).Count();
                        case 'C':
                            return (from e in context.Tasks
                                    where e.UserId == id && e.Priority.Contains("C")
                                    select e).Count();
                        case 'D':
                            return (from e in context.Tasks
                                    where e.UserId == id && e.Priority.Contains("D")
                                    select e).Count();
                        case 'N':
                            return (from e in context.Tasks
                                    where e.UserId == id && e.Priority==" "
                                    select e).Count();
                    }
                }
                return 0;
            }
        }
    }
}