using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Table.Jobs
{
    public class EmailSender: IJob
    {
        public void Execute(Quartz.IJobExecutionContext context)
        {
            //sender
            MailAddress from = new MailAddress("yaroshenko.aleksandr8@gmail.com", "Task-Manager");
            //applier
            MailAddress to = new MailAddress("98sancho@ukr.net");
            //creating message object
            MailMessage m = new MailMessage(from, to);
            //message subject
            m.Subject = "Отзыв о проекте Doer task-manager";
            //message text
            m.Body = "<h2>От: " + "sancho" + ", Email: " + "email" + "</h2><br />";
            m.Body += "<h2>" + "feedback" + "</h2>";
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
            //sending
            smtp.Send(m);
        }
    }
}