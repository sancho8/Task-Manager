using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(PushNotifyApp.Startup))]
namespace PushNotifyApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}