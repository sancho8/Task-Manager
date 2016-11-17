using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Hosting;
using Table.Models;

namespace Table.Jobs
{
    public class EmailSender : IJob
    {
        public void Execute(Quartz.IJobExecutionContext context)
        {
            string errors = "";
            using (TaskContext taskContext = new TaskContext())
            {
                var a = from e in taskContext.Users
                        where e.NeedDelivery == true
                        select e;
                foreach (var user in a)
                {
                    //sender
                    MailAddress from = new MailAddress("yaroshenko.aleksandr8@gmail.com", "Task-Manager");
                    //applier
                    MailAddress to = new MailAddress(user.Email);
                    //creating message object
                    MailMessage m = new MailMessage(from, to);
                    m.SubjectEncoding = Encoding.UTF8;
                    m.BodyEncoding = Encoding.UTF8;
                    //message subject
                    m.Subject = "План задач Doer task-manager";
                    //message text
                    // m.Body = "<h2>От: " + "sancho" + ", Email: " + "email" + "</h2><br />";
                    //m.Body += "<h2>" + "feedback" + "</h2>";
                    //System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/EmailTemplates/DailyTasksMailTemplate.html"));
                    // HttpCookie cookie = Reqest.Cookies["Authorization"];
                    //string id = cookie["ID"];
                    DateTime dt = DateTime.Today;
                    var tasks = (from e in taskContext.Tasks
                                 where (e.UserId == user.Id
                            && e.Data.Value.Day == dt.Day
                            && e.Data.Value.Month == dt.Month
                            && e.Data.Value.Year == dt.Year)
                                 select e).ToList<Table.Models.Task>().OrderBy(t => t.Data);
                    m.Body = user.Login;
                    m.Body = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/EmailTemplates/DailyTasksMailTemplate.html"));
                    string generatedTasks = "";
                    foreach (Task t in tasks)
                    {
                        generatedTasks += "<tr>";
                        generatedTasks += "<td>" + t.Description + "</td>";
                        generatedTasks += "<td>" + t.Data.Value.TimeOfDay + "</td>";
                        generatedTasks += "<td>" + t.Priority+ "</td>";
                        generatedTasks += "<td>" + t.Number + "</td>";
                        if (t.IsComplete==true)
                        {
                            generatedTasks += "<td>" + "Выполнено" + "</td>";
                        }
                        else
                        {
                            generatedTasks += "<td>" + "Невыполнено" + "</td>";
                        }
                        generatedTasks += "</tr>";
                    }
                    m.Body = m.Body.Replace("*****", generatedTasks);
                    //specify, that body is html
                    m.IsBodyHtml = true;
                    //adress of smtp-server and port
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    //login and password of sender account
                    smtp.Credentials = new System.Net.NetworkCredential("yaroshenko.aleksandr8@gmail.com", "assasin777");
                    //specify delivery method
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    //turn ssl on
                    smtp.EnableSsl = true;
                    try
                    {
                        //sending
                        smtp.Send(m);
                    }
                    catch (Exception ex)
                    {
                        errors += ex.Message + "\n";
                    }
                }
            }
        }
    }
}