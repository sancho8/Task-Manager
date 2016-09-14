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
        public static List<Task> TaskList = new List<Task>();

        enum TaskMode
        {
            SingleTableMode,
            MatrixMode,
            CalendarMode
        }

        private TaskMode taskMode = TaskMode.SingleTableMode;

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
            /*TaskContext context = new TaskContext();
            int ID = Int32.Parse(id);
            var itemToRemove = (from s1 in context.Tasks
                                where s1.Id == ID
                                select s1).First(); //returns a single item.

            if (itemToRemove != null)
            {
                context.Tasks.Remove(itemToRemove);
                context.SaveChanges();
            }*/
            using (var conn = new SqlConnection(connection))
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
            }
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
            return GetTaskInPartialView();
        }

        [HttpPost]
        public ActionResult AddTask(string description, string data, char priority, int number)
        {
            using (SqlConnection con = new SqlConnection(connection))
            {
                //string command =
                //  "INSERT INTO Tasks (Id, UserId, Description, Data, Priority, Number, IsComplete) VALUES (@Id, @UserId, @Description, @Data, @Priority, @Number, @IsComplete)";

                //getting number of new id for added tak
                int Id = 0;
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
                return GetTaskInPartialView();
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
        }

        public ActionResult OrderTasks(string prop)
        {
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
            switch (prop)
            {
                case "Description": TaskList = TaskList.OrderBy(o => o.Description).ToList(); break;
                case "Data": TaskList = TaskList.OrderBy(o => o.Data).ToList(); break;
                case "Priority": TaskList = TaskList.OrderBy(o => o.Priority).ToList(); break;
                case "Number": TaskList = TaskList.OrderBy(o => o.Number).ToList(); break;
                case "IsCompleted": TaskList = TaskList.OrderBy(o => o.IsComplete).ToList(); break;
            }
            switch (taskMode)
            {
                case TaskMode.SingleTableMode: return PartialView("TaskRows", TaskList);
                case TaskMode.MatrixMode: return PartialView("MatrixMode", TaskList);
                case TaskMode.CalendarMode: return PartialView("CalendarMode", TaskList);
                default: return Index();
            }
        }

        [HttpPost]
        public ActionResult UpdateTask(string id, string description, string data, string priority, string number, string isComplete)
        {
            using (var conn = new SqlConnection(connection))
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

        private void GetTasksFromDatabase(string UserID)
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
        }

        public ActionResult GetTaskInPartialView()
        {
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
            switch (taskMode)
            {
                case TaskMode.SingleTableMode: return PartialView("TaskRows", TaskList);
                case TaskMode.MatrixMode: return PartialView("MatrixMode", TaskList);
                case TaskMode.CalendarMode: return PartialView("CalendarMode", TaskList);
                default: return Index();
            }
        }

        public ActionResult GetTaskInPartialViewByMode(string partialViewName)
        {
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
            ViewBag.Tasks = TaskList;
            return PartialView(partialViewName, TaskList);
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