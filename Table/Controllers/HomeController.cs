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
        string connection = @"Data Source=.\SQLEXPRESS;AttachDbFilename='|DataDirectory|\Task_Database.mdf';Integrated Security=True;User Instance=True;";
        public static List<Task> TaskList = new List<Task>();
        public int UserID = 2;
        // GET: Home
        public ActionResult Index()
        {
            GetTasksFromDatabase();
            //DbExecuteCommand("Select * from Users");
            ViewBag.Tasks = TaskList;
            return View("Index");
        }

        [HttpPost]
        public ActionResult AddTask(string description, string data, int priority, bool iscompleted)
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

                SqlCommand cmd = new SqlCommand(command);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                cmd.Parameters.AddWithValue("@Id", newId);
                cmd.Parameters.AddWithValue("@UserId", UserID);
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@Data", data);
                cmd.Parameters.AddWithValue("@Priority", priority);
                cmd.Parameters.AddWithValue("@IsComplete", iscompleted);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            GetTasksFromDatabase();
            ViewBag.Tasks = TaskList;
            return View("Index");
        }

        public ActionResult OrderTasks(string prop)
        {
            switch (prop)
            {
                case "Id": TaskList = TaskList.OrderBy(o => o.Id).ToList(); break;
                case "UserId": TaskList = TaskList.OrderBy(o => o.UserId).ToList(); break;
                case "Description": TaskList = TaskList.OrderBy(o => o.Description).ToList(); break;
                case "Data": TaskList = TaskList.OrderBy(o => o.Data).ToList(); break;
                case "Priority": TaskList = TaskList.OrderBy(o => o.Priority).ToList(); break;
                case "IsCompleted": TaskList = TaskList.OrderBy(o => o.IsComplete).ToList(); break;
            }
            ViewBag.Tasks = TaskList;
            return View("Index");
        }

        private void GetTasksFromDatabase()
        {
            TaskList.Clear();

            DataTable table = new DataTable();

            using (var conn = new SqlConnection(connection))
            {
                string command = "SELECT * FROM Tasks WHERE UserId = " + UserID.ToString();

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