using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Table.Controllers
{
    public class AuthController : Controller
    {
        string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectToDatabase"].ToString();
        // GET: Auth
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogInUser(string login, string password)
        {
            string command =
                   "SELECT Id FROM USERS WHERE Login = '" + login + "' AND Password = '" + password + "'";

            string Id;

            using (SqlConnection thisConnection = new SqlConnection(connection))
            {
                using (SqlCommand cmd = new SqlCommand(command, thisConnection))
                {
                    thisConnection.Open();
                    try
                    {
                        Id = cmd.ExecuteScalar().ToString();
                    }
                    catch(Exception ex)
                    {
                        Id = "1";
                    }
                }
            }
            HttpCookie authCookie = new HttpCookie("Authorization");
            authCookie["Login"] = login;
            authCookie["Password"] = password;
            authCookie["Id"] = Id;
            Response.Cookies.Add(authCookie);
            return RedirectToAction("Index", "Home");
        }

    }
}