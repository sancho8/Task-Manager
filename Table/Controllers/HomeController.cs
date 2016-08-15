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
        string connection = @"Data Source=.\SQLEXPRESS;AttachDbFilename='|DataDirectory|\Task_Database.mdf';Integrated Security=True;User Instance=True";
        public static List<Task> TaskList = new List<Task>();
        
        // GET: Home
        public ActionResult Index()
        {
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
                string command = "SELECT * FROM Tasks WHERE UserId = 2";

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
    }
}