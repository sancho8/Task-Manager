using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Table.Models;
using System.Data.Entity;
using System.Globalization;

namespace Table.Controllers
{
    [RequireHttps]
    public class TaskController : Controller
    {
        string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectToDatabase"].ToString();
        // @"Data Source=.\SQLEXPRESS;AttachDbFilename='|DataDirectory|\Task_Database.mdf';Integrated Security=True;User Instance=True;";
        // public static List<Task> TaskList = new List<Task>();

        enum TaskMode
        {
            SingleTableMode,
            MatrixMode,
            CalendarMode
        }

        private static TaskMode taskMode = TaskMode.SingleTableMode;

        // GET: Task
        public ActionResult Index()
        { 
            HttpCookie cookie = Request.Cookies["Authorization"];
            ViewBag.UserLogin = HttpUtility.UrlDecode(cookie["Login"]);
            GetTaskInPartialView();
            return View("Tasks");
        }

        public ActionResult DeleteTask(string id)
        {
            using (TaskContext context = new TaskContext())
            {
                int taskToDeleteId = Int32.Parse(id);
                var taskToRemove = context.Tasks.FirstOrDefault(x => x.Id == taskToDeleteId);
                if (taskToRemove != null)
                {
                    context.Tasks.Remove(taskToRemove);
                    context.SaveChanges();
                }
            }
            return GetTaskInPartialView();
        }

        [HttpPost]
        public ActionResult AddTask(string description, string data, char? priority, int? number)
        {
            //getting number of new id for added tak
            using (TaskContext context = new TaskContext())
            {
                HttpCookie cookie = Request.Cookies["Authorization"];
                int userId = Int32.Parse(cookie["Id"]);
                var command = "SELECT TOP 1 Id FROM Tasks ORDER BY Id DESC";
                int newId = context.Database.SqlQuery<int>(command).Single() + 1;
                var taskToAdd = new Task(newId, userId, description, ParseData(data), priority.ToString(), number, false);
                context.Tasks.Add(taskToAdd);
                context.SaveChanges();
            }
            return GetTaskInPartialView();
        }

        private Nullable<DateTime> ParseData(string str)
        {
            DateTime date;
            return DateTime.TryParse(str, out date) ? DateTime.Parse(str) : (DateTime?)null;
        }

        public ActionResult OrderTasks(string prop)
        {
            using (TaskContext context = new TaskContext())
            {
                HttpCookie cookie = Request.Cookies["Authorization"];
                string UserID = cookie["Id"];
                var tasks = from e in context.Tasks
                            where e.UserId.Value.ToString() == UserID
                            orderby prop
                            select e;
                switch (prop)
                {
                    case "Description": return GetTaskInPartialViewWithList(tasks.ToList().OrderBy(o => o.Description));
                    case "Data": return GetTaskInPartialViewWithList(tasks.ToList().OrderByDescending(o => o.Data.HasValue).ThenBy(o => o.Data));
                    case "Priority": return GetTaskInPartialViewWithList(tasks.ToList().OrderBy(o => String.IsNullOrWhiteSpace(o.Priority)).ThenBy(o => o.Priority).ThenBy(o => o.Number));
                    case "Number": return GetTaskInPartialViewWithList(tasks.ToList().OrderByDescending(o => o.Number.HasValue).ThenBy(o => o.Number));
                    case "IsComplete": return GetTaskInPartialViewWithList(tasks.ToList().OrderBy(o => o.IsComplete));
                    default: return GetTaskInPartialView();
                }
            }
        }

        [HttpPost]
        public ActionResult UpdateTask(string id, string description, string data, string priority, string number, string isComplete)
        {
            using (TaskContext context = new TaskContext())
            {
                HttpCookie cookie = Request.Cookies["Authorization"];
                int userId = Int32.Parse(cookie["Id"]);
                int x;
                int? num = null;
                if (Int32.TryParse(number, out x))
                {
                    num = x;
                }
                var taskToUpdate = context.Tasks.Find(Int32.Parse(id));
                var updateTask = new Task(Int32.Parse(id), userId, description, ParseData(data), priority, num, Boolean.Parse(isComplete));
                if (taskToUpdate != null)
                {
                    context.Entry(taskToUpdate).CurrentValues.SetValues(updateTask);
                    context.SaveChanges();
                }
            }
            return GetTaskInPartialView();
        }

