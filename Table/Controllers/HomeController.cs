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
    public class HomeController : Controller
    {
        string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectToDatabase"].ToString();
            // @"Data Source=.\SQLEXPRESS;AttachDbFilename='|DataDirectory|\Task_Database.mdf';Integrated Security=True;User Instance=True;";
        public static List<Task> TaskList = new List<Task>();
        // GET: Home
        public ActionResult Index()
        {
            try
            {
                HttpCookie cookie = Request.Cookies["Authorization"];
                GetTasksFromDatabase(cookie["Id"]);
            }
            catch (Exception ex)
            {

            }
            ViewBag.Tasks = TaskList;
            return View("Index");
        }

        [HttpPost]
        public ActionResult AddTask(string description, string data, int priority)
        {
            using (SqlConnection con = new SqlConnection(connection))
            {
                string command =
                    "INSERT INTO Tasks (Id, UserId, Description, Data, Priority, IsComplete) VALUES (@Id, @UserId, @Description, @Data, @Priority, @IsComplete)";

                //getting number of new id for added tak
                int newId;
                using (SqlConnection thisConnection = new SqlConnection(connection))
                {
                    using (SqlCommand cmdCount = new SqlCommand("SELECT COUNT(Id) FROM Tasks", thisConnection))
                    {
                        thisConnection.Open();
                        newId = (int)(cmdCount.ExecuteScalar()) + 1;
                    }
                }
                int Id = 1;
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
                cmd.Parameters.AddWithValue("@Data", data);
                cmd.Parameters.AddWithValue("@Priority", priority);
                cmd.Parameters.AddWithValue("@IsComplete", false);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            HttpCookie cook = Request.Cookies["Authorization"];
            GetTasksFromDatabase(cook["Id"]);
            ViewBag.Tasks = TaskList;
            return View("Index");
        }

        public ActionResult OrderTasks(string prop)
        {
            switch (prop)
            {
                case "Id": TaskList = TaskList.OrderBy(o => o.IsComplete).ThenBy(o => o.Id).ToList(); break;
                case "UserId": TaskList = TaskList.OrderBy(o => o.IsComplete).ThenBy(o => o.UserId).ToList(); break;
                case "Description": TaskList = TaskList.OrderBy(o => o.IsComplete).ThenBy(o => o.Description).ToList(); break;
                case "Data": TaskList = TaskList.OrderBy(o => o.IsComplete).ThenBy(o => o.Data).ToList(); break;
                case "Priority": TaskList = TaskList.OrderBy(o => o.IsComplete).ThenBy(o => o.Priority).ToList(); break;
                case "IsCompleted": TaskList = TaskList.OrderBy(o => o.IsComplete).ThenBy(o => o.IsComplete).ToList(); break;
            }
            ViewBag.Tasks = TaskList;
            return View("Index");
        }
        /*public ActionResult OrderTasks(string prop, bool asc)
        {
            switch (prop)
            {
                case "Id":
                    if (asc)
                    {
                        TaskList = TaskList.OrderBy(o => o.Id).ToList();
                    }
                    else
                    {
                        TaskList = TaskList.OrderByDescending(o => o.Id).ToList();
                    }
                    break;
                case "UserId": TaskList = TaskList.OrderBy(o => o.UserId).ToList(); break;
                case "Description": TaskList = TaskList.OrderBy(o => o.Description).ToList(); break;
                case "Data": TaskList = TaskList.OrderBy(o => o.Data).ToList(); break;
                case "Priority": TaskList = TaskList.OrderBy(o => o.Priority).ToList(); break;
                case "IsCompleted": TaskList = TaskList.OrderBy(o => o.IsComplete).ToList(); break;
            }
            ViewBag.Tasks = TaskList;
            return View("Index");
        }*/


        private void GetTasksFromDatabase(string UserID)
        {
            TaskList.Clear();

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
                int Priority = Int32.Parse(row["Priority"].ToString());
                bool IsCompleted = Boolean.Parse(row["IsComplete"].ToString());
                TaskList.Add(new Task(Id, UserId, Description, Data, Priority, IsCompleted));
            }
            TaskList.OrderBy(o => o.IsComplete).ThenBy(o => o.Id);
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