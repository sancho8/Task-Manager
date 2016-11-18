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
        // Connection to database
        string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectToDatabase"].ToString();

        // Modes of task view.
        enum TaskMode
        {
            SingleTableMode,
            MatrixMode,
            CalendarMode
        }

        // Current task mode.
        private static TaskMode taskMode = TaskMode.SingleTableMode;

        // Currently selected data for calendar mode
        private static DateTime dt = DateTime.Now;

        // GET: Task
        public ActionResult Index()
        { 
            //Get user login for diaplaying in view;
            HttpCookie cookie = Request.Cookies["Authorization"];
            ViewBag.UserLogin = HttpUtility.UrlDecode(cookie["Login"]);
            // Load user's tasks
            GetTaskInPartialView();
            return View("Tasks");
        }

        // Deleting task by its id(get from js-script).
        public ActionResult DeleteTask(string id)
        {
            using (TaskContext context = new TaskContext())
            {
                // Id of task that needs to delete.
                int taskToDeleteId = Int32.Parse(id);
                // Get task that needs to remove from db context.
                var taskToRemove = context.Tasks.FirstOrDefault(x => x.Id == taskToDeleteId);
                // Check if task exist.
                if (taskToRemove != null)
                {
                    // Remove this task and save changes to db.
                    context.Tasks.Remove(taskToRemove);
                    context.SaveChanges();
                }
            }
            // Load updated tasks.
            return GetTaskInPartialView();
        }

        // Add task function(form values came from js script)
        [HttpPost]
        public ActionResult AddTask(string description, string data, char? priority, int? number)
        {
            using (TaskContext context = new TaskContext())
            {
                HttpCookie cookie = Request.Cookies["Authorization"];
                try
                {
                    // Get user id.
                    int userId = Int32.Parse(cookie["Id"]);
                    // Find new task id number.
                    var command = "SELECT TOP 1 Id FROM Tasks ORDER BY Id DESC";
                    int newId = context.Database.SqlQuery<int>(command).Single() + 1;
                    // Create new task object.
                    var taskToAdd = new Task(newId, userId, description, ParseData(data), priority.ToString(), number, false);
                    // Add new task and save changes.
                    context.Tasks.Add(taskToAdd);
                    context.SaveChanges();
                }
                catch(Exception ex) { }
            }
            return GetTaskInPartialView();
        }

        // Parse data, return true if data passed correctly
        private Nullable<DateTime> ParseData(string str)
        {
            DateTime date;
            return DateTime.TryParse(str, out date) ? DateTime.Parse(str) : (DateTime?)null;
        }


        // Function for sorting tasks in table.
        // Get name of property by whic we sort tasks.
        public ActionResult OrderTasks(string prop)
        {
            using (TaskContext context = new TaskContext())
            {
                HttpCookie cookie = Request.Cookies["Authorization"];
                // Get user id
                string UserID = cookie["Id"];
                // Get user tasks
                var tasks = from e in context.Tasks
                            where e.UserId.Value.ToString() == UserID
                            orderby prop
                            select e;
                // Sort by property.
                // object with empty properties goes last.
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

        // Function for updating task.
        [HttpPost]
        public ActionResult UpdateTask(string id, string description, string data, string priority, string number, string isComplete)
        {
            using (TaskContext context = new TaskContext())
            {
                // Get user id
                HttpCookie cookie = Request.Cookies["Authorization"];
                int userId = Int32.Parse(cookie["Id"]);
                // Try parse number parameter to string. 
                // Buffer parameter for checking if casting is correct. 
                int x;
                // Parameter for using in task constructor
                int? num = null;
                if (Int32.TryParse(number, out x))
                {
                    num = x;
                }
                // Try parse number parameter to string. 
                // Buffer parameter for checking if casting is correct. 
                DateTime buf = new DateTime();
                // Parameter for using in task constructor
                Nullable<DateTime> date = new DateTime();
                if (DateTime.TryParse(data, out buf))
                {
                    date = DateTime.Parse(data);
                }
                else
                {
                    date = null;
                }
                // Get task with id
                var taskToUpdate = context.Tasks.Find(Int32.Parse(id));
                // Create new task with parameters from form
                var updateTask = new Task(Int32.Parse(id), userId, description, date, priority, num, Boolean.Parse(isComplete));
                // Update old task to new and save changes
                if (taskToUpdate != null)
                {
                    context.Entry(taskToUpdate).CurrentValues.SetValues(updateTask);
                    context.SaveChanges();
                }
            }
            return GetTaskInPartialView();
        }

        //Change task status (completed/uncompleted)
        [HttpPost]
        public ActionResult ChangeTaskStatus(string id, string value)
        {
            using (TaskContext context = new TaskContext())
            {
                // Get task id
                int Id = Int32.Parse(id);
                // Get task by id
                var a = (from e in context.Tasks
                        where e.Id == Id
                        select e).Single();
                // Set 'isComplete' field value to passed value to methd
                a.IsComplete = Boolean.Parse(value);
                // Save changes in db
                context.SaveChanges();
                return GetTaskInPartialView();
            }
        }

        // Load tasks in partial view.
        public ActionResult GetTaskInPartialView()
        {
            using (TaskContext context = new TaskContext())
            {
                // Get authorization cookie.
                HttpCookie cookie = Request.Cookies["Authorization"];
                // Get user id from cookie.
                string UserID = cookie["Id"];
                // Get all tasks for this user.
                var tasks = from e in context.Tasks
                            where e.UserId.Value.ToString() == UserID
                            select e;
                // Check current mode of task view.
                switch (taskMode)
                {
                    // Return tasks in special partial view for each mode
                    case TaskMode.SingleTableMode: return PartialView("TaskRows", tasks.ToList());
                    case TaskMode.MatrixMode: return PartialView("MatrixMode", tasks.ToList());
                    case TaskMode.CalendarMode: return UpdateTaskByDate();
                    default: return Index();
                }
            }
        }

        // Returns tasks with specified list (this script used in SortTasks() function).
        public ActionResult GetTaskInPartialViewWithList(System.Linq.IOrderedEnumerable<Table.Models.Task> list)
        {
            using (TaskContext context = new TaskContext())
            {
                // Get authorization cookie.
                HttpCookie cookie = Request.Cookies["Authorization"];
                // Get user id
                string UserID = cookie["Id"];
                // Check current mode of task view.
                switch (taskMode)
                {
                    // Return tasks in special partial view for each mode
                    case TaskMode.SingleTableMode: return PartialView("TaskRows", list);
                    case TaskMode.MatrixMode: return PartialView("MatrixMode", list);
                    case TaskMode.CalendarMode:
                        ViewBag.SelectedDate = DateTime.Now.Date;
                        return PartialView("CalendarMode", list);
                    default: return Index();
                }
            }
        }

        // Update calendar mode view when data is changed
        public ActionResult UpdateTaskByDate()
        {
            using (TaskContext context = new TaskContext())
            {
                // Get authorization cookie
                HttpCookie cookie = Request.Cookies["Authorization"];
                // Get user id
                int UserID = Int32.Parse(cookie["Id"]);
                // Get all task with selected date value
                var tasksByDate = from e in context.Tasks
                        where (e.UserId == UserID
                            && e.Data.Value.Day == dt.Day
                            && e.Data.Value.Month == dt.Month
                            && e.Data.Value.Year == dt.Year)
                        select e;
                //return tasks in partial view for 'Calendar' mode
                return PartialView("CalendarMode", tasksByDate.ToList());
            }
        }

        // Called when user change data in calendar mode
        [HttpPost]
        public void ChangeDate(string date)
        {
            // Change date and set it in correct format
            dt = DateTime.ParseExact(date.Substring(0, 24),
                  "ddd MMM d yyyy HH:mm:ss",
                  CultureInfo.InvariantCulture);
        }

        // Change task mode to 'Single table' mode
        public ActionResult SingleTableMode()
        {
            taskMode = TaskMode.SingleTableMode;
            return GetTaskInPartialView();
        }

        // Change task mode to 'Eizenhover's matrix' mode
        public ActionResult MatrixMode()
        {
            taskMode = TaskMode.MatrixMode;
            return GetTaskInPartialView();
        }

        // Change task mode to 'Calendar' mode
        public ActionResult CalendarMode()
        {
            taskMode = TaskMode.CalendarMode;
            return GetTaskInPartialView();
        }
    }
}