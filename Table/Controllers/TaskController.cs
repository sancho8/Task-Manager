using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Table.Models;
using System.Data.Entity;

namespace Table.Controllers
{

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
            GetTaskInPartialView();
            try
            {
                HttpCookie cookie = Request.Cookies["Authorization"];
                ViewBag.UserLogin = cookie["Login"];
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
            return View("Tasks");
        }

        public ActionResult DeleteTask(string id)
        {
            /*using (var conn = new SqlConnection(connection))
            {
                using (var cmd = new SqlCommand("DELETE FROM Tasks WHERE Id='" + id + "'", conn))
                {
                    SqlDataAdapter adapt = new SqlDataAdapter(cmd);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            using (var conn = new SqlConnection(connection))
            {
                using (var cmd = new SqlCommand("UPDATE Tasks SET Id = Id - 1 WHERE Id > '" + id + "'", conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }*/
            using (TaskContext context = new TaskContext())
            {
                int taskToDeleteId = Int32.Parse(id);
                var taskToRemove = context.Tasks.FirstOrDefault(x => x.Id == taskToDeleteId);
                if(taskToRemove != null)
                {
                    context.Tasks.Remove(taskToRemove);
                    context.SaveChanges();
                }
            }
            return GetTaskInPartialView();
        }

        [HttpPost]
        public ActionResult AddTask(string description, string data, char priority, int number)
        {
                //string command =
                //  "INSERT INTO Tasks (Id, UserId, Description, Data, Priority, Number, IsComplete) VALUES (@Id, @UserId, @Description, @Data, @Priority, @Number, @IsComplete)";

                //getting number of new id for added tak
                using (TaskContext context = new TaskContext())
                {
                    HttpCookie cookie = Request.Cookies["Authorization"];
                    int userId = Int32.Parse(cookie["Id"]);
                    var command = "SELECT TOP 1 Id FROM Tasks ORDER BY Id DESC";
                    int newId = context.Database.SqlQuery<int>(command).Single() + 1;
                    var taskToAdd = new Task(newId, userId, description, DateTime.Now, priority.ToString(), number, false);
                    context.Tasks.Add(taskToAdd);
                    context.SaveChanges();
                }
                return GetTaskInPartialView();
                /*int Id = 0;
                try
                {
                    HttpCookie cookie = Request.Cookies["Authorization"];
                    Id = Int32.Parse(cookie["Id"]);
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    return View("Error");
                }
                int newId;
                using (SqlConnection thisConnection = new SqlConnection(connection))
                {
                    using (SqlCommand cmdCount = new SqlCommand("SELECT TOP 1 Id FROM Tasks ORDER BY Id DESC", thisConnection))
                    {
                        thisConnection.Open();
                        newId = (int)(cmdCount.ExecuteScalar()) + 1;
                        TaskContext db = new TaskContext();
                        db.Tasks.Add(new Task(newId, Id, description, DateTime.Parse(data), priority.ToString(), number, false));
                        db.SaveChanges();
                    }
                }
                return GetTaskInPartialView();*/
                /*SqlCommand cmd = new SqlCommand(command);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                cmd.Parameters.AddWithValue("@Id", newId);
                cmd.Parameters.AddWithValue("@UserId", Id);
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@Data", data + ":00");
                cmd.Parameters.AddWithValue("@Priority", priority);
                cmd.Parameters.AddWithValue("@Number", number);
                cmd.Parameters.AddWithValue("@IsComplete", false);
                con.Open();
                cmd.ExecuteNonQuery();
                try
                {
                    HttpCookie cookie = Request.Cookies["Authorization"];
                    GetTasksFromDatabase(cookie["Id"]);
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    return View("Error");
                }
                return GetTaskInPartialView();*/
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
                    case "Description": return PartialView("TaskRows", tasks.ToList().OrderBy(o => o.Description));
                    case "Data": return PartialView("TaskRows", tasks.ToList().OrderBy(o => o.Data));
                    case "Priority": return PartialView("TaskRows", tasks.ToList().OrderBy(o => o.Priority));
                    case "Number": return PartialView("TaskRows", tasks.ToList().OrderBy(o => o.Number));
                    case "IsComplete": return PartialView("TaskRows", tasks.ToList().OrderBy(o => o.IsComplete));
                    default: return PartialView("TaskRows", tasks.ToList());
                }
            }
        }

        [HttpPost]
        public ActionResult UpdateTask(string id, string description, string data, string priority, string number, string isComplete)
        {
           /* using (var conn = new SqlConnection(connection))
            {
                string command = @"UPDATE Tasks SET " + 
                   "Description=' " + description + "'," +
                   "Data = ' " + data + "'," +
                   "Priority = ' " + priority + "'," +
                   "Number = ' " + number + "'," +
                   "IsComplete = ' " + isComplete + "'" +
                   " WHERE Id = '" + id + "'";

                using (var cmd = new SqlCommand(command, conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }*/
            using(TaskContext context = new TaskContext())
            {
                HttpCookie cookie = Request.Cookies["Authorization"];
                int userId = Int32.Parse(cookie["Id"]);
                var taskToUpdate = context.Tasks.Find(Int32.Parse(id));
                var updateTask = new Task(Int32.Parse(id), userId, description, DateTime.Parse(data), priority, Int32.Parse(number), Boolean.Parse(isComplete));
                if(taskToUpdate != null)
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

        /*private void GetTasksFromDatabase(string UserID)
        {
            TaskList.Clear();

            if (UserID == "") { return; }

            DataTable table = new DataTable();

            using (var conn = new SqlConnection(connection))
            {
                string command = "SELECT * FROM Tasks WHERE UserId = " + UserID;

                using (var cmd = new SqlCommand(command, conn))
                {
                    SqlDataAdapter adapt = new SqlDataAdapter(cmd);
                    conn.Open();
                    adapt.Fill(table);
                    conn.Close();
                }
            }
            foreach (DataRow row in table.Rows)
            {
                int Id = Int32.Parse(row["Id"].ToString());
                int UserId = Int32.Parse(row["UserId"].ToString());
                string Description = row["Description"].ToString();
                string Data = row["Data"].ToString();
                string Priority = row["Priority"].ToString();
                int Number = Int32.Parse(row["Number"].ToString());
                bool IsCompleted = Boolean.Parse(row["IsComplete"].ToString());
                TaskList.Add(new Task(Id, UserId, Description, Convert.ToDateTime(Data), Priority, Number, IsCompleted));
            }
        }*/

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
                    case TaskMode.CalendarMode: return PartialView("CalendarMode", tasks.ToList());
                    default: return Index();
                }
            }
        }

       /* public ActionResult GetTaskInPartialViewByMode(string partialViewName)
        {
            try
            {
                HttpCookie cookie = Request.Cookies["Authorization"];
                //GetTasksFromDatabase(cookie["Id"]);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
            ViewBag.Tasks = TaskList;
            return PartialView(partialViewName, TaskList);
        }*/

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