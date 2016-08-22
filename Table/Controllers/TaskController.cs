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
    public class TaskController : Controller
    {
        string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectToDatabase"].ToString();
        // @"Data Source=.\SQLEXPRESS;AttachDbFilename='|DataDirectory|\Task_Database.mdf';Integrated Security=True;User Instance=True;";
        public static List<Task> TaskList = new List<Task>();

        // GET: Task
        public ActionResult Index()
        {
            GetTaskInPartialView();
            return View("Tasks");
        }

        public ActionResult RedirectToView(string view)
        {
            return View(view);
        }

        public ActionResult DeleteTask(string id)
        {
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
            ViewBag.Tasks = TaskList;
            return GetTaskInPartialView();
        }

        [HttpPost]
        public ActionResult AddTask(string description, string data, char priority, int number)
        {
            using (SqlConnection con = new SqlConnection(connection))
            {
                string command =
                    "INSERT INTO Tasks (Id, UserId, Description, Data, Priority, Number, IsComplete) VALUES (@Id, @UserId, @Description, @Data, @Priority, @Number, @IsComplete)";

                //getting number of new id for added tak
                int newId;
                using (SqlConnection thisConnection = new SqlConnection(connection))
                {
                    using (SqlCommand cmdCount = new SqlCommand("SELECT TOP 1 Id FROM Tasks ORDER BY Id DESC", thisConnection))
                    {
                        thisConnection.Open();
                        newId = (int)(cmdCount.ExecuteScalar()) + 1;
                    }
                }
                int Id = 0;
                try
                {
                    HttpCookie cookie = Request.Cookies["Authorization"];
                    Id = Int32.Parse(cookie["Id"]);
                }
                catch (Exception ex)
                {

                }
                SqlCommand cmd = new SqlCommand(command);
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
                ViewBag.Tasks = TaskList;
                return GetTaskInPartialView();
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
            ViewBag.Tasks = TaskList;
            return PartialView("TaskRows");
        }

        public ActionResult OrderTasks(string prop)
        {
            switch (prop)
            {
                case "Description": TaskList = TaskList.OrderBy(o => o.IsComplete).ThenBy(o => o.Description).ToList(); break;
                case "Data": TaskList = TaskList.OrderBy(o => o.IsComplete).ThenBy(o => o.Data).ToList(); break;
                case "Priority": TaskList = TaskList.OrderBy(o => o.IsComplete).ThenBy(o => o.Priority).ToList(); break;
                case "Number": TaskList = TaskList.OrderBy(o => o.IsComplete).ThenBy(o => o.Number).ToList(); break;
                case "IsCompleted": TaskList = TaskList.OrderBy(o => o.IsComplete).ThenBy(o => o.IsComplete).ToList(); break;
            }
            ViewBag.Tasks = TaskList;
            return PartialView("TaskRows");
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
                TaskList.Add(new Task(Id, UserId, Description, Data, Priority, Number, IsCompleted));
            }
            TaskList.OrderBy(o => o.Id).ThenBy(o => o.IsComplete);
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