        [HttpPost]
        public ActionResult ChangeTaskStatus(string id, string value)
        {
            using (var conn = new SqlConnection(connection))
            {
                string command = "UPDATE Tasks SET IsComplete = '" + value + "'WHERE Id = '" + id + "'";

                using (var cmd = new SqlCommand(command, conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return GetTaskInPartialView();
        }

        public ActionResult GetTaskInPartialView()
        {
            using (TaskContext context = new TaskContext())
            {
                HttpCookie cookie = Request.Cookies["Authorization"];
                string UserID = cookie["Id"];
                var tasks = from e in context.Tasks
                            where e.UserId.Value.ToString() == UserID
                            select e;
                switch (taskMode)
                {
                    case TaskMode.SingleTableMode: return PartialView("TaskRows", tasks.ToList());
                    case TaskMode.MatrixMode: return PartialView("MatrixMode", tasks.ToList());
                    case TaskMode.CalendarMode: return UpdateTaskByDate();
                    default: return Index();
                }
            }
        }

        public ActionResult GetTaskInPartialViewWithList(System.Linq.IOrderedEnumerable<Table.Models.Task> list)
        {
            using (TaskContext context = new TaskContext())
            {
                HttpCookie cookie = Request.Cookies["Authorization"];
                string UserID = cookie["Id"];
                /*var tasks = from e in context.Tasks
                            where e.UserId.Value.ToString() == UserID
                            select e;*/
                switch (taskMode)
                {
                    case TaskMode.SingleTableMode: return PartialView("TaskRows", list);
                    case TaskMode.MatrixMode: return PartialView("MatrixMode", list);
                    case TaskMode.CalendarMode:
                        ViewBag.SelectedDate = DateTime.Now.Date;
                        return PartialView("CalendarMode", list);
                    default: return Index();
                }
            }
        }

        public static DateTime dt = DateTime.Now;

        public ActionResult UpdateTaskByDate()
        {
            using (TaskContext context = new TaskContext())
            {
                HttpCookie cookie = Request.Cookies["Authorization"];
                int UserID = Int32.Parse(cookie["Id"]);
                var a = from e in context.Tasks
                        where (e.UserId == UserID
                            && e.Data.Value.Day == dt.Day
                            && e.Data.Value.Month == dt.Month
                            && e.Data.Value.Year == dt.Year)
                        select e;
                return PartialView("CalendarMode", a.ToList());
            }
        }

        [HttpPost]
        public void ChangeDate(string date)
        {
            dt = DateTime.ParseExact(date.Substring(0, 24),
                  "ddd MMM d yyyy HH:mm:ss",
                  CultureInfo.InvariantCulture);
        }

        public ActionResult SingleTableMode()
        {
            taskMode = TaskMode.SingleTableMode;
            return GetTaskInPartialView();
        }

        public ActionResult MatrixMode()
        {
            taskMode = TaskMode.MatrixMode;
            return GetTaskInPartialView();
        }

        public ActionResult CalendarMode()
        {
            taskMode = TaskMode.CalendarMode;
            return GetTaskInPartialView();
        }

        [HttpPost]
        public bool IsValueExist(string param, string value)
        {
            if (param == "Login")
            {
                TaskContext context = new TaskContext();
                if (context.Users.Any(t => t.Login == value))
                {
                    return false;
                }
            }
            return true;
        }

        private void DbExecuteCommand(string command)
        {
            using (var conn = new SqlConnection(connection))
            {
                using (var cmd = new SqlCommand(command, conn))
                {
                    SqlDataAdapter adapt = new SqlDataAdapter(cmd);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
    }
}