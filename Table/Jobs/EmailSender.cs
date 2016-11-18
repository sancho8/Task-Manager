using Quartz;
using System;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Hosting;
using Table.Models;

namespace Table.Jobs
{
    public class EmailSender : IJob
    {
        public void Execute(Quartz.IJobExecutionContext context)
        {
            using (TaskContext taskContext = new TaskContext())
            {
                var a = from e in taskContext.Users
                        where e.NeedDelivery == true
                        select e;
                foreach (var user in a)
                {
                    // Sender creation.
                    MailAddress from = new MailAddress("yaroshenko.aleksandr8@gmail.com", "Task-Manager");
                    // Applier creation.
                    MailAddress to = new MailAddress(user.Email);
                    // Creating message object.
                    MailMessage m = new MailMessage(from, to);
                    m.SubjectEncoding = Encoding.UTF8;
                    m.BodyEncoding = Encoding.UTF8;
                    // Setting message subject.
                    m.Subject = "План задач Doer task-manager";
                    // Initializing value for usage in LINQ expression
                    DateTime dt = DateTime.Today;
                    // Select tasks for message.
                    // Select all tasks of this user where data == today.
                    var tasks = (from e in taskContext.Tasks
                                 where (e.UserId == user.Id
                            && e.Data.Value.Day == dt.Day
                            && e.Data.Value.Month == dt.Month
                            && e.Data.Value.Year == dt.Year)
                                 select e).ToList<Table.Models.Task>().OrderBy(t => t.Data);
                    m.Body = user.Login;
                    // Read HTML template from exiting file.
                    m.Body = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/EmailTemplates/DailyTasksMailTemplate.html"));
                    // Initializing param for generate HTML code of task table. 
                    string generatedTasks = "";
                    // Generation of HTML code for each task.
                    // Create new table row with parameters of task object. 
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
                    // Replace 
                    m.Body = m.Body.Replace("*****", generatedTasks);
                    // Specify, that body is in HTML format.
                    m.IsBodyHtml = true;
                    // Set address of smtp-server and port.
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    // Specify login and password of sender account.
                    smtp.Credentials = new System.Net.NetworkCredential("yaroshenko.aleksandr8@gmail.com", "assasin777");
                    // Specify delivery method.
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    // Turn ssl on.
                    smtp.EnableSsl = true;
                    try
                    {
                        // Sending.
                        smtp.Send(m);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }
    }
}