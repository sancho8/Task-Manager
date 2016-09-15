using System;
using System.Collections.Generic;
using System.Data;
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
                    catch (Exception ex)
                    {
                        Id = "";
                        ViewBag.ErrorMessage = ex.Message;
                        return PartialView("Error");
                    }
                }
            }
            HttpCookie authCookie = new HttpCookie("Authorization");
            authCookie["Login"] = login;
            authCookie["Id"] = Id;
            Response.Cookies.Add(authCookie);
            ViewBag.UserLogin = login;
            return RedirectToAction("Index", "Task");
        }

        public ActionResult LogOutUser()
        {
            HttpCookie authCookie = new HttpCookie("Authorization");
            authCookie["Login"] = "";
            authCookie["Id"] = "";
            Response.Cookies.Add(authCookie);
            return RedirectToAction("MoveToPage","Home",new { page="About" });
        }

        public ActionResult RegisterUser(string login, string email, string password, string confirmPassword, string needDelivery)
        {
            string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectToDatabase"].ToString();
            using (SqlConnection con = new SqlConnection(connection))
            {
                string command =
                    "INSERT INTO Users (Id, Login, Password, Email, NeedDelivery) VALUES (@Id, @Login, @Password, @Email, @NeedDelivery)";

                if (password != confirmPassword)
                {
                    ViewBag.ErrorMessage = "Passwords not match";
                    return View("Error");
                }

                //getting number of new id for added user
                int newId;
                using (SqlConnection thisConnection = new SqlConnection(connection))
                {
                    using (SqlCommand cmdCount = new SqlCommand("SELECT TOP 1 Id FROM Users ORDER BY Id DESC", thisConnection))
                    {
                        thisConnection.Open();
                        newId = (int)(cmdCount.ExecuteScalar()) + 1;
                    }
                }
                SqlCommand cmd = new SqlCommand(command);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                cmd.Parameters.AddWithValue("@Id", newId);
                cmd.Parameters.AddWithValue("@Login", login);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@NeedDelivery", needDelivery);
                con.Open();
                cmd.ExecuteNonQuery();
                try
                {
                    HttpCookie cookie = Request.Cookies["Authorization"];

                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    return View("Error");
                }
                return RedirectToAction("Index", "Task");
            }
        }
    }
}