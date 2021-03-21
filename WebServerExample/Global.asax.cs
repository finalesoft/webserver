using Finale.WebServer;
using System;
using System.Web.Routing;

namespace WebServerExample {
    public class Global : System.Web.HttpApplication {
        protected void Application_Start(object sender, EventArgs e) {
            RouteTable.Routes.MapRoute(
                "{*anything}",
                defaults: new { controller = "Default", action = "Default" }
            );
        }
    }
}