using System;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Web;

public class SendMailModule : System.Web.IHttpModule
{
    static Timer timer;
    long interval = 10000; //10 секунд
    static object synclock = new object();
    static bool sent = false;

    public void Init(HttpApplication app)
    {
        timer = new Timer(new TimerCallback(SendEmail), null, 0, interval);
    }
     
    private void SendEmail(object obj)
    {
        lock (synclock)
        {
            DateTime dd = DateTime.Now;
            if (sent == false)
            {
                // отправитель - устанавливаем адрес и отображаемое в письме имя
                MailAddress from = new MailAddress("yaroshenko.aleksandr8@gmail.com", "Task-Manager");
                // кому отправляем
                MailAddress to = new MailAddress("98sancho@ukr.net");
                // создаем объект сообщения
                MailMessage m = new MailMessage(from, to);
                // тема письма
                m.Subject = "Отзыв о проекте Doer task-manager";
                // текст письма
                m.Body = "<h2>Письмо по таймеру</h2><br />";
                m.Body += "<h2>" + DateTime.Now.ToString() + "</h2>";
                // письмо представляет код html
                m.IsBodyHtml = true;
                // адрес smtp-сервера и порт, с которого будем отправлять письмо
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                // логин и пароль
                smtp.Credentials = new NetworkCredential("yaroshenko.aleksandr8@gmail.com", "assasin777");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = true;
                smtp.Send(m);
                sent = true;
            }
        }
    }
    public void Dispose()
    { }
}