using Finale.WebServer;
using System;
using System.Web.Routing;

namespace MvcExample {
    public class Global : System.Web.HttpApplication {
        protected void Application_Start(object sender, EventArgs e) {
            RouteTable.Routes.MapRoute(
                "{controller}/{action}"
            );

            RouteTable.Routes.MapRoute(
                "{*anything}",
                defaults: new { controller = "Default", action = "Default" }
            );
        }
    }
